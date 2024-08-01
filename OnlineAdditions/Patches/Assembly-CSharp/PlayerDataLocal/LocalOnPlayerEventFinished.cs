using HarmonyLib;

namespace OnlineAdditions.Patches
{
    //Updating a boolean (important)
    [HarmonyPatch(typeof(PlayerDataLocal), "LocalOnPlayerEventFinished")]
    internal class PlayerDataLocal__LocalOnPlayerEventFinished
    {
        [HarmonyPostfix]
        internal static void UpdatePlayerFinishedState()
        {
            Mod.Instance.playerFinished = true;
        }
    }
}
