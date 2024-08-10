using HarmonyLib;

namespace OnlineAdditions.Patches
{
    [HarmonyPatch(typeof(RigidbodyStateTransceiver), "EaseTowards")]
    internal class RigidbodyStateTransceiver__EaseTowards
    {
        [HarmonyPrefix]
        internal static bool KinematicEaseTowards(RigidbodyStateTransceiver __instance)
        {
            //The goal here is to make it possible for Kinematic network cars to update their position in a non-snappy way.
            //This will potentially make the replay camera less jittery and make collisions more accurate.
            
            if (Mod.EnableCollision.Value && !Mod.Instance.playerFinished)
            {
                RigidbodyStateTransceiver.Snapshot snapshot = RigidbodyStateTransceiver.Snapshot.Difference(__instance.goal_, new RigidbodyStateTransceiver.Snapshot(__instance.rigidbody_));
                double sqrMagnitude = snapshot.pos.sqrMagnitude;
                if (sqrMagnitude > 1000.0 || __instance.errorTimer_ > 1.0)
                    __instance.setPositionImmediate_ = true;
                else if (sqrMagnitude > 100.0)
                    __instance.errorTimer_ += UnityEngine.Time.fixedDeltaTime;
                else
                    __instance.errorTimer_ = 0.0f;
                if (__instance.setPositionImmediate_) //Note that setPositionImmediate will break physics a bit. It sets the transform directly. (Change if needed)
                    return false;
                 
                //Refract's Math.
                UnityEngine.Vector3 vector3_1 = 100f * snapshot.pos;
                UnityEngine.Vector3 vector3_2 = RigidbodyStateTransceiver.posCorrectionSpringDamping_ * snapshot.vel;
                UnityEngine.Vector3 vector3_3 = 250f * snapshot.rot.ToVector3();
                UnityEngine.Vector3 vector3_4 = RigidbodyStateTransceiver.rotCorrectionSpringDamping_ * snapshot.rotVel;

                if (__instance.rigidbody_.isKinematic)
                {
                    //Kinematic position setting
                    __instance.rigidbody_.MovePosition((vector3_1 + vector3_2) * UnityEngine.Time.fixedDeltaTime);
                    __instance.rigidbody_.MoveRotation(UnityEngine.Quaternion.Euler((vector3_3 + vector3_4) * UnityEngine.Time.fixedDeltaTime).Normalized());
                }
                else
                {
                    //Simulated position setting, this is Refract's original way.
                    __instance.rigidbody_.AddForce(vector3_1 + vector3_2, UnityEngine.ForceMode.Acceleration);
                    __instance.rigidbody_.AddTorque(vector3_3 + vector3_4, UnityEngine.ForceMode.Acceleration);
                }

                //This should prevent the car doing insane snaps into place when collisions start
                __instance.goal_.SetData(__instance.rigidbody_);
                return false;
            }
            else
            {
                __instance.rigidbody_.isKinematic = false; //Just makin sure
                return true;
            }
        }
    }
}
