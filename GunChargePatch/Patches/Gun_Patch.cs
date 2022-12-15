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
    [HarmonyPatch(typeof(Gun))]
    class GunPatchDefaultStats
    {
        [HarmonyPostfix]
        [HarmonyPatch("ResetStats")]
        private static void SetOnReset(Gun __instance)
        {
            __instance.chargeDamageMultiplier = 1f;
            __instance.chargeEvenSpreadTo = 0f;
            __instance.chargeRecoilTo = 0f;
            __instance.chargeSpeedTo = 1f;
            __instance.chargeSpreadTo = 0f;
        }

        [HarmonyPatch(typeof(Gun))]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[] { })]
        [HarmonyPostfix]
        private static void SetOnCreate(Gun __instance)
        {
            __instance.chargeDamageMultiplier = 1f;
            __instance.chargeEvenSpreadTo = 0f;
            __instance.chargeRecoilTo = 0f;
            __instance.chargeSpeedTo = 1f;
            __instance.chargeSpreadTo = 0f;
        }
    }

    [HarmonyPatch(typeof(Gun))]
    class Gun_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Gun.Attack))]
        [HarmonyPriority(Priority.Last)]
        static void ResetCharge(Gun __instance, float charge, bool __result)
        {
            if (__result)
            {
                __instance.currentCharge = 0f;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("ApplyProjectileStats")]
        [HarmonyPriority(Priority.First)]
        static void AdjustBulletSpeed(Gun __instance, GameObject obj)
        {
            if (!__instance.useCharge)
            {
                return;
            }

            ProjectileHit bullet = obj.GetComponent<ProjectileHit>();
            MoveTransform move = obj.GetComponent<MoveTransform>();

            float charge = Extensions.ProjectileHitExtensions.GetAdditionalData(bullet).charge;

            move.localForce *= charge * __instance.chargeSpeedTo;
            bullet.damage *= charge * __instance.chargeDamageMultiplier;
        }

        [HarmonyPatch("usedCooldown", MethodType.Getter)]
        [HarmonyPostfix]
        static void ChargeWeaponCD(Gun __instance, ref float __result)
        {
            if (__instance.useCharge)
            {
                __result = 0.1f;
            }
        }
    }

    [HarmonyPatch]
    class Gun_PatchTranspiler
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
        static CodeInstruction[] NewFireBurstCode = null;

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            OriginalFireBurstCode = codes.ToArray();

            //UnityEngine.Debug.Log("Running Gun transpiler");

            //for (var i = 0; i < codes.Count; i++)
            //{
            //    UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
            //}

            // The original methods
            var getInitMono = AccessTools.Method(typeof(GameObject), nameof(GameObject.GetComponent), generics: new Type[] { typeof(ProjectileInit) });
            CodeInstruction getInit = new CodeInstruction(OpCodes.Callvirt, getInitMono);

            //UnityEngine.Debug.Log("Getting Offline Methods");

            var getOriginalOffline = AccessTools.Method(typeof(ProjectileInit), "OFFLINE_Init", new Type[] { typeof(int), typeof(int), typeof(float), typeof(float) });
            CodeInstruction OriginalOfflineInit = new CodeInstruction(OpCodes.Callvirt, getOriginalOffline);
            var getOriginalOfflineNoAmmo = AccessTools.Method(typeof(ProjectileInit), "OFFLINE_Init_noAmmoUse", new Type[] { typeof(int), typeof(int), typeof(float), typeof(float) });
            CodeInstruction OriginalOfflineInitNoAmmo = new CodeInstruction(OpCodes.Callvirt, getOriginalOfflineNoAmmo);
            var getOriginalOfflineSeparate = AccessTools.Method(typeof(ProjectileInit), "OFFLINE_Init_SeparateGun", new Type[] { typeof(int), typeof(int), typeof(int), typeof(float), typeof(float) });
            CodeInstruction OriginalOfflineInitSeparate = new CodeInstruction(OpCodes.Callvirt, getOriginalOfflineSeparate);

            //UnityEngine.Debug.Log("Getting Player Data");
            var getView = AccessTools.Method(typeof(GameObject), nameof(GameObject.GetComponent), generics: new Type[] { typeof(PhotonView) });
            var getRPC = AccessTools.Method(typeof(PhotonView), nameof(PhotonView.RPC), new Type[] { typeof(string), typeof(RpcTarget), typeof(object[]) });

            // 2 methods of getting the player ID
            var holdable = AccessTools.Field(typeof(Gun), nameof(Gun.holdable));
            var holdableData = AccessTools.Field(typeof(Holdable), nameof(Holdable.holder));
            var holdableDataPlayer = AccessTools.Field(typeof(CharacterData), nameof(CharacterData.player));

            var player = AccessTools.Field(typeof(Gun), nameof(Gun.player));

            var playerID = AccessTools.Field(typeof(Player), nameof(Player.playerID));

            // Get the new mono on the bullet
            var getChargedMono = AccessTools.Method(typeof(GameObject), nameof(GameObject.GetComponent), generics: new Type[] { typeof(ChargedProjectileInit) });
            CodeInstruction getComponent = new CodeInstruction(OpCodes.Callvirt, getChargedMono);

            // Debug log stuff
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

            List<CodeInstruction> playerParam = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldfld, player),
                new CodeInstruction(OpCodes.Ldfld, playerID),
                new CodeInstruction(OpCodes.Box, typeof(int)),
                new CodeInstruction(OpCodes.Stelem_Ref)
            };

            Dictionary<string, string> rpcReplacements = new Dictionary<string, string>() {
                    { "RPCA_Init", nameof(ChargedProjectileInit.RPCA_InitCharge) },
                    { "RPCA_Init_noAmmoUse", nameof(ChargedProjectileInit.RPCA_Init_noAmmoUseCharge) },
                    { "RPCA_Init_SeparateGun", nameof(ChargedProjectileInit.RPCA_Init_SeparateGunCharge) }
                };

            for (int i = 0; i < codes.Count(); i++)
            {
                // Replace Method calls with the new ones
                if ((codes[i].opcode == OpCodes.Callvirt))
                {
                    //UnityEngine.Debug.Log(codes[i]);
                    if (((MethodInfo)codes[i].operand) == getInitMono)
                    {
                        codes[i].operand = getChargedMono;
                    }
                    if (((MethodInfo)codes[i].operand) == getOriginalOffline)
                    {
                        codes[i].operand = getOffline;
                        codes.InsertRange(i, chargeParam);
                        i += chargeParam.Count();
                        //UnityEngine.Debug.Log("OFFLINE_Init swapped out.");
                    }
                    if (((MethodInfo)codes[i].operand) == getOriginalOfflineNoAmmo)
                    {
                        codes[i].operand = getOfflineNoAmmo;
                        codes.InsertRange(i, chargeParam);
                        i += chargeParam.Count();
                        //UnityEngine.Debug.Log("OFFLINE_InitNoAmmo swapped out.");
                    }
                    if (((MethodInfo)codes[i].operand) == getOriginalOfflineSeparate)
                    {
                        codes[i].operand = getOfflineSeparate;
                        codes.InsertRange(i, chargeParam);
                        i += chargeParam.Count();
                        //UnityEngine.Debug.Log("OFFLINE_InitSeparateGun swapped out.");
                    }
                }

                // Replace the RPC calls with the new ones
                if (codes[i].opcode == OpCodes.Ldstr && (rpcReplacements.Keys.Contains(((string)codes[i].operand))))
                {
                    if (!((codes[i-1].opcode == OpCodes.Callvirt) && ((MethodInfo)codes[i-1].operand == getView)))
                    {
                        continue;
                    }
                    codes[i].operand = rpcReplacements[(string)codes[i].operand];

                    while (codes[i].opcode != OpCodes.Newarr)
                    {
                        i++;
                    }
                    int arrSize = i - 1;

                    int arrayElem = 0;
                    while (!((codes[i].opcode == OpCodes.Callvirt) && ((MethodInfo)codes[i].operand == getRPC)))
                    {
                        if (codes[i].opcode == OpCodes.Stelem_Ref)
                        {
                            arrayElem++;
                        }
                        i++;
                    }
                    codes.InsertRange(i, photonChargeParam);
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldc_I4_S, arrayElem));
                    arrayElem++;
                    codes[arrSize] = new CodeInstruction(OpCodes.Ldc_I4_S, arrayElem);
                }
            }

            { // Code for exact replacements

                //{ // All the arrays sent by RPCs need to have their sizes increased by 1
                //    codes[142] = new CodeInstruction(OpCodes.Ldc_I4_5);
                //    codes[200] = new CodeInstruction(OpCodes.Ldc_I4_5);
                //    codes[258] = new CodeInstruction(OpCodes.Ldc_I4_6);
                //}

                //{ // The RPCS need to be calling our function and not the base one.
                //    codes[140] = new CodeInstruction(OpCodes.Ldstr, "RPCA_InitCharge");
                //    codes[198] = new CodeInstruction(OpCodes.Ldstr, "RPCA_Init_noAmmoUseCharge");
                //    codes[256] = new CodeInstruction(OpCodes.Ldstr, "RPCA_Init_SeparateGunCharge");
                //}

                //{ // We need to be getting our component instead of projectileinit
                //    codes[121] = getComponent;
                //    codes[179] = getComponent;
                //    codes[237] = getComponent;
                //}

                //{ // We need to be calling our offline methods
                //    codes[136] = OfflineInit;
                //    codes[194] = OfflineInitNoAmmo;
                //    codes[252] = OfflineInitSeparate;
                //}

                //{ // Start adding the parameters now, done last since it adjusts size.
                //    codes.InsertRange(295, photonChargeParam);
                //    codes.Insert(296, new CodeInstruction(OpCodes.Ldc_I4_5));
                //    codes.InsertRange(252, chargeParam);
                //    codes.InsertRange(232, photonChargeParam);
                //    codes.Insert(233, new CodeInstruction(OpCodes.Ldc_I4_4));
                //    codes.InsertRange(194, chargeParam);
                //    codes.InsertRange(174, photonChargeParam);
                //    codes.Insert(175, new CodeInstruction(OpCodes.Ldc_I4_4));
                //    codes.InsertRange(136, chargeParam);
                //}

                //{ // Insert a debug statement
                //    codes.InsertRange(58, debug);
                //}
            }

            NewFireBurstCode = codes.ToArray();

            //for (int i = 0; i < codes.Count; i++)
            //{
            //    UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
            //    //UnityEngine.Debug.Log(codes[i]);
            //}


            return codes.AsEnumerable();
        }
    }
}