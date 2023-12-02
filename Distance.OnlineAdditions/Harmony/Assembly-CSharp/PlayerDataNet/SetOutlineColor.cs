using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(PlayerDataNet), "SetOutlineColor")]
    internal class PlayerDataNet__SetOutlineColor
    {
        [HarmonyPrefix]
        internal static bool NetworkOutline(PlayerDataNet __instance)
        {
            if (!(bool)(UnityEngine.Object)__instance.outline_)
                return false;
            __instance.outline_.SetOutlineColorNoNorm(UnityEngine.Color.Lerp(UnityEngine.Color.black, __instance.GlowColor_.Normalized(), Mod.Instance.Config.OutlineBrightness));
            return false;
        }
    }
}
