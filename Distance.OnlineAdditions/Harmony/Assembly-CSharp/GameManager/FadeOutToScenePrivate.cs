/*using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(GameManager), "FadeOutToScenePrivate")]
    internal class FadeOutToScenePrivate
    {
        [HarmonyPrefix]
        internal static void Restarting()
        {
            if(Mod.Instance.Restarting)
            {
                Mod.Instance.Restarting = false;
            }
        }
    }
}*/
