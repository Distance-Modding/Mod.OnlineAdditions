using HarmonyLib;
using UnityEngine.Events;

namespace OnlineAdditions.Patches
{
    //Grabbing variables.
    [HarmonyPatch(typeof(PlayerDataLocal), "InitCarVirtual", new System.Type[] { typeof(bool) })]
    internal class PlayerDataLocal__InitCarVirtual
    {

        [HarmonyPostfix]
        internal static void GetCar(PlayerDataLocal __instance, bool fastRespawn)
        {
            Mod.Instance.playerCar = __instance.carObj_;
            Mod.Instance.playerFinished = false;
            //Need to figure out a check for the whether or not the level actually started because collisions start way too early
            if (Mod.EnableCollision.Value && fastRespawn)
                __instance.StartCoroutine(Mod.Instance.ActivateCollidersAfterSeconds(10f));
        }
    }
}
