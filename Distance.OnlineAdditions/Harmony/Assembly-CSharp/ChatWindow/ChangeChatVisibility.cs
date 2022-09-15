using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    //IT DOESN'T PATCH!!!?!??!?!
    [HarmonyPatch(typeof(ChatWindow), "ChangeChatVisibility")]
    internal class ChangeChatVisibility__ChatWindow
    {
        [HarmonyPrefix]
        internal static bool HideChatPrefix()
        {
            Mod.Instance.Logger.Debug("DIE IDIOT");
            if (Mod.Instance.Config.HideChat)
                return false;
            else
                return true;
        }
    }
}
