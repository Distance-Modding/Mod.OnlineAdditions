using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Events;
using Events.Local;
using Events.ClientToAllClients;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace OnlineAdditions
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public sealed class Mod : BaseUnityPlugin
    {
        //Mod Details
        private const string modGUID = "Distance.OnlineAdditions";
        private const string modName = "Online Additions";
        private const string modVersion = "2.0.0";

        //Config Entry Strings
        public static string EnableCollisionKey = "Enable Collision";
        public static string EnableCheatsKey = "Enable Cheats";
        public static string EnableEventsKey = "Enable Online Events";
        public static string DisableCarAudioKey = "Disable Car Audio";
        public static string DisableTimeoutKey = "Disable Timeout";
        public static string DisableMultiKillGridRenderKey = "Disable Multiplayer KillGrid Rendering";
        public static string HideChatKey = "Hide Chat";
        public static string HidePlayersKey = "Hide Player Names";
        public static string MaxLevelOfDetailKey = "Max Level Of Car Detail";
        public static string MaxPlayerKey = "Max Players (For Host)";
        public static string OutlineKey = "Car Outline Brightness";
        public static string TimeLimitKey = "Time Limit Amount";

        //Config Entries
        public static ConfigEntry<bool> DisableCarAudio { get; set; }
        public static ConfigEntry<bool> DisableMultiKillGridRender { get; set; }
        public static ConfigEntry<bool> DisableTimeout { get; set; }
        public static ConfigEntry<bool> EnableCheats { get; set; }
        public static ConfigEntry<bool> EnableCollision { get; set; }
        public static ConfigEntry<bool> EnableOnlineEvents { get; set; }
        public static ConfigEntry<bool> HideChat { get; set; }
        public static ConfigEntry<bool> HidePlayerNames { get; set; }
        public static ConfigEntry<float> OutlineBrightness { get; set; }
        public static ConfigEntry<CarLevelOfDetail.Level> MaxLevelOfDetail { get; set; }
        public static ConfigEntry<int> MaxPlayerCount { get; set; }
        public static ConfigEntry<int> TimeLimitAmount { get; set; }

        //Public Variables
        public bool allPlayersFinished { get; set; }
        public bool amIHost { get; set; }
        public bool commandFromHost { get; set; }
        public bool countdownActive { get; set; }
        public bool playerFinished { get; set; }
        public bool selfRestart { get; set; }
        public bool uploadScore { get; set; }
        public UnityEngine.GameObject playerCar { get; set; }
        public int countdownLength { get; set; }
        public List<PlayerDataNet> networkPlayers { get; set; }
        public string playerName { get; set; }

        //Other
        private static readonly Harmony harmony = new Harmony(modGUID);
        public static ManualLogSource Log = new ManualLogSource(modName);
        public static Mod Instance;
        private bool activatingCollisions = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            Log = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            Logger.LogInfo("Thanks for using Online Additions!");

            networkPlayers = new List<PlayerDataNet>();
            

            //Config Setup
            DisableCarAudio = Config.Bind("General",
                DisableCarAudioKey,
                false,
                new ConfigDescription("Toggles whether car audio from other cars in multiplayer are on."));

            DisableMultiKillGridRender = Config.Bind("General",
                DisableMultiKillGridRenderKey,
                false,
                new ConfigDescription("Disables the rendering of the kill grid for every player in multiplayer. The killgrid will only render for you!"));

            DisableTimeout = Config.Bind("For Hosting",
                DisableTimeoutKey,
                false,
                new ConfigDescription("Completely disables the timeout when one player is left."));

            MaxPlayerCount = Config.Bind("For Hosting",
                MaxPlayerKey,
                30,
                new ConfigDescription("Adjust the max number of players that can join your server"));

            EnableCheats = Config.Bind("Turns Off Leaderboard",
                EnableCheatsKey,
                false,
                new ConfigDescription("Toggle whether cheats can be enabled in multiplayer."));

            EnableCollision = Config.Bind("Turns Off Leaderboard",
                EnableCollisionKey,
                false,
                new ConfigDescription("Toggle whether collisions are enabled when playing online."));

            EnableOnlineEvents = Config.Bind("Turns Off Leaderboard",
                EnableEventsKey,
                false,
                new ConfigDescription("Toggle whether events get triggered by all players in multiplayer (Only works with Collisions on.) NOTE: CAN BREAK SEVERAL LEVELS!"));

            HideChat = Config.Bind("General",
                HideChatKey,
                false,
                new ConfigDescription("Toggle whether the chat stays completely hidden in multiplayer."));

            HidePlayerNames = Config.Bind("General",
                HidePlayersKey,
                false,
                new ConfigDescription("Toggle whether online player names are visible."));

            OutlineBrightness = Config.Bind("General",
                OutlineKey,
                1.0f,
                new ConfigDescription("Adjust the brightness of the outlines on online player cars",
                    new AcceptableValueRange<float>(0.0f, 1.0f)));

            //Ultra, Very High, High, Medium, Low, Very Low,
            MaxLevelOfDetail = Config.Bind("General",
                MaxLevelOfDetailKey,
                CarLevelOfDetail.Level.InFocus,
                new ConfigDescription("The maximum detail online cars can have. This will lower the visual quality other cars have online. InFocus is the usual default. For an idea of how this looks, Speck turns off all animations of an online car."));

            TimeLimitAmount = Config.Bind("For Hosting",
                TimeLimitKey,
                60,
                new ConfigDescription("Adjust the amount of time is set when a time limit occurs",
                    new AcceptableValueRange<int>(0, 600)));

            DisableCarAudio.SettingChanged += OnConfigChanged;
            DisableMultiKillGridRender.SettingChanged += OnConfigChanged;
            DisableTimeout.SettingChanged += OnConfigChanged;
            EnableCheats.SettingChanged += OnConfigChanged;
            EnableCollision.SettingChanged += OnConfigChanged;
            HideChat.SettingChanged += OnConfigChanged;
            HidePlayerNames.SettingChanged += OnConfigChanged;
            OutlineBrightness.SettingChanged += OnConfigChanged;
            MaxLevelOfDetail.SettingChanged += OnConfigChanged;
            MaxPlayerCount.SettingChanged += OnConfigChanged;
            TimeLimitAmount.SettingChanged += OnConfigChanged;

            //Apply Patches
            Logger.LogInfo("Loading...");
            harmony.PatchAll();
            Logger.LogInfo("Loaded!");
        }

        public void LateInitialize()
        {
            //Used for nothin right now
        }

        private void OnConfigChanged(object sender, EventArgs e)
        {
            SettingChangedEventArgs settingChangedEventArgs = e as SettingChangedEventArgs;

            if (settingChangedEventArgs == null) return;

            if (settingChangedEventArgs.ChangedSetting.Definition.Key == EnableCollisionKey || 
                settingChangedEventArgs.ChangedSetting.Definition.Key == EnableCheatsKey || 
                settingChangedEventArgs.ChangedSetting.Definition.Key == EnableEventsKey)
            {
                uploadScore = false;
                
            }
        }

        public void ActivateRestart()
        {
            //StopCoroutine("ChatInput");
            StartCoroutine(ActivateRestartAfterSeconds(2));
        }

        //Should restart the level BUT IT DOESN'T WORK! SAD!
        public System.Collections.IEnumerator ActivateRestartAfterSeconds(float seconds)
        {
            yield return new UnityEngine.WaitForSeconds(seconds);
            //Log.LogInfo("I am the Restart Coroutine");
            playerCar = null; //I have to make the car null to void the infinite loading screen
            //Player's state needs to be set to LoadingGameModeScene so it doesn't get stuck.
            selfRestart = true;
            if (Instance.playerFinished && !Instance.countdownActive)
            {
                //MMmmmmm sadge
                G.Sys.PlayerManager_.clientLogic_.UpdateLevelIfNecessaryThenGoToGameMode();
            }
        }

        public System.Collections.IEnumerator ActivateCollidersAfterSeconds(float seconds)
        {
            networkPlayers.RemoveAll(item => item == null);
            if (!networkPlayers.IsNullOrEmpty())
            {
                activatingCollisions = true;
                foreach (PlayerDataNet playerNet in networkPlayers)
                {
                    if (playerNet.CarLOD_ != null)
                    {
                        //Log.LogInfo("Collisions off for " + playerNet.name_);
                        playerNet.SetAllColliderLayers(Layers.OnlineCar);
                        playerNet.CarLOD_.rigidbody_.isKinematic = false;
                        playerNet.CarLOD_.SetCarSimulationEnabled(false);
                        playerNet.GlowColor_ = UnityEngine.Color.white;
                        playerNet.SetOutlineColor();
                    }
                    //else
                        //Log.LogInfo("Car Does not Exist!");
                }
                yield return new UnityEngine.WaitForSeconds(seconds);
                activatingCollisions = false;
                if (!Mod.Instance.playerFinished)
                {
                    networkPlayers.RemoveAll(item => item == null);
                    foreach (PlayerDataNet playerNet in networkPlayers)
                    {
                        if (playerNet.CarLOD_ != null)
                        {
                            //Log.LogInfo("Collisions on for " + playerNet.name_);
                            playerNet.SetAllColliderLayers(Layers.Player2);
                            playerNet.CarLOD_.rigidbody_.isKinematic = true;
                            playerNet.CarLOD_.SetCarSimulationEnabled(true);
                            playerNet.GlowColor_ = playerNet.OriginalGlowColor_;
                            playerNet.SetOutlineColor();
                        }
                       // else
                            //Log.LogInfo("Car Does not Exist!");
                    }
                }
            }
            //else
                //Log.LogInfo("No Network Players exist!");
        }

        public System.Collections.IEnumerator ActivateCollidersAfterSeconds(float seconds, PlayerDataNet playerNet)
        {
            if (!activatingCollisions)
            {
                playerNet.SetAllColliderLayers(Layers.OnlineCar);
                playerNet.CarLOD_.rigidbody_.isKinematic = false;
                playerNet.CarLOD_.SetCarSimulationEnabled(false);
                playerNet.GlowColor_ = UnityEngine.Color.white;
                playerNet.SetOutlineColor();
                yield return new UnityEngine.WaitForSeconds(seconds);
                if (playerNet.CarLOD_ != null)
                {
                    //Log.LogInfo("Collisions on for " + playerNet.name_);
                    playerNet.SetAllColliderLayers(Layers.Player2);
                    playerNet.CarLOD_.rigidbody_.isKinematic = true;
                    playerNet.CarLOD_.SetCarSimulationEnabled(true);
                    playerNet.GlowColor_ = playerNet.OriginalGlowColor_;
                    playerNet.SetOutlineColor();
                }
            }
            else
            {
                //Log.LogInfo("Already Activating Collisions");
            }
        }
    }
}
