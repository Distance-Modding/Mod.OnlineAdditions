using HarmonyLib;

namespace OnlineAdditions.Patches
{
    //Grabbing variables.
    [HarmonyPatch(typeof(PlayerDataLocal), "InitCarVirtual")]
    internal class PlayerDataLocal__InitCarVirtual
    {
        [HarmonyPostfix]
        internal static void GetCar(PlayerDataLocal __instance)
        {
            Mod.Instance.playerCar = __instance.carObj_;
            Mod.Instance.playerFinished = false;
        }
    }
}
