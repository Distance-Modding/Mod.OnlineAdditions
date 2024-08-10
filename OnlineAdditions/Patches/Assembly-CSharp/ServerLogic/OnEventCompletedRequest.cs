using HarmonyLib;

namespace OnlineAdditions.Patches
{
    [HarmonyPatch(typeof(ServerLogic), "OnEventCompletedRequest", new System.Type[] { typeof(Events.ClientToServer.CompletedRequest.Data) })]
    internal static class ServerLogic__OnEventCompletedRequest
    {
        [HarmonyPrefix]
        internal static void SetState(ServerLogic __instance, Events.ClientToServer.CompletedRequest.Data data)
        {
            ServerLogic.ClientInfo clientInfo = __instance.GetClientInfo(data.networkPlayer_);
            if (data.request_ == ServerRequest.LoadGameModeScene && Mod.Instance.selfRestart)
            {
                clientInfo.SetStateOnCondition(ServerLogic.ClientInfo.State.LoadingGameModeScene, ServerLogic.ClientInfo.State.StartedMode);
                Mod.Instance.selfRestart = false;
            }
        }
    }
}
