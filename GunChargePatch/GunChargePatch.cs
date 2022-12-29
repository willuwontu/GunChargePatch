using BepInEx;
using HarmonyLib;
using UnityEngine;
using Photon.Pun;
using GunChargePatch.Extensions;
using GunChargePatch.Extensions.Copied;

namespace GunChargePatch
{
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("io.olavim.rounds.rwf", BepInDependency.DependencyFlags.SoftDependency)]
	// Declares our mod to Bepin
	[BepInPlugin(ModId, ModName, Version)]
    // The game our mod is associated with
    [BepInProcess("Rounds.exe")]
    public class GunChargePatch : BaseUnityPlugin
    {
        private const string ModId = "com.rounds.willuwontu.gunchargepatch";
        private const string ModName = "GunChargePatch";
        public const string Version = "0.0.3"; // What version are we on (major.minor.patch)?

        public static GunChargePatch instance { get; private set; }

        void Awake()
        {
            instance = this;

            // Use this to call any harmony patch files your mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();

			//foreach (GameObject obj in Resources.LoadAll<GameObject>(""))
			//{
			//	if (obj.GetComponent<ProjectileInit>())
			//	{
			//		if (!obj.GetComponent<ChargedProjectileInit>())
			//		{
   //                     obj.AddComponent<ChargedProjectileInit>();
   //                 }
   //             }
			//}
        }
        void Start()
        {
            foreach (GameObject obj in Resources.LoadAll<GameObject>(""))
            {
                if (obj.GetComponent<ProjectileInit>())
                {
                    if (!obj.GetComponent<ChargedProjectileInit>())
                    {
                        obj.AddComponent<ChargedProjectileInit>();
                    }
                }
            }
        }
    }

	public class ChargedProjectileInit : MonoBehaviour
	{
		[PunRPC]
		internal void RPCA_InitCharge(int senderID, int nrOfProj, float dmgM, float randomSeed, float charge)
		{
			//UnityEngine.Debug.Log(string.Format("Charge of {0:F2} passed on to bullet.", charge));
			this.gameObject.GetComponent<ProjectileHit>().GetAdditionalData().charge = charge;
			//UnityEngine.Debug.Log($"SenderID: {senderID}");
			//UnityEngine.Debug.Log($"nrOfProj: {nrOfProj}");
			//UnityEngine.Debug.Log($"dmgM: {dmgM}");
			//UnityEngine.Debug.Log($"randomSeed: {randomSeed}");
			//UnityEngine.Debug.Log($"Charge: {randomSeed}");
			//UnityEngine.Debug.Log($"{this.gameObject.GetComponent<ProjectileInit>()}");
			//UnityEngine.Debug.Log($"Owner ID: {this.gameObject.GetComponent<PhotonView>().OwnerActorNr}");
			//if (this.gameObject.GetComponent<PhotonView>().IsMine)
   //         {
			//	this.gameObject.GetComponent<PhotonView>().RPC("RPCA_Init", RpcTarget.All, new object[] { senderID, nrOfProj, dmgM, randomSeed });

			//}
			//typeof(ProjectileInit).GetMethod("RPCA_Init", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Instance).Invoke(this.gameObject.GetComponent<ProjectileInit>(), new object[] { senderID, nrOfProj, dmgM, randomSeed });
			this.gameObject.GetComponent<ProjectileInit>().InvokeMethod("RPCA_Init", new object[] { senderID, nrOfProj, dmgM, randomSeed });
		}

		internal void OFFLINE_InitCharge(int senderID, int nrOfProj, float dmgM, float randomSeed, float charge)
		{
			//UnityEngine.Debug.Log(string.Format("Charge of {0:F2} passed on to bullet.", charge));
			this.gameObject.GetComponent<ProjectileHit>().GetAdditionalData().charge = charge;
			//UnityEngine.Debug.Log($"SenderID: {senderID}");
			//UnityEngine.Debug.Log($"nrOfProj: {nrOfProj}");
			//UnityEngine.Debug.Log($"dmgM: {dmgM}");
			//UnityEngine.Debug.Log($"randomSeed: {randomSeed}");
			//UnityEngine.Debug.Log($"Charge: {randomSeed}");
			//UnityEngine.Debug.Log($"{this.gameObject.GetComponent<ProjectileInit>()}");
			this.gameObject.GetComponent<ProjectileInit>().InvokeMethod("OFFLINE_Init", new object[] { senderID, nrOfProj, dmgM, randomSeed });
		}

		[PunRPC]
		internal void RPCA_Init_SeparateGunCharge(int senderID, int gunID, int nrOfProj, float dmgM, float randomSeed, float charge)
		{
			//UnityEngine.Debug.Log(string.Format("Charge of {0:F2} passed on to bullet.", charge));
			this.gameObject.GetComponent<ProjectileHit>().GetAdditionalData().charge = charge;
			//UnityEngine.Debug.Log($"SenderID: {senderID}");
			//UnityEngine.Debug.Log($"GunID: {gunID}");
			//UnityEngine.Debug.Log($"nrOfProj: {nrOfProj}");
			//UnityEngine.Debug.Log($"dmgM: {dmgM}");
			//UnityEngine.Debug.Log($"randomSeed: {randomSeed}");
			//UnityEngine.Debug.Log($"Charge: {randomSeed}");
			//UnityEngine.Debug.Log($"{this.gameObject.GetComponent<ProjectileInit>()}");
			this.gameObject.GetComponent<ProjectileInit>().InvokeMethod("RPCA_Init_SeparateGun", new object[] { senderID, gunID, nrOfProj, dmgM, randomSeed });
		}

		internal void OFFLINE_Init_SeparateGunCharge(int senderID, int gunID, int nrOfProj, float dmgM, float randomSeed, float charge)
		{
			//UnityEngine.Debug.Log(string.Format("Charge of {0:F2} passed on to bullet.", charge));
			this.gameObject.GetComponent<ProjectileHit>().GetAdditionalData().charge = charge;
			//UnityEngine.Debug.Log($"SenderID: {senderID}");
			//UnityEngine.Debug.Log($"GunID: {gunID}");
			//UnityEngine.Debug.Log($"nrOfProj: {nrOfProj}");
			//UnityEngine.Debug.Log($"dmgM: {dmgM}");
			//UnityEngine.Debug.Log($"randomSeed: {randomSeed}");
			//UnityEngine.Debug.Log($"Charge: {randomSeed}");
			//UnityEngine.Debug.Log($"{this.gameObject.GetComponent<ProjectileInit>()}");
			this.gameObject.GetComponent<ProjectileInit>().InvokeMethod("OFFLINE_Init_SeparateGun", new object[] { senderID, gunID, nrOfProj, dmgM, randomSeed });
		}

		private Gun GetChildGunWithID(int id, GameObject player)
		{
			if (this.guns == null)
			{
				this.guns = player.GetComponentsInChildren<Gun>();
			}
			return this.guns[id];
		}

		private static Player GetPlayerWithActorAndPlayerIDs(int actorID, int playerID)
		{
			Player res = null;
			foreach (Player player in PlayerManager.instance.players)
			{
				if (player.data.view.ControllerActorNr == actorID && player.playerID == playerID) { res = player; break; }
			}
			return res;
		}

		[PunRPC]
		internal void RPCA_Init_noAmmoUseCharge(int senderID, int nrOfProj, float dmgM, float randomSeed, float charge)
		{
			//UnityEngine.Debug.Log(string.Format("Charge of {0:F2} passed on to bullet.", charge));
			this.gameObject.GetComponent<ProjectileHit>().GetAdditionalData().charge = charge;
			//UnityEngine.Debug.Log($"SenderID: {senderID}");
			//UnityEngine.Debug.Log($"nrOfProj: {nrOfProj}");
			//UnityEngine.Debug.Log($"dmgM: {dmgM}");
			//UnityEngine.Debug.Log($"randomSeed: {randomSeed}");
			//UnityEngine.Debug.Log($"Charge: {randomSeed}");
			//UnityEngine.Debug.Log($"{this.gameObject.GetComponent<ProjectileInit>()}");
			this.gameObject.GetComponent<ProjectileInit>().InvokeMethod("RPCA_Init_noAmmoUse", new object[] { senderID, nrOfProj, dmgM, randomSeed });
		}

		internal void OFFLINE_Init_noAmmoUseCharge(int senderID, int nrOfProj, float dmgM, float randomSeed, float charge)
		{
			//UnityEngine.Debug.Log(string.Format("Charge of {0:F2} passed on to bullet.", charge));
			this.gameObject.GetComponent<ProjectileHit>().GetAdditionalData().charge = charge;
			//UnityEngine.Debug.Log($"SenderID: {senderID}");
			//UnityEngine.Debug.Log($"nrOfProj: {nrOfProj}");
			//UnityEngine.Debug.Log($"dmgM: {dmgM}");
			//UnityEngine.Debug.Log($"randomSeed: {randomSeed}");
			//UnityEngine.Debug.Log($"Charge: {randomSeed}");
			//UnityEngine.Debug.Log($"{this.gameObject.GetComponent<ProjectileInit>()}");
			this.gameObject.GetComponent<ProjectileInit>().InvokeMethod("OFFLINE_Init_noAmmoUse", new object[] { senderID, nrOfProj, dmgM, randomSeed });
		}

		[PunRPC]
		public void RPCA_SetBulletCharge(float charge)
		{
            this.gameObject.GetComponent<ProjectileHit>().GetAdditionalData().charge = charge;
        }

		private Gun[] guns;
	}
}
