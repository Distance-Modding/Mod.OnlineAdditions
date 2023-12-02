using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(CarLevelOfDetail), "IncrementLevel")]
    internal class CarLevelOfDetail__IncrementLevel
    {
        [HarmonyPostfix]
        internal static void MakeSureSimulationIsOn(CarLevelOfDetail __instance)
        {
            if (__instance.type_ == CarLevelOfDetail.Type.Networked && Mod.Instance.Config.EnableCollision)
            {
                __instance.SetCarSimulationEnabled(true);
            }
            else
            {
                if(__instance.rigidbody_.isKinematic)
                {
                    __instance.rigidbody_.isKinematic = false;
                }
            }
        }
    }
}
