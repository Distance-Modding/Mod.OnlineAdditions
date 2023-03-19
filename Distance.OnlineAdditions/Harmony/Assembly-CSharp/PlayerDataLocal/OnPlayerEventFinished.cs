using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(PlayerDataLocal), "OnPlayerEventFinished")]
    internal class PlayerDataLocal__OnPlayerEventFinished
    {
        [HarmonyPostfix]
        internal static void UpdatePlayerFinishedState(PlayerDataLocal __instance)
        {
            Mod.Instance.playerFinishType = __instance.finishType_;
        }
    }
}
