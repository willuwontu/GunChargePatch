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
	[HarmonyPatch(typeof(HealthHandler))]
    class HealthHandler_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(HealthHandler.Revive))]
        static void ResetCharge(CharacterData ___data)
        {
            ___data.weaponHandler.gun.currentCharge = 0f;
        }
    }
}