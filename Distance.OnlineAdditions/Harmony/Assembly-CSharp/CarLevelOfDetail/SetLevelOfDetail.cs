using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(CarLevelOfDetail), "SetLevelOfDetail", new System.Type[] { typeof(CarLevelOfDetail.Level) })]
    internal class CarLevelOfDetail__SetLevelOfDetail
    {
        [HarmonyPrefix]
        internal static bool MaxLevelDetail(CarLevelOfDetail __instance, CarLevelOfDetail.Level newLevel)
        {
            if(newLevel < (CarLevelOfDetail.Level)Mod.Instance.Config.MaxLevelOfDetail && G.Sys.NetworkingManager_.IsOnline_)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
