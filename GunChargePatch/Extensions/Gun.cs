using HarmonyLib;
using System.Runtime.CompilerServices;

namespace GunChargePatch.Extensions
{
    public class GunAdditionalData
    {
        public float maxCharge = 1f;
        public float chargeTime = 1f;
        public bool useDefaultChargingMethod = true;
        public GunAdditionalData()
        {
            maxCharge = 1f;
            chargeTime = 1f;
            useDefaultChargingMethod = true;
        }
    }

    public static class GunExtensions
    {
        private static readonly ConditionalWeakTable<Gun, GunAdditionalData> additionalData = new ConditionalWeakTable<Gun, GunAdditionalData>();
        public static GunAdditionalData GetAdditionalData(this Gun instance)
        {
            return additionalData.GetOrCreateValue(instance);
        }
    }

    [HarmonyPatch(typeof(Gun), "ResetStats")]
    class GunPatchResetStats
    {
        private static void Prefix(Gun __instance)
        {
            __instance.GetAdditionalData().maxCharge = 1f;
            __instance.GetAdditionalData().chargeTime = 1f;
            __instance.GetAdditionalData().useDefaultChargingMethod = true;
        }
    }
}