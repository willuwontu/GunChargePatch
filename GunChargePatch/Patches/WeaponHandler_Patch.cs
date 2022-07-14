using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Photon.Pun;
using UnityEngine;
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
    }
}