using HarmonyLib;
using Events.Player;

namespace OnlineAdditions.Patches
{
    [HarmonyPatch(typeof(KillGrid), "OnEventAddRemovePlayerData", new System.Type[] { typeof(AddRemovePlayerData.Data) })]
    internal class KillGrid__OnEventAddRemovePlayerData
    {
        
        [HarmonyPrefix]
        internal static bool KillGridRenderControl(KillGrid __instance, AddRemovePlayerData.Data data)
        {
            if (Mod.DisableMultiKillGridRender.Value)
            {
                if (data.added_)
                {
                    if (!__instance.startCalled_)
                        Mod.Log.LogInfo((object)"start wasn't called yet");
                    if (__instance.isInvisible_)
                        return false;
                    if (data.type_ == AddRemovePlayerData.PDType.Replay)
                    {
                        PlayerDataReplay player = data.player_ as PlayerDataReplay;
                        if ((bool)(UnityEngine.Object)player && player.IsGhost_)
                            return false;
                    }
                    if (data.type_ == AddRemovePlayerData.PDType.Net)
                    {
                        //This should disable rendering for network cars on the killgrids.
                        Mod.Log.LogInfo("No killgrids for network car!");
                        return false;
                    }
                    UnityEngine.Renderer component = __instance.GetComponent<UnityEngine.Renderer>();
                    __instance.CreateParentIfNeeded();
                    UnityEngine.GameObject followerObj = UnityEngine.Object.Instantiate(__instance.followerPrefab_);
                    followerObj.transform.SetParentKeepingLocalTransform(__instance.followerParent_);
                    __instance.helpers_.Add(new KillGrid.KillGridFollowerHelper(data.player_, followerObj, component.sharedMaterial.color, __instance.curvature_, new KillGrid.PositionFollowerFn(__instance.PositionFollower)));
                }
                else
                {
                    KillGrid.KillGridFollowerHelper gridFollowerHelper = __instance.helpers_.Find((System.Predicate<KillGrid.KillGridFollowerHelper>)(val => (UnityEngine.Object)val.player_ == (UnityEngine.Object)data.player_));
                    if (gridFollowerHelper == null)
                        return false;
                    gridFollowerHelper.Destroy();
                    __instance.helpers_.Remove(gridFollowerHelper);
                }
                return false;
            }
            return true;
        }
    }
}
