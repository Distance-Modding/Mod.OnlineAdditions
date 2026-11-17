using HarmonyLib;

namespace OnlineAdditions.Patches
{
    [HarmonyPatch(typeof(CheatMenu), "TweakCheat", new System.Type[] { typeof(ECheat) })]
    internal class TweakCheat__CheatMenu
    {

        [HarmonyPrefix]
        internal static bool CheckMotorcycleCheat(ECheat c)
        {
            //It will not generate the motorcycle cheat in the menu when you are in multiplayer
            if (c == ECheat.MotorcycleMode && G.Sys.NetworkingManager_.IsOnline_)
                return false;
            else
                return true;
        }
    }
}
