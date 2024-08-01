using HarmonyLib;

namespace OnlineAdditions.Patches
{
    //Enable all changes need on network cars
    [HarmonyPatch(typeof(PlayerDataNet), "InitCarVirtual")]
    internal class PlayerDataNet__InitCarVirtual
    {
        [HarmonyPostfix]
        internal static void CollisionPostFix(PlayerDataNet __instance)
        {
            //Used to activate collisions (Right now, this only affects Network cars and not The local. The problem is resetting does not actually protect you from spawn kills)
            System.Collections.IEnumerator ActivateCollidersAfterSeconds(float seconds)
            {
                yield return new UnityEngine.WaitForSeconds(seconds);
                if (!Mod.Instance.playerFinished)
                {
                    __instance.SetAllColliderLayers(Layers.Player2);
                    __instance.CarLOD_.rigidbody_.isKinematic = true;
                    __instance.CarLOD_.SetCarSimulationEnabled(true);
                }
            }

            //Activate collisions
            if (Mod.EnableCollision.Value)
            {
                LowerImpactDeath lowerImpactDeath = __instance.carObj_.GetOrAddComponent<LowerImpactDeath>();
                lowerImpactDeath.deathThresholdMultipler_ = 2.5f;
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
