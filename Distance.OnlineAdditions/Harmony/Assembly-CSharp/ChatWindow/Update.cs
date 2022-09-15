using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(ChatWindow), "Update")]
    internal class Update__ChatWindow
    {
        [HarmonyPostfix]
        internal static void HideChatPost(ChatWindow __instance)
        {
            if(Mod.Instance.Config.HideChat)
            {
                __instance.panel_.alpha = 0f;
               // __instance.SetTextListLineCount(0);
            }
        }
    }
}
