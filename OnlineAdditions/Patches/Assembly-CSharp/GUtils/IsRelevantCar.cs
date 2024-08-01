using HarmonyLib;

namespace OnlineAdditions.Patches
{
    [HarmonyPatch(typeof(GUtils), "IsRelevantCar", new System.Type[] { typeof(UnityEngine.Collider) })]
    internal class GUtils__IsRelevantCar
    {
        //Make it so Network cars can trigger event triggers
        //Recreating Refract's code to be safe lol
        [HarmonyPostfix]
        internal static void IsRelevantNetworkCar(UnityEngine.Collider car, ref PlayerDataBase __result)
        {
            if (Mod.EnableOnlineEvents.Value && G.Sys.NetworkingManager_.IsOnline_)
            {
                if ((UnityEngine.Object)car != (UnityEngine.Object)null)
                {
                    CarLogic component = car.GetComponent<CarLogic>();
                    if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.enabled)
                    {
                        PlayerDataBase playerData = component.PlayerData_;
                        if (playerData.IsAliveAndNotFinished_)
                            __result = playerData.IsCarRelevant_ || playerData is PlayerDataNet ? playerData : (PlayerDataBase)null;
                    }
                }
                else
                {
                    __result = (PlayerDataBase)null;
                }
            }
        }
    }
}
