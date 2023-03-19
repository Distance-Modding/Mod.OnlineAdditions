/*using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(GameManager), "GoToCurrentLevel", new System.Type[] { typeof(GameManager.OpenOnMainMenuInit), typeof(bool) })]
    internal class GameManager__GoToCurrentLevel
    {
        [HarmonyPrefix]
        internal static bool OnlineRestartCheck(GameManager __instance, GameManager.OpenOnMainMenuInit openInMainMenu, bool advancedMenu)
        {
            if(Mod.Instance.Restarting)
            {
                G.Sys.LevelEditor_.RestartLevel();
                Mod.Instance.Restarting = false;
                return false;
            }
            return true;
        }
    }
}*/
