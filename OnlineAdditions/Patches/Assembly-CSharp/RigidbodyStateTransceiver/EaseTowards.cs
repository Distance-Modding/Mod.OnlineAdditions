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

                /*
                 * I gave up. This didn't quite work the way I wanted and I think it'll take too long to do it at this point
                 * 
                //Raycast to prevent the car from moving through roads

                //Raycast and the line renderer may not be the same, figure that shit out mang
                UnityEngine.LineRenderer line = __instance.GetOrAddComponent<UnityEngine.LineRenderer>();
                line.useWorldSpace = false;
                line.sortingOrder = 1;
                UnityEngine.RaycastHit hit;
                UnityEngine.Vector3 hit_position = UnityEngine.Vector3.zero;
                UnityEngine.Vector3 result_position;
                UnityEngine.Vector3 snap_direction = (__instance.rigidbody_.position - snapshot.pos).normalized;
                if (UnityEngine.Physics.Raycast(__instance.prevGoal_.pos, snap_direction, out hit, 500)) //use sqrmagnitude instead of 500
                {
                    line.material.color = UnityEngine.Color.yellow;
                    line.SetPosition(0, __instance.rigidbody_.position);
                    line.SetPosition(1, hit.point);
                    hit_position = hit.point;
                }
                else
                {
                    line.material.color = UnityEngine.Color.green;
                    line.SetPosition(0, __instance.rigidbody_.position);
                    line.SetPosition(0, snapshot.pos);
                }

                result_position = (vector3_1 + vector3_2) * UnityEngine.Time.fixedDeltaTime;

                if (hit_position != UnityEngine.Vector3.zero)
                {
                    //Compare magnitude instead of distance?? Hmm.
                    if (UnityEngine.Vector3.Distance((vector3_1 + vector3_2) * UnityEngine.Time.fixedDeltaTime, __instance.transform.position) > UnityEngine.Vector3.Distance(hit_position, __instance.transform.position))
                    {
                        //Move it backward along the ray??? The result should be at the hit_position I think
                        result_position = (vector3_1 + vector3_2) * UnityEngine.Time.fixedDeltaTime + (UnityEngine.Vector3.up * UnityEngine.Vector3.Distance((vector3_1 + vector3_2) * UnityEngine.Time.fixedDeltaTime, hit_position));
                    }
                }*/

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
