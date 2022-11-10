using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    //Enable to collider on network cars

    [HarmonyPatch(typeof(PlayerDataNet))]


    [HarmonyPatch(typeof(PlayerDataNet), "InitCarVirtual")]
    internal class PlayerDataNet__InitCarVirtual
    {
        [HarmonyPostfix]
        internal static void CollisionPostFix(PlayerDataNet __instance)
        {
            //Used to activate collisions
            System.Collections.IEnumerator ActivateCollidersAfterSeconds(float seconds)
            {
                yield return new UnityEngine.WaitForSeconds(seconds);
                __instance.SetAllColliderLayers(Layers.Default);
            }

            //Activate collisions
            if (Mod.Instance.Config.EnableCollision)
            {
                __instance.StartCoroutine(ActivateCollidersAfterSeconds(10f));
            }

            //Hide player names
            if (Mod.Instance.Config.HidePlayerNames)
                __instance.hoverNameObj_.SetActive(false);

            
        }
    }

    
}
