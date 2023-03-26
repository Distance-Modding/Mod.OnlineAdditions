using HarmonyLib;

namespace Distance.OnlineAdditions.Harmony
{
    [HarmonyPatch(typeof(RigidbodyStateTransceiver), "EaseTowards")]
    internal class RigidbodyStateTransceiver__EaseTowards
    {
        [HarmonyPrefix]
        internal static bool KinematicEaseTowards(RigidbodyStateTransceiver __instance)
        {
            if (Mod.Instance.Config.EnableCollision && !Mod.Instance.PlayerFinished)
            {
                RigidbodyStateTransceiver.Snapshot snapshot = RigidbodyStateTransceiver.Snapshot.Difference(__instance.goal_, new RigidbodyStateTransceiver.Snapshot(__instance.rigidbody_));
                double sqrMagnitude = snapshot.pos.sqrMagnitude;
                if (sqrMagnitude > 1000.0 || __instance.errorTimer_ > 1.0)
                    __instance.setPositionImmediate_ = true;
                else if (sqrMagnitude > 100.0)
                    __instance.errorTimer_ += UnityEngine.Time.fixedDeltaTime;
                else
                    __instance.errorTimer_ = 0.0f;
                if (__instance.setPositionImmediate_)
                    return false;

                /*This is Refract's original math for positiong the car when the rigidbody is using simulated physics instead of kinematic.
                 * 
                 * UnityEngine.Vector3 vector3_1 = 100f * snapshot.pos;
                UnityEngine.Vector3 vector3_2 = RigidbodyStateTransceiver.posCorrectionSpringDamping_ * snapshot.vel;
                UnityEngine.Vector3 vector3_3 = 250f * snapshot.rot.ToVector3();
                UnityEngine.Vector3 vector3_4 = RigidbodyStateTransceiver.rotCorrectionSpringDamping_ * snapshot.rotVel;
                UnityEngine.Quaternion deltaRotation = UnityEngine.Quaternion.Euler(vector3_3 + vector3_4);*/

                RigidbodyStateTransceiver.Snapshot prevSnapshot = RigidbodyStateTransceiver.Snapshot.Difference(__instance.prevGoal_, new RigidbodyStateTransceiver.Snapshot(__instance.rigidbody_));

                __instance.rigidbody_.interpolation = UnityEngine.RigidbodyInterpolation.Interpolate;

                UnityEngine.Vector3 smoothpos = sqrMagnitude < .05f ? snapshot.pos : UnityEngine.Vector3.Lerp(prevSnapshot.pos, snapshot.pos, .5f);
                UnityEngine.Quaternion smoothrot = sqrMagnitude < .05f ? snapshot.rot : UnityEngine.Quaternion.Lerp(prevSnapshot.rot, snapshot.rot, .5f);


                __instance.rigidbody_.MovePosition(__instance.rigidbody_.position + smoothpos);
                __instance.rigidbody_.MoveRotation(smoothrot * __instance.rigidbody_.rotation);
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
