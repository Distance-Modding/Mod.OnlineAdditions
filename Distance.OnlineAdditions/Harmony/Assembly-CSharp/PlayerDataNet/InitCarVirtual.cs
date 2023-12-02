using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    //Enable to collider on network cars
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
                if (!Mod.Instance.PlayerFinished)
                {
                    __instance.SetAllColliderLayers(Layers.Default);
                    __instance.CarLOD_.rigidbody_.isKinematic = true;
                    __instance.CarLOD_.SetCarSimulationEnabled(true);
                }
            }

            //Activate collisions
            if (Mod.Instance.Config.EnableCollision)
            {
                __instance.StartCoroutine(ActivateCollidersAfterSeconds(10f));
            }

            //Hide player names
            if (Mod.Instance.Config.HidePlayerNames)
                __instance.hoverNameObj_.SetActive(false);

            //Disable audio
            if (Mod.Instance.Config.DisableCarAudio)
                __instance.DisableCarSounds(__instance.carObj_);
        }

        
    }

    
}
