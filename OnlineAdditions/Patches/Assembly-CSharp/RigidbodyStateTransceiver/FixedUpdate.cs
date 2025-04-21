using HarmonyLib;

namespace OnlineAdditions.Patches
{
    [HarmonyPatch(typeof(RigidbodyStateTransceiver), "FixedUpdate")]
    internal class RigidbodyStateTransceiver__FixedUpdate
    {
        [HarmonyPrefix]
        internal static bool FixedTheFixedUpdate(RigidbodyStateTransceiver __instance)
        {
            //Refract's original code actually forced Network cars to never use the fixed update. I'm not sure why, but this makes it so fixedUpdate can remain true.
            //Also instead of setting the transform directly, I'm using MovePosition so the physics don't get confused
            if (!__instance.setCarOnFixedUpdate_)
                return false;
            __instance.rigidbody_.MovePosition(__instance.posSpring_.Pos_ + __instance.setCarOnFixedUpdateCoef_ * 0.01f * __instance.velSpring_.Pos_);
            __instance.rigidbody_.MoveRotation(__instance.rotSpring_.Pos_.Normalized());
            //Velocity is already being calulated by MovePos/Rot so those lines are commented out
            //__instance.rigidbody_.velocity = __instance.velSpring_.Pos_;
            //__instance.rigidbody_.angularVelocity = __instance.rotSpring_.Vel_;
            return false;
        }
    }
}
