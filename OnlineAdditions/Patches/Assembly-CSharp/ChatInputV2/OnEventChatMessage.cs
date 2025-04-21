using Events.ChatLog;
using HarmonyLib;

namespace OnlineAdditions.Patches
{
    [HarmonyPatch(typeof(ChatInputV2), "OnEventChatMessage", new System.Type[] { typeof(AddMessage.Data) })]
    internal class ChatInputV2__OnEventChatMessage
    {
        [HarmonyPostfix]
        internal static void CheckingForServerCode(ChatInputV2 __instance, AddMessage.Data data)
        {
            if (Mod.Instance.amIHost)
            {
                string messageText = "";
                string messageFrom = "";
                System.Text.RegularExpressions.Regex regexMessage = new System.Text.RegularExpressions.Regex(@"(]: )(?!.*(]: )).*");
                System.Text.RegularExpressions.Regex regexName = new System.Text.RegularExpressions.Regex(@"(?<=\]).*?(?=\[)");
                if (regexName.IsMatch(data.message_) && regexMessage.IsMatch(data.message_))
                {
                    messageFrom = regexName.Matches(data.message_)[0].Value;
                    if (messageFrom != Mod.Instance.playerName)
                    {
                        messageText = regexMessage.Matches(data.message_)[0].Value;
                        int num1 = messageText.IndexOf(" ");
                        string key = messageText.Substring(num1 + 1, messageText.Length - (num1 + 1));
                        //Mod.Log.LogInfo(messageText);
                        //Mod.Log.LogInfo(key);
                        __instance.CheckForServerCode(key);
                    }
                }
            }
        }
    }
}
