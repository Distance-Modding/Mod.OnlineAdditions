using HarmonyLib;

namespace OnlineAdditions.Patches
{
    [HarmonyPatch(typeof(GameMode), "UploadScoreAndReplay")]
    internal class GameMode__UploadScoreAndReplay
    {
        [HarmonyPrefix]
        internal static bool DoISkipMethod()
        {
            if (!Mod.Instance.uploadScore && G.Sys.NetworkingManager_.IsOnline_)
            {
                Mod.Log.LogInfo("Skipping leaderboard upload because online collisions, cheats, or events were enabled");

                //If collisions aren't actually on right now then set upload score to true now
                //This will prevent the situation where someone disables multiplayer collisions right before they finish and gets their score uploaded.
                if (!Mod.EnableCollision.Value && !Mod.EnableCheats.Value && !Mod.EnableOnlineEvents.Value)
                    Mod.Instance.uploadScore = true;

                return false;
            }
            return true;
        }
    }
}
