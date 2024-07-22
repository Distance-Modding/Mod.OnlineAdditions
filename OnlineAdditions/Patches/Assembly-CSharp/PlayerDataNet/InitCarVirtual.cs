using HarmonyLib;

namespace OnlineAdditions.Patches
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
                if (!Mod.Instance.playerFinished)
                {
                    __instance.SetAllColliderLayers(Layers.OnlyPlayer1);
                    __instance.CarLOD_.rigidbody_.isKinematic = true;
                    __instance.CarLOD_.SetCarSimulationEnabled(true);
                }
            }

            //Activate collisions
            if (Mod.EnableCollision.Value)
            {
                __instance.StartCoroutine(ActivateCollidersAfterSeconds(10f));
            }

            //Hide player names
            if (Mod.HidePlayerNames.Value)
                __instance.hoverNameObj_.SetActive(false);

            //Disable audio
            if (Mod.DisableCarAudio.Value)
                __instance.DisableCarSounds(__instance.carObj_);
        }


    }
}
