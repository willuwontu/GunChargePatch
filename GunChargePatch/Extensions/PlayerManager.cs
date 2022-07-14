using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GunChargePatch.Extensions
{
    public static class PlayerManagerExtension
    {
        public static Player GetPlayerWithID(this PlayerManager playerManager, int playerID)
        {
            return (Player)typeof(PlayerManager).InvokeMember("GetPlayerWithID",
                BindingFlags.Instance | BindingFlags.InvokeMethod |
                BindingFlags.NonPublic, null, playerManager, new object[] { playerID });
        }
        public static Player GetPlayerWithActorID(this PlayerManager playerManager, int actorID)
        {
            return (Player)typeof(PlayerManager).InvokeMember("GetPlayerWithActorID",
                BindingFlags.Instance | BindingFlags.InvokeMethod |
                BindingFlags.NonPublic, null, playerManager, new object[] { actorID });
        }
    }
}