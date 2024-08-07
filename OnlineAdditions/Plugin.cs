﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineAdditions
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public sealed class Mod : BaseUnityPlugin
    {
        //Mod Details
        private const string modGUID = "Distance.OnlineAdditions";
        private const string modName = "Online Additions";
        private const string modVersion = "1.9";

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
        public static string MaxPlayerKey = "Max Number of Players for Hosting Servers";
        public static string OutlineKey = "Adjust Brightness of Car Outlines";
        public static string TimeLimitKey = "Adjust Time Limit Amount";

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
        public static ConfigEntry<int> MaxLevelOfDetail { get; set; }
        public static ConfigEntry<int> MaxPlayerCount { get; set; }
        public static ConfigEntry<int> TimeLimitAmount { get; set; }

        //Public Variables
        public bool allPlayersFinished { get; set; }
        public bool amIHost { get; set; }
        public bool countdownActive { get; set; }
        public bool playerFinished { get; set; }
        public bool uploadScore { get; set; }
        public UnityEngine.GameObject playerCar { get; set; }
        public int countdownLength { get; set; }

        //Other
        private static readonly Harmony harmony = new Harmony(modGUID);
        public static ManualLogSource Log = new ManualLogSource(modName);
        public static Mod Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            Log = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            Logger.LogInfo("Thanks for using Online Additions!");

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
                new ConfigDescription("Toggle whether events get triggered by all players in multiplayer"));

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
                1,
                new ConfigDescription("The maximum detail online cars can have. This will lower the visual quality other cars have online. 1 is the usual default. For an idea of how this looks, 6 turns off all animations of an online car.",
                    new AcceptableValueRange<int>(1,6)));

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
            StopCoroutine("Chat Input");
            StartCoroutine(ActivateRestartAfterSeconds(2));
        }

        //Should restart the level.
        private System.Collections.IEnumerator ActivateRestartAfterSeconds(float seconds)
        {
            yield return new UnityEngine.WaitForSeconds(seconds);
            Log.LogInfo("I am the Restart Coroutine");
            if (!Instance.playerFinished && !Instance.countdownActive)
            {
                G.Sys.GameManager_.RestartLevel();
            }
        }
    }
}
