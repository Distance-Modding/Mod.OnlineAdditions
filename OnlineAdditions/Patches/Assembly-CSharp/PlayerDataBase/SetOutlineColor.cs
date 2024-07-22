using HarmonyLib;

namespace OnlineAdditions.Patches
{
    [HarmonyPatch(typeof(PlayerDataBase), "SetOutlineColor")]
    internal class PlayerDataBase__SetOutlineColor
    {
        [HarmonyPrefix]
        internal static bool NetworkOutline(PlayerDataBase __instance)
        {
            if (!(__instance is PlayerDataNet playerDataNet))
            {
                return true;
            }

            if (!(bool)(UnityEngine.Object)__instance.outline_)
                return false;
            __instance.outline_.SetOutlineColorNoNorm(UnityEngine.Color.Lerp(UnityEngine.Color.black, __instance.GlowColor_.Normalized(), Mod.OutlineBrightness.Value));
            return false;
        }
    }
}
