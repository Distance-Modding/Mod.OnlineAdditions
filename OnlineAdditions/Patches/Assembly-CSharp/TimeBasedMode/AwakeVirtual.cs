using HarmonyLib;

namespace OnlineAdditions.Patches
{
    //I'm stealing some booleans
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
