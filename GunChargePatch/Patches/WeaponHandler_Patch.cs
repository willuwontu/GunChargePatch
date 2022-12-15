using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Photon.Pun;
using UnityEngine;
using GunChargePatch.Extensions;
using HarmonyLib;

namespace GunChargePatch.Patches
{
	[HarmonyPatch(typeof(WeaponHandler))]
    class WeaponHandler_Patch
    {
		[HarmonyPatch("Attack")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> PassCharge(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var passCharge = new List<CodeInstruction>();
            //var resetCharge = new List<CodeInstruction>();

            //for (var i = 0; i < codes.Count; i++)
            //{
            //    UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
            //}

            var gun = AccessTools.Field(typeof(WeaponHandler), nameof(WeaponHandler.gun));
            var currentCharge = AccessTools.Field(typeof(Gun), nameof(Gun.currentCharge));

            passCharge.Add(new CodeInstruction(OpCodes.Dup));
            passCharge.Add(new CodeInstruction(OpCodes.Ldfld, currentCharge));
            passCharge.Add(new CodeInstruction(OpCodes.Conv_R4));

            //resetCharge.Add(new CodeInstruction(OpCodes.Ldarg_0));
            //resetCharge.Add(new CodeInstruction(OpCodes.Ldfld, gun));
            //resetCharge.Add(new CodeInstruction(OpCodes.Ldc_R4, 0.0f));
            //resetCharge.Add(new CodeInstruction(OpCodes.Stfld, currentCharge));

            //codes.InsertRange(117, resetCharge);
            codes.RemoveAt(111);
            codes.InsertRange(111, passCharge);

            //UnityEngine.Debug.Log("New code:");

            //for (var i = 0; i < codes.Count; i++)
            //{
            //    UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
            //}

            return codes.AsEnumerable();
        }

        [HarmonyPrefix]
        [HarmonyPatch("Attack")]
        static void DefaultChargingMethod(WeaponHandler __instance, CharacterData ___data)
        {
            if (!__instance)
            {
                return;
            }

            if (!___data)
            {
                return;
            }

            if (!__instance.gun)
            {
                return;
            }

            if (!__instance.gun.useCharge)
            {
                return;
            }

            if (___data.input.shootIsPressed && !___data.dead && (bool)(typeof(PlayerVelocity).GetField("simulated", BindingFlags.Instance | BindingFlags.GetField |
                        BindingFlags.NonPublic).GetValue(___data.playerVel)) && (0 < (int)typeof(GunAmmo).GetField("currentAmmo", BindingFlags.Instance | BindingFlags.GetField |
                        BindingFlags.NonPublic).GetValue(__instance.gun.GetComponentInChildren<GunAmmo>())))
            {
                __instance.gun.currentCharge = Mathf.Clamp(__instance.gun.currentCharge + ((TimeHandler.deltaTime / __instance.gun.GetAdditionalData().chargeTime) * __instance.gun.GetAdditionalData().maxCharge), 0f, __instance.gun.GetAdditionalData().maxCharge);
            }
        }
    }
}