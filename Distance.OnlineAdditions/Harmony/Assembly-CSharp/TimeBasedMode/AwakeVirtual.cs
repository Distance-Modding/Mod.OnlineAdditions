using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(TimeBasedMode), "AwakeVirtual")]
    internal class TimeBasedMode__AwakeVirtual
    {
        [HarmonyPostfix]
        internal static void VirtualedYourVirtual(TimeBasedMode __instance)
        {
            Mod.Instance.Logger.Debug("SETTING BOOLS");
            Mod.Instance.AmIHost = __instance.IsHost_;
            Mod.Instance.CountdownActive = false;
            Mod.Instance.AllPlayersFinished = false;
        }
    }
}
