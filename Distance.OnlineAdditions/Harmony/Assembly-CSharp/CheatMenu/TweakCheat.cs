using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(CheatMenu), "TweakCheat", new System.Type[] { typeof(ECheat) })]
    internal class TweakCheat__CheatMenu
    {

        [HarmonyPrefix]
        internal static bool CheckMotorcycleCheat(ECheat c)
        {
            //It will not generate the motorcycle cheat in the menu if cheats are enabled in multiplayer
            if (c == ECheat.MotorcycleMode && Mod.Instance.Config.EnableCheatsInMultiplayer)
                return false;
            else
                return true;
        }
    }
}

