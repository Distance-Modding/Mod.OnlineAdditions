/*using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    //IT DOESN'T PATCH!!!?!??!?!
    [HarmonyPatch(typeof(ChatWindow), "Update")]
    internal class Update__ChatWindow
    {
        [HarmonyPostfix]
        internal static void HideChatPost(ChatWindow _instance)
        {
            if(Mod.Instance.Config.HideChat)
            {
                _instance.panel_.alpha = 0f;
                Mod.Instance.Logger.Debug("LMAOOO");
               // _instance.SetTextListLineCount(0);
            }
        }
    }
}*/
