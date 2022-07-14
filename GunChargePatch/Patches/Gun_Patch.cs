using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Photon.Pun;
using UnityEngine;
using HarmonyLib;

namespace GunChargePatch.Patches
{
    [HarmonyPatch(typeof(Gun), "ResetStats")]
    class GunPatchResetStats
    {
        private static void Prefix(Gun __instance)
        {
            __instance.chargeDamageMultiplier = 1f;
            __instance.chargeEvenSpreadTo = 0f;
            __instance.chargeRecoilTo = 0f;
            __instance.chargeSpeedTo = 0f;
            __instance.chargeSpreadTo = 0f;
        }
    }

    [HarmonyPatch(typeof(Gun))]
    class Gun_Patch2
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Gun.Attack))]
        [HarmonyPriority(Priority.Last)]
        static void ResetCharge(Gun __instance, float charge, bool __result)
        {
            UnityEngine.Debug.Log($"Gun told to attack with a charge of {string.Format("{0:F2}", charge)}");

            if (__result)
            {
                __instance.currentCharge = 0f;
            }
        }
    }

    [HarmonyPatch(typeof(Gun))]
    class Gun_Patch
    {
        static Type GetNestedIDoBlockTransitionType()
        {
            var nestedTypes = typeof(Gun).GetNestedTypes(BindingFlags.Instance | BindingFlags.NonPublic);
            Type nestedType = null;

            foreach (var type in nestedTypes)
            {
                if (type.Name.Contains("FireBurst"))
                {
                    nestedType = type;
                    break;
                }
            }

            return nestedType;
        }
        [HarmonyTargetMethod]
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(GetNestedIDoBlockTransitionType(), "MoveNext");
        }

        static CodeInstruction[] OriginalFireBurstCode;
        static CodeInstruction[] NewFireBurstCode;

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            OriginalFireBurstCode = codes.ToArray();

            //for (var i = 0; i < codes.Count; i++)
            //{
            //    UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
            //}

            var getChargedMono = AccessTools.Method(typeof(GameObject), nameof(GameObject.GetComponent), generics: new Type[] { typeof(ChargedProjectileInit) });
            CodeInstruction getComponent = new CodeInstruction(OpCodes.Callvirt, getChargedMono);

            var getdebugLog = AccessTools.Method(typeof(UnityEngine.Debug), nameof(UnityEngine.Debug.Log), new Type[] { typeof(object) });
            CodeInstruction debugLog = new CodeInstruction(OpCodes.Call, getdebugLog);

            List<CodeInstruction> debug = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(codes[25].opcode, codes[25].operand),
                new CodeInstruction(OpCodes.Conv_R4),
                new CodeInstruction(codes[293].opcode, codes[293].operand),
                debugLog
            };

            var getOffline = AccessTools.Method(typeof(ChargedProjectileInit), nameof(ChargedProjectileInit.OFFLINE_InitCharge), new Type[] { typeof(int), typeof(int), typeof(float), typeof(float), typeof(float) });
            CodeInstruction OfflineInit = new CodeInstruction(OpCodes.Callvirt, getOffline);
            var getOfflineNoAmmo = AccessTools.Method(typeof(ChargedProjectileInit), nameof(ChargedProjectileInit.OFFLINE_Init_noAmmoUseCharge), new Type[] { typeof(int), typeof(int), typeof(float), typeof(float), typeof(float) });
            CodeInstruction OfflineInitNoAmmo = new CodeInstruction(OpCodes.Callvirt, getOfflineNoAmmo);
            var getOfflineSeparate = AccessTools.Method(typeof(ChargedProjectileInit), nameof(ChargedProjectileInit.OFFLINE_Init_SeparateGunCharge), new Type[] { typeof(int), typeof(int), typeof(int), typeof(float), typeof(float), typeof(float) });
            CodeInstruction OfflineInitSeparate = new CodeInstruction(OpCodes.Callvirt, getOfflineSeparate);

            List<CodeInstruction> chargeParam = new List<CodeInstruction>() { 
                new CodeInstruction(codes[24].opcode, codes[24].operand), 
                new CodeInstruction(codes[25].opcode, codes[25].operand), 
                new CodeInstruction(OpCodes.Conv_R4) 
            };

            List<CodeInstruction> photonChargeParam = new List<CodeInstruction>();
            photonChargeParam.Add(new CodeInstruction(OpCodes.Dup));
            photonChargeParam.AddRange(chargeParam);
            photonChargeParam.Add(new CodeInstruction(codes[293].opcode, codes[293].operand));
            photonChargeParam.Add(new CodeInstruction(OpCodes.Stelem_Ref));

            { // All the arrays sent by RPCs need to have their sizes increased by 1
                codes[142] = new CodeInstruction(OpCodes.Ldc_I4_5);
                codes[200] = new CodeInstruction(OpCodes.Ldc_I4_5);
                codes[258] = new CodeInstruction(OpCodes.Ldc_I4_6);
            }

            { // The RPCS need to be calling our function and not the base one.
                codes[140] = new CodeInstruction(OpCodes.Ldstr, "RPCA_InitCharge");
                codes[198] = new CodeInstruction(OpCodes.Ldstr, "RPCA_Init_noAmmoUseCharge");
                codes[256] = new CodeInstruction(OpCodes.Ldstr, "RPCA_Init_SeparateGunCharge");
            }

            { // We need to be getting our component instead of projectileinit
                codes[121] = getComponent;
                codes[179] = getComponent;
                codes[237] = getComponent;
            }

            { // We need to be calling our offline methods
                codes[136] = OfflineInit;
                codes[194] = OfflineInitNoAmmo;
                codes[252] = OfflineInitSeparate;
            }

            { // Start adding the parameters now, done last since it adjusts size.
                codes.InsertRange(295, photonChargeParam);
                codes.Insert(296, new CodeInstruction(OpCodes.Ldc_I4_5));
                codes.InsertRange(252, chargeParam);
                codes.InsertRange(232, photonChargeParam);
                codes.Insert(233, new CodeInstruction(OpCodes.Ldc_I4_4));
                codes.InsertRange(194, chargeParam);
                codes.InsertRange(174, photonChargeParam);
                codes.Insert(175, new CodeInstruction(OpCodes.Ldc_I4_4));
                codes.InsertRange(136, chargeParam);
            }

            { // Insert a debug statement
                codes.InsertRange(58, debug);
            }

            NewFireBurstCode = codes.ToArray();

            for (int i = 0; i < codes.Count; i++)
            {
                UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
            }

            return codes.AsEnumerable();
        }


    }
}