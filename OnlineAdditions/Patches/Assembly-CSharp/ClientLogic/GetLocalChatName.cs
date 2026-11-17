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
            {
                string cheatString = "";
                if (G.Sys.CheatsManager_.IsEnabled(ECheat.MonsterTruck)) cheatString += " (Monster Truck)";
                if (G.Sys.CheatsManager_.IsEnabled(ECheat.SkyWalker)) cheatString += " (Sky Walker)";
                if (G.Sys.CheatsManager_.IsEnabled(ECheat.SuperBoost)) cheatString += " (Super Boost)";
                if (G.Sys.CheatsManager_.IsEnabled(ECheat.DeathProof)) cheatString += " (Death Proof)";
                if (G.Sys.CheatsManager_.IsEnabled(ECheat.PreservationalBoosterSloth)) cheatString += " (Sloth)";
                

                __result += cheatString;
            }
            if (Mod.EnableCollision.Value)
                __result += " (Collision)";
        }
    }
}
