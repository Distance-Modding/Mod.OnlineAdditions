using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(ClientLogic), "GetLocalChatName")]
    internal class GetLocalChatName__ClientLogic
    {
        [HarmonyPostfix]
        internal static void ModifyLocalPlayerName(ref string __result)
        {
            if (Mod.Instance.Config.EnableCheatsInMultiplayer)
                __result += " (Cheating)";
            if (Mod.Instance.Config.EnableCollision)
                __result += " (Collision)";
        }
    }
}
