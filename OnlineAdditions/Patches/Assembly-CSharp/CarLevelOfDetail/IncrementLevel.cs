using HarmonyLib;

namespace OnlineAdditions.Patches
{
    [HarmonyPatch(typeof(CarLevelOfDetail), "IncrementLevel")]
    internal class CarLevelOfDetail__IncrementLevel
    {
        [HarmonyPostfix]
        internal static void MakeSureSimulationIsOn(CarLevelOfDetail __instance)
        {
            if (__instance.type_ == CarLevelOfDetail.Type.Networked)
            {
                if (Mod.EnableCollision.Value && !Mod.Instance.playerFinished)
                {
                    __instance.SetCarSimulationEnabled(true);
                }
                else
                {
                    if (Mod.Instance.playerFinished)
                    {
                        if (__instance.rigidbody_.isKinematic)
                        {
                            __instance.rigidbodyStateTransceiver_.setCarOnFixedUpdate_ = false;
                            __instance.rigidbody_.isKinematic = false;
                        }
                    }
                }
            }
        }
    }
}
