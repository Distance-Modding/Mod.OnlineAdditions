using HarmonyLib;

namespace OnlineAdditions.Patches
{
    //Add the network player to the list!
    [HarmonyPatch(typeof(PlayerDataNet), "AwakeVirtual")]
    internal class PlayerDataNet__AwakeVirtual
    {
        [HarmonyPostfix]
        internal static void SubscribeToEvent(PlayerDataNet __instance)
        {
            Mod.Instance.networkPlayers.Add(__instance);
        }
    }
}
