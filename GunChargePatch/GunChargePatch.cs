using BepInEx;
using HarmonyLib;
using UnityEngine;
using Photon.Pun;
using GunChargePatch.Extensions;

namespace GunChargePatch
{
    // Declares our mod to Bepin
    [BepInPlugin(ModId, ModName, Version)]
    // The game our mod is associated with
    [BepInProcess("Rounds.exe")]
    public class GunChargePatch : BaseUnityPlugin
    {
        private const string ModId = "com.rounds.willuwontu.gunchargepatch";
        private const string ModName = "GunChargePatch";
        public const string Version = "0.0.0"; // What version are we on (major.minor.patch)?

        public static GunChargePatch instance { get; private set; }

        void Awake()
        {
            instance = this;

            // Use this to call any harmony patch files your mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }
        void Start()
        {
            GameObject bullet = Resources.Load<GameObject>("Bullet_Base");
            bullet.AddComponent<ChargedProjectileInit>();
        }
    }

	public class ChargedProjectileInit : MonoBehaviour
	{
		[PunRPC]
		internal void RPCA_InitCharge(int senderID, int nrOfProj, float dmgM, float randomSeed, float charge)
		{
			UnityEngine.Debug.Log(string.Format("Charge of {0:F2} passed on to bullet.", charge));
			this.gameObject.GetComponent<ProjectileHit>().GetAdditionalData().charge = charge;
			PlayerManager.instance.GetPlayerWithActorID(senderID).data.weaponHandler.gun.BulletInit(this.gameObject, nrOfProj, dmgM, randomSeed, true);
		}

		internal void OFFLINE_InitCharge(int senderID, int nrOfProj, float dmgM, float randomSeed, float charge)
		{
			UnityEngine.Debug.Log(string.Format("Charge of {0:F2} passed on to bullet.", charge));
			this.gameObject.GetComponent<ProjectileHit>().GetAdditionalData().charge = charge;
			PlayerManager.instance.players[senderID].data.weaponHandler.gun.BulletInit(this.gameObject, nrOfProj, dmgM, randomSeed, true);
		}

		[PunRPC]
		internal void RPCA_Init_SeparateGunCharge(int senderID, int gunID, int nrOfProj, float dmgM, float randomSeed, float charge)
		{
			UnityEngine.Debug.Log(string.Format("Charge of {0:F2} passed on to bullet.", charge));
			this.gameObject.GetComponent<ProjectileHit>().GetAdditionalData().charge = charge;
			this.GetChildGunWithID(gunID, PlayerManager.instance.GetPlayerWithActorID(senderID).gameObject).BulletInit(this.gameObject, nrOfProj, dmgM, randomSeed, true);
		}

		internal void OFFLINE_Init_SeparateGunCharge(int senderID, int gunID, int nrOfProj, float dmgM, float randomSeed, float charge)
		{
			UnityEngine.Debug.Log(string.Format("Charge of {0:F2} passed on to bullet.", charge));
			this.gameObject.GetComponent<ProjectileHit>().GetAdditionalData().charge = charge;
			this.GetChildGunWithID(gunID, PlayerManager.instance.players[senderID].gameObject).BulletInit(this.gameObject, nrOfProj, dmgM, randomSeed, true);
		}

		private Gun GetChildGunWithID(int id, GameObject player)
		{
			if (this.guns == null)
			{
				this.guns = player.GetComponentsInChildren<Gun>();
			}
			return this.guns[id];
		}

		[PunRPC]
		internal void RPCA_Init_noAmmoUseCharge(int senderID, int nrOfProj, float dmgM, float randomSeed, float charge)
		{
			UnityEngine.Debug.Log(string.Format("Charge of {0:F2} passed on to bullet.", charge));
			this.gameObject.GetComponent<ProjectileHit>().GetAdditionalData().charge = charge;
			PlayerManager.instance.GetPlayerWithActorID(senderID).data.weaponHandler.gun.BulletInit(this.gameObject, nrOfProj, dmgM, randomSeed, false);
		}

		internal void OFFLINE_Init_noAmmoUseCharge(int senderID, int nrOfProj, float dmgM, float randomSeed, float charge)
		{
			UnityEngine.Debug.Log(string.Format("Charge of {0:F2} passed on to bullet.", charge));
			this.gameObject.GetComponent<ProjectileHit>().GetAdditionalData().charge = charge;
			PlayerManager.instance.players[senderID].data.weaponHandler.gun.BulletInit(this.gameObject, nrOfProj, dmgM, randomSeed, false);
		}

		private Gun[] guns;
	}
}
