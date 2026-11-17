using HarmonyLib;
using System.Text.RegularExpressions;

namespace OnlineAdditions.Patches
{
    //Adds "(Cheating)" or "(Collision)" to your chat name if enabled.
    [HarmonyPatch(typeof(ClientLogic), "GetClientChatName")]
    internal class ClientLogic__GetClientChatName
    {
        [HarmonyPostfix]
        internal static void ModifyLocalPlayerName(ClientLogic __instance, ref string __result)
        {
            Mod.Log.LogInfo("Client Chat Name: " + __result);
            Regex regex = new Regex($@"({__instance.localPlayerInfos_[0].Username_})");
            string[] split_result = regex.Split(__result, 2);
            foreach(string s in split_result)
            {
                Mod.Log.LogInfo("Split: " + s);
            }
            string cheating = string.Empty;
            string collision = string.Empty;
            if (Mod.EnableCheats.Value)
            {
                string cheatString = "";
                if (G.Sys.CheatsManager_.IsEnabled(ECheat.MonsterTruck)) cheatString += " (Monster Truck)";
                if (G.Sys.CheatsManager_.IsEnabled(ECheat.SkyWalker)) cheatString += " (Sky Walker)";
                if (G.Sys.CheatsManager_.IsEnabled(ECheat.SuperBoost)) cheatString += " (Super Boost)";
                if (G.Sys.CheatsManager_.IsEnabled(ECheat.DeathProof)) cheatString += " (Death Proof)";
                if (G.Sys.CheatsManager_.IsEnabled(ECheat.PreservationalBoosterSloth)) cheatString += " (Sloth)";


                cheating = cheatString;
            }
            if (Mod.EnableCollision.Value)
                collision = " (Collision)";

            Mod.Instance.playerName = __instance.localPlayerInfos_[0].Username_ + cheating + collision;

            __result = split_result[0] + split_result[1] + cheating + collision + split_result[2];//*/
        }
    }
}
