using HarmonyLib;

namespace OnlineAdditions.Patches
{
    //Hides chat. (Chat won't reappear, needs reappearing energy)
    [HarmonyPatch(typeof(ChatWindow), "Update")]
    internal class Update__ChatWindow
    {
        [HarmonyPostfix]
        internal static void HideChatPost(ChatWindow __instance)
        {
            if (Mod.HideChat.Value)
            {
                __instance.panel_.alpha = 0f;
                // __instance.SetTextListLineCount(0);
            }
        }
    }
}
