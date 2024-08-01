using HarmonyLib;

namespace OnlineAdditions.Patches
{
	[HarmonyPatch(typeof(NetworkingManager), "CreateServer")]
	internal static class NetworkingManager__CreateServer
	{
		internal static bool Prefix(NetworkingManager __instance, string serverTitle, string password, int maxPlayerCount)
		{
			UnityEngine.Network.InitializeSecurity();

			try
			{
				__instance.password_ = password;
				__instance.serverTitle_ = serverTitle;

				G.Sys.GameData_.SetString("ServerTitleDefault", __instance.serverTitle_);

				__instance.maxPlayerCount_ = UnityEngine.Mathf.Clamp(maxPlayerCount, 1, Mod.MaxPlayerCount.Value);

				G.Sys.GameData_.SetInt("MaxPlayersDefault", __instance.maxPlayerCount_);

				const int num = 1;
				int connections = __instance.maxPlayerCount_ - num;

				UnityEngine.NetworkConnectionError networkConnectionError = UnityEngine.Network.InitializeServer(connections, 32323, true);

				if (networkConnectionError != UnityEngine.NetworkConnectionError.NoError)
				{
					G.Sys.MenuPanelManager_.ShowError("Failed to create game lobby. Error code: " + networkConnectionError.ToString(), "Network Error", null, UIWidget.Pivot.Center);
				}
			}
			catch (System.Exception ex)
			{
				UnityEngine.Debug.LogError(ex.Message);
				Mod.Log.LogInfo(ex);
			}

			return false;
		}
	}
}
