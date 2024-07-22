using HarmonyLib;

namespace OnlineAdditions.Patches
{
    [HarmonyPatch(typeof(TimeBasedMode), "UpdateModeFinishedState")]
    internal class TimeBasedMode__UpdateModeFinishedState
    {
        [HarmonyPrefix]
        internal static bool ModifyFinalCountdown(TimeBasedMode __instance)
        {
            if (__instance.IsHost_)
            {
                Mod.Instance.amIHost = true;

                if (__instance.modePlayerInfos_.FindAll(val => val != null && val.finishType_ != FinishType.None).Count == __instance.modePlayerInfos_.Count)
                {
                    Mod.Log.LogInfo("ALL PLAYERS HAVE FINISHED!");
                    Mod.Instance.allPlayersFinished = true;
                    Mod.Instance.countdownActive = false;
                }
                else
                {
                    Mod.Log.LogInfo("NOT ALL OF THE PLAYERS FINISHED YET");
                    Mod.Instance.allPlayersFinished = false;
                }

                //Timeout Stuff Starts here
                if (Mod.DisableTimeout.Value)
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
                    Events.StaticTargetedEvent<Events.RaceMode.FinalCountdownActivate.Data>.Broadcast(UnityEngine.RPCMode.All, new Events.RaceMode.FinalCountdownActivate.Data(Timex.ModeTime_ + Mod.TimeLimitAmount.Value, UnityEngine.Mathf.RoundToInt(Mod.TimeLimitAmount.Value)));
                    Mod.Instance.countdownActive = true;
                }
                else
                {
                    if (num2 < num1 || num2 >= num1 - 1)
                    {
                        return false;
                    }
                    Mod.Log.LogInfo("Someone seems to have joined during the countdown! Canceling!");
                    //I don't actually know for sure if that's what's happening here. If that is the case Ima be pretty happy though.
                    Events.StaticTargetedEvent<Events.RaceMode.FinalCountdownCancel.Data>.Broadcast(UnityEngine.RPCMode.All, new Events.RaceMode.FinalCountdownCancel.Data());
                    Mod.Instance.countdownActive = false;
                }
                return false;
            }
            Mod.Instance.amIHost = false;
            return false;
        }
    }
}
