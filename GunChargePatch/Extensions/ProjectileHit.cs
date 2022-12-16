using HarmonyLib;
using System.Runtime.CompilerServices;

namespace GunChargePatch.Extensions
{
    public class ProjectileHitAdditionalData
    {
        public float charge = 0f;
    }
    public static class ProjectileHitExtensions
    {
        private static readonly ConditionalWeakTable<ProjectileHit, ProjectileHitAdditionalData> additionalData = new ConditionalWeakTable<ProjectileHit, ProjectileHitAdditionalData>();
        internal static ProjectileHitAdditionalData GetAdditionalData(this ProjectileHit instance)
        {
            return additionalData.GetOrCreateValue(instance);
        }

        public static float GetBulletCharge(this ProjectileHit instance)
        {
            return instance.GetAdditionalData().charge;
        }
    }
}