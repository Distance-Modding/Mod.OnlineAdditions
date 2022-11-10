using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(ClientLogic), "GetClientChatName")]
    internal class GetClientChatName
    {
        [HarmonyPostfix]
        internal static void ModifyLocalPlayerName(ClientLogic __instance, ref string __result)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"(\[)");
            string[] split_result = regex.Split(__result);
            string cheating = string.Empty;
            string collision = string.Empty;
            if (Mod.Instance.Config.EnableCheatsInMultiplayer)
                cheating = " (Cheating)";
            if (Mod.Instance.Config.EnableCollision)
                collision += " (Collision)";
            __result = split_result[1] + split_result[2] + cheating + collision + split_result[3] + split_result[4];
        }
    }
}
