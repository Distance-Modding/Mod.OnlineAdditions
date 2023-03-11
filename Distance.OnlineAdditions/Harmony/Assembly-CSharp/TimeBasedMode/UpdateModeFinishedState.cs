using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(TimeBasedMode), "UpdateModeFinishedState")]
    internal class TimeBasedMode__UpdateModeFinishedState
    {
        [HarmonyPrefix]
        internal static bool ModifyFinalCountdown(TimeBasedMode __instance)
        {
            if (__instance.IsHost_)
            {
                Mod.Instance.AmIHost = true;

                if (__instance.modePlayerInfos_.FindAll(val => val != null && val.finishType_ != FinishType.None).Count == __instance.modePlayerInfos_.Count)
                {
                    Mod.Instance.Logger.Debug("ALL PLAYERS HAVE FINISHED!");
                    Mod.Instance.AllPlayersFinished = true;
                    Mod.Instance.CountdownActive = false;
                }
                else
                {
                    Mod.Instance.Logger.Debug("NOT ALL OF THE PLAYERS FINISHED YET");
                    Mod.Instance.AllPlayersFinished = false;
                }

                //Timeout Stuff Starts here
                if (Mod.Instance.Config.DisableTimeout)
                {
                    return false;
                }

                int count1 = __instance.modePlayerInfos_.FindAll(val => val == null || val.finishType_ != FinishType.None).Count;
                int count2 = __instance.modePlayerInfos_.FindAll((val => val != null && val.finishType_ == FinishType.Spectate)).Count;
                int num1 = __instance.modePlayerInfos_.Count - count2; //All players who are currently loaded - Spectators
                int num2 = count1 - count2; //All players who have finished - Spectators

                if (!__instance.theFinalCountdown_)
                {
                    if (num1 <= 1 || num2 != num1 - 1)
                    {
                        return false;
                    }
                    Events.StaticTargetedEvent<Events.RaceMode.FinalCountdownActivate.Data>.Broadcast(UnityEngine.RPCMode.All, new Events.RaceMode.FinalCountdownActivate.Data(Timex.ModeTime_ + Mod.Instance.Config.TimeLimitAmount, UnityEngine.Mathf.RoundToInt(Mod.Instance.Config.TimeLimitAmount)));
                    Mod.Instance.CountdownActive = true;
                }
                else
                {
                    if (num2 < num1 || num2 >= num1 - 1)
                    {
                        return false;
                    }
                    Mod.Instance.Logger.Debug("Someone seems to have joined during the countdown! Canceling!");
                    //I don't actually know for sure if that's what's happening here. If that is the case Ima be pretty happy though.
                    Events.StaticTargetedEvent<Events.RaceMode.FinalCountdownCancel.Data>.Broadcast(UnityEngine.RPCMode.All, new Events.RaceMode.FinalCountdownCancel.Data());
                    Mod.Instance.CountdownActive = false;
                }
                return false;
            }
            Mod.Instance.AmIHost = false;
            return false;
        }
    }
}
