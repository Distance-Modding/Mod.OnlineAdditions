using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(CheatsManager), "OnEventSceneLoadFinished")]
    internal class OnEventSceneLoadFinished
    {
        [HarmonyPrefix]
        internal static bool KeepCheatsPrefix(CheatsManager __instance)
        {
            if (Mod.Instance.Config.EnableCheatsInMultiplayer)
            {
                __instance.gameplayCheatsRecognized_ = true;
            }
            else
                __instance.gameplayCheatsRecognized_ = !G.Sys.NetworkingManager_.IsOnline_;

            __instance.UpdateEnabledFlags();
            __instance.anyGameplayCheatsUsedThisLevel_ = __instance.AnyGameplayCheatsCurrentlyUsed_;

            // always skip the method so this is the replacement method
            return false;
        }
    }
}