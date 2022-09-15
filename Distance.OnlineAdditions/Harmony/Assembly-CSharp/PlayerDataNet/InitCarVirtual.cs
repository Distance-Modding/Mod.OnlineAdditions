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
                try
                {
                    __instance.Car_.SetLayerRecursively(Layers.CollidesWithCars);
                }
                catch (Exception e)
                {
                    Mod.Instance.Logger.Debug(e);
                    Mod.Instance.Logger.Debug("Failed to set layers to collides with cars");
                }
            }

            //Hide player names
            if (Mod.Instance.Config.HidePlayerNames)
                __instance.hoverNameObj_.SetActive(false);
        }
    }
}
