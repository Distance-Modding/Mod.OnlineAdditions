using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(GameMode), "UploadScoreAndReplay")]
    internal class GameMode__UploadScoreAndReplay
    {
        [HarmonyPrefix]
        internal static bool DoISkipMethod()
        {
            //False skips the method, true does not.
            if (!Mod.Instance.UploadScore)
            {
                Mod.Instance.Logger.Debug("Skipping leaderboard upload because online collisions or cheats were enabled");

                //If collisions aren't actually on right now then set upload score to true now
                //This will prevent the situation where someone disables multiplayer collisions right before they finish
                if (!Mod.Instance.Config.EnableCollision && !Mod.Instance.Config.EnableCheatsInMultiplayer)
                    Mod.Instance.UploadScore = true;

                return false;
            }
            return true;
        }
    }
}
