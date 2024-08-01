using HarmonyLib;

namespace OnlineAdditions.Patches
{
    //Adds "(Cheating)" and "(Collision)" to your name if you have it enabled
    [HarmonyPatch(typeof(ClientLogic), "GetLocalChatName")]
    internal class ClientLogic__GetLocalChatName
    {
        [HarmonyPostfix]
        internal static void ModifyLocalPlayerName(ref string __result)
        {
            if (Mod.EnableCheats.Value)
                __result += " (Cheating)";
            if (Mod.EnableCollision.Value)
                __result += " (Collision)";
        }
    }
}
