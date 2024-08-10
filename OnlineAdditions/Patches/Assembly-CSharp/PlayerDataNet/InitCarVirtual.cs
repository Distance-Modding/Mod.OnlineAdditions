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
            //Activate collisions
            if (Mod.EnableCollision.Value)
            {
                LowerImpactDeath lowerImpactDeath = __instance.carObj_.GetOrAddComponent<LowerImpactDeath>();
                lowerImpactDeath.deathThresholdMultipler_ = 2.25f;
                __instance.StartCoroutine(Mod.Instance.ActivateCollidersAfterSeconds(10f, __instance));
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
