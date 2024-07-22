using HarmonyLib;

namespace OnlineAdditions.Patches
{
    [HarmonyPatch(typeof(TimeBasedMode), "AwakeVirtual")]
    internal class TimeBasedMode__AwakeVirtual
    {
        [HarmonyPostfix]
        internal static void VirtualedYourVirtual(TimeBasedMode __instance)
        {
            Mod.Instance.amIHost = __instance.IsHost_;
            Mod.Instance.countdownActive = false;
            Mod.Instance.allPlayersFinished = false;
        }
    }
}
