using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(PlayerDataLocal), "LocalOnPlayerEventFinished")]
    internal class PlayerDataLocal__LocalOnPlayerEventFinished
    {
        [HarmonyPostfix]
        internal static void UpdatePlayerFinishedState()
        {
            Mod.Instance.PlayerFinished = true;
        }
    }
}
