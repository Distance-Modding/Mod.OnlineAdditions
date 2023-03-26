using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(PlayerDataLocal), "InitCarVirtual")]
    internal class InitCarVirtual
    {
        [HarmonyPostfix]
        internal static void GetCar(PlayerDataLocal __instance)
        {
            Mod.Instance.playerCar = __instance.carObj_;
            Mod.Instance.PlayerFinished = false;
        }
    }
}
