using HarmonyLib;

namespace OnlineAdditions.Patches
{
    [HarmonyPatch(typeof(PlayerDataLocal), "Update")]
    internal class PlayerDataLocal__Update
    {
        [HarmonyPostfix]
        internal static void TestLineRenderer(PlayerDataLocal __instance)
        {
            //Line Renderer Code Here
        }
    }
}
