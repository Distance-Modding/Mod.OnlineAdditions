using Events.RaceMode;
using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(ChatInputV2), "CheckForServerCode", new System.Type[] {typeof(string) })]
    internal class ChatInputV2__CheckForServerCode
    {
        [HarmonyPrefix]
        internal static bool CheckForCommands(ChatInputV2 __instance, string text)
        {
            System.Collections.IEnumerator RestartPlayerAfter(float time)
            {
                G.Sys.GameManager_.Mode_.FinishAllLocalPlayers(FinishType.Spectate);
                /*if (Mod.Instance.playerCar != null)
                {
                    Mod.Instance.playerCar.Destroy();
                }*/
                yield return new UnityEngine.WaitForSeconds(time);
                
                ChatLog.AddMessage("RESTART ISN'T FUNCTIONAL YOU FOOL!!!!");
                //G.Sys.GameManager_.FadeOutToGameMode(true);
            }

            if (string.IsNullOrEmpty(text) || text.Length > 0 && text[0] != '/')
            {
                return true;
            }
            int num1 = text.IndexOf(" ");
            string str = string.Empty;
            string key;
            bool flag = false; //This is used when dealing with specific users
            int result1 = 0;

            if(num1 != -1)
            {
                key = text.Substring(1, num1 - 1);
                str = text.Substring(num1 + 1, text.Length - num1 - 1);
                flag = int.TryParse(str, out result1);
            }
            else
            {
                key = text.Substring(1, text.Length - 1);
            }

            if(key != null)
            {
                if(key == "timeout" && Mod.Instance.AmIHost && !Mod.Instance.AllPlayersFinished)
                {
                    Events.StaticTargetedEvent<FinalCountdownActivate.Data>.Broadcast(UnityEngine.RPCMode.All, new FinalCountdownActivate.Data(Timex.ModeTime_ + Mod.Instance.Config.TimeLimitAmount, UnityEngine.Mathf.RoundToInt(Mod.Instance.Config.TimeLimitAmount)));
                    Mod.Instance.CountdownActive = true;
                    return false;
                }
                if(key == "canceltimeout" && Mod.Instance.AmIHost && !Mod.Instance.AllPlayersFinished)
                {
                    Events.StaticTargetedEvent<FinalCountdownCancel.Data>.Broadcast(UnityEngine.RPCMode.All, new FinalCountdownCancel.Data());
                    Mod.Instance.CountdownActive = false;
                    return false;
                }
                if(key == "restartme")
                {
                    ChatLog.AddMessage("Oh you're trying to restart?");
                    __instance.StartCoroutine(RestartPlayerAfter(3f));
                    Mod.Instance.Restarting = true;
                    return false;
                }
            }
            return true;
        }
    }
}
