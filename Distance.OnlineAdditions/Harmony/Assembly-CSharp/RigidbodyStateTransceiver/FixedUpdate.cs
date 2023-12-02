using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(RigidbodyStateTransceiver), "FixedUpdate")]
    internal class RigidbodyStateTransceiver__FixedUpdate
    {
        [HarmonyPrefix]
        internal static bool FixedTheFixedUpdate(RigidbodyStateTransceiver __instance)
        {
            if (!__instance.setCarOnFixedUpdate_)
                return false;
            __instance.rigidbody_.transform.position = __instance.posSpring_.Pos_ + __instance.setCarOnFixedUpdateCoef_ * 0.01f * __instance.velSpring_.Pos_;
            __instance.rigidbody_.transform.rotation = __instance.rotSpring_.Pos_;
            __instance.rigidbody_.velocity = __instance.velSpring_.Pos_;
            __instance.rigidbody_.angularVelocity = __instance.rotSpring_.Vel_;
            return false;
        }
    }
}
