using HarmonyLib;

namespace OnlineAdditions.Patches
{
    //Adds "(Cheating)" or "(Collision)" to your chat name if enabled.
    [HarmonyPatch(typeof(ClientLogic), "GetClientChatName")]
    internal class ClientLogic__GetClientChatName
    {
        [HarmonyPostfix]
        internal static void ModifyLocalPlayerName(ClientLogic __instance, ref string __result)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"(\[)");
            string[] split_result = regex.Split(__result);
            string cheating = string.Empty;
            string collision = string.Empty;
            if (Mod.EnableCheats.Value)
                cheating = " (Cheating)";
            if (Mod.EnableCollision.Value)
                collision += " (Collision)";

            int num1 = split_result[2].IndexOf("]");
            Mod.Instance.playerName = split_result[2].Substring(num1 + 1, split_result[2].Length - num1 - 1) + cheating + collision;

            __result = split_result[1] + split_result[2] + cheating + collision + split_result[3] + split_result[4];
        }
    }
}
