using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(CarLevelOfDetail), "IncrementLevel")]
    internal class CarLevelOfDetail__IncrementLevel
    {
        [HarmonyPostfix]
        internal static void MakeSureSimulationIsOn(CarLevelOfDetail __instance)
        {
            if (__instance.type_ == CarLevelOfDetail.Type.Networked)
            {
                if (Mod.Instance.Config.EnableCollision && !Mod.Instance.PlayerFinished)
                {
                    __instance.SetCarSimulationEnabled(true);
                }
                else
                {
                    if (Mod.Instance.PlayerFinished)
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
