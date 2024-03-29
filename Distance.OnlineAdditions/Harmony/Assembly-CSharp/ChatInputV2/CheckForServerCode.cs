﻿using Events.RaceMode;
using HarmonyLib;
using System.Text.RegularExpressions;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(ChatInputV2), "CheckForServerCode", new System.Type[] {typeof(string) })]
    internal class ChatInputV2__CheckForServerCode
    {
        [HarmonyPrefix]
        internal static bool CheckForCommands(ChatInputV2 __instance, string text)
        {
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
                if (key == "timeout" && Mod.Instance.AmIHost && !Mod.Instance.AllPlayersFinished)
                {
                    int time;
                    if (Regex.Match(str, @"^\d+$").Success)
                    {
                        time = int.Parse(Regex.Match(str, @"^\d+$").Value);
                    }
                    else
                    {
                        time = Mod.Instance.Config.TimeLimitAmount;
                    }

                    Events.StaticTargetedEvent<FinalCountdownActivate.Data>.Broadcast(UnityEngine.RPCMode.All, new FinalCountdownActivate.Data(Timex.ModeTime_ + time, UnityEngine.Mathf.RoundToInt(time)));
                    Mod.Instance.CountdownActive = true;
                    return false;
                }
                if(key == "canceltimeout" && Mod.Instance.AmIHost && !Mod.Instance.AllPlayersFinished)
                {
                    Events.StaticTargetedEvent<FinalCountdownCancel.Data>.Broadcast(UnityEngine.RPCMode.All, new FinalCountdownCancel.Data());
                    Mod.Instance.CountdownActive = false;
                    return false;
                }
            }
            return true;
        }
    }
}
