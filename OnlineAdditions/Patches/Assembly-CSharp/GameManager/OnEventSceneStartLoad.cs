using HarmonyLib;

namespace OnlineAdditions.Patches
{
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.OnEventSceneStartLoad))]
    internal static class GameManager__OnEventSceneStartLoad
    {
        [HarmonyPostfix]
        internal static void SettingPlayerFinished()
        {
            //Mod.Log.LogInfo("Starting to load a scene!");
            Mod.Instance.playerFinished = false;
        }
    }
}
