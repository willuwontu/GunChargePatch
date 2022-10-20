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
    [HarmonyPatch(typeof(CardInfoDisplayer))]
    class CardInfoDisplayer_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(CardInfoDisplayer.DrawCard))]
        static void StopChargeBreak(ref bool charge)
        {
            charge = false;
        }
    }
}