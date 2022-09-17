using HarmonyLib;
using System;

namespace Distance.OnlineAdditions.Harmony
{
    //Enable to collider on network cars
    [HarmonyPatch(typeof(PlayerDataNet), "InitCarVirtual")]
    internal class PlayerDataNet__InitCarVirtual
    {
        [HarmonyPostfix]
        internal static void CollisionPostFix(PlayerDataNet __instance)
        {
            //Activate collisions
            if (Mod.Instance.Config.EnableCollision)
            {
                __instance.SetAllColliderLayers(Layers.Default);
            }

            //Hide player names
            if (Mod.Instance.Config.HidePlayerNames)
                __instance.hoverNameObj_.SetActive(false);
        }
    }
}
