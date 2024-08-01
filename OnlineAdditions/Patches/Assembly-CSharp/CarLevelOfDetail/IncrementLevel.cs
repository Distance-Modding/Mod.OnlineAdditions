using HarmonyLib;

namespace OnlineAdditions.Patches
{
    //This is here to make sure a network car's simulation remains no matter what level of detail is on.
    //Problem: Cars mega far away still simulate, this will change.
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
                            //Make sure fixedUpdate stops getting used as it's no longer required when collisions are off
                            __instance.rigidbodyStateTransceiver_.setCarOnFixedUpdate_ = false;
                            __instance.rigidbody_.isKinematic = false;
                        }
                    }
                }
            }
        }
    }
}
