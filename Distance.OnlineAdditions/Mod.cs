using Reactor.API.Attributes;
using Reactor.API.Interfaces.Systems;
using Reactor.API.Logging;
using Reactor.API.Runtime.Patching;
using Centrifuge.Distance.Game;
using Centrifuge.Distance.GUI.Data;
using Centrifuge.Distance.GUI.Controls;
using System;
using UnityEngine;

namespace Distance.OnlineAdditions
{
    /// <summary>
    /// The mod's main class containing its entry point
    /// </summary>
    [ModEntryPoint("Online Additions")]
    public sealed class Mod : MonoBehaviour
    {
        public static Mod Instance { get; private set; }

        public IManager Manager { get; private set; }

        public Log Logger { get; private set; }

        public ConfigLogic Config { get; private set; }

        public GameObject playerCar { get; set; }

        public bool UploadScore { get; set; }
        public bool AmIHost { get; set; }
        public bool AllPlayersFinished { get; set; }
        public bool PlayerFinished { get; set; }
        public bool CountdownActive { get; set; }
        public int CountdownLength { get; set; }

        /// <summary>
        /// Method called as soon as the mod is loaded.
        /// WARNING:	Do not load asset bundles/textures in this function
        ///				The unity assets systems are not yet loaded when this
        ///				function is called. Loading assets here can lead to
        ///				unpredictable behaviour and crashes!
        /// </summary>
        public void Initialize(IManager manager)
        {
            // Do not destroy the current game object when loading a new scene
            DontDestroyOnLoad(this);

            Instance = this;

            Manager = manager;

            //True by default so the first time that gets set online isn't guarenteed to not upload
            UploadScore = true;
            AmIHost = false;
            AllPlayersFinished = false;
            CountdownActive = false;

            Config = gameObject.AddComponent<ConfigLogic>();

            CountdownLength = Config.TimeLimitAmount;

            //Check whether or not leaderboard uploads can happen
            OnConfigChanged(Config);

            //Subcribe to config event
            Config.OnChanged += OnConfigChanged;

            // Create a log file
            Logger = LogManager.GetForCurrentAssembly();

            Logger.Info("Thanks for using Online Online Additions! Your one stop shop for everything multiplayer!");

            try
            {
                CreateSettingsMenu();
            }
            catch (Exception e)
            {
                Logger.Exception(e);
                Logger.Error("This likely happened because you have the wrong version of Centrifuge.Distance. \nTo fix this, be sure to use the Centrifuge.Distance.dll file that came included with the mod's zip file. \nDespite this error, the mod will still function, however, you will not have access to the mod's menu.");
            }

            try
            {
                // Never ever EVER use this!!!
                // It's the same as below (with `GetCallingAssembly`) wrapped around a silent catch-all.
                //RuntimePatcher.AutoPatch();

                RuntimePatcher.HarmonyInstance.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Logger.Error("Online Additions: Error during Harmony.PatchAll()");
                Logger.Exception(ex);
                throw;
            }
        }

        private void CreateSettingsMenu()
        {
            MenuTree settingsMenu = new MenuTree("menu.mod.onlineadditions", "Online Additions Settings")
            {
                new CheckBox(MenuDisplayMode.Both, "settings:enable_onlinecheats", "ENABLE CHEATS IN MULTIPLAYER")
                .WithGetter(() => Config.EnableCheatsInMultiplayer)
                .WithSetter((x) => Config.EnableCheatsInMultiplayer = x)
                .WithDescription("Toggle whether all cheats can be enabled in multiplayer. (Turns off leaderboard uploads)"),

                new CheckBox(MenuDisplayMode.Both, "settings:hide_chat", "HIDE CHAT")
                .WithGetter(() => Config.HideChat)
                .WithSetter((x) => Config.HideChat = x)
                .WithDescription("Toggle whether the chat stays completely hidden in multiplayer or not."),

                new CheckBox(MenuDisplayMode.Both, "settings:hide_playername", "HIDE ONLINE PLAYER NAMES")
                .WithGetter(() => Config.HidePlayerNames)
                .WithSetter((x) => Config.HidePlayerNames = x)
                .WithDescription("Toggle whether online player names are visible."),

                new CheckBox(MenuDisplayMode.Both, "settings:enable_collision", "ENABLE ONLINE COLLISIONS")
                .WithGetter(() => Config.EnableCollision)
                .WithSetter((x) => Config.EnableCollision = x)
                .WithDescription("Toggle whether or not collisions are enabled when playing online. (Turns off leaderboard uploads)"),

                new CheckBox(MenuDisplayMode.Both, "settings:disable_audio", "DISABLE ONLINE CAR AUDIO")
                .WithGetter(() => Config.DisableCarAudio)
                .WithSetter((x) => Config.DisableCarAudio = x)
                .WithDescription("Toggles whether car audio from other cars in multiplayer are on"),

                new IntegerSlider(MenuDisplayMode.Both, "settings:timeout_amount", "ADJUST LENGTH OF TIMEOUT TIME")
                .WithDefaultValue(60)
                .LimitedByRange(0, 300)
                .WithGetter(() => Config.TimeLimitAmount)
                .WithSetter((x) => Config.TimeLimitAmount = x)
                .WithDescription("Adjust the amount of time is set when a time limit occurs"),

                new CheckBox(MenuDisplayMode.Both, "setting:disable_timeout", "DISABLE TIMEOUT")
                .WithGetter(() => Config.DisableTimeout)
                .WithSetter((x) => Config.DisableTimeout = x)
                .WithDescription("Completely disables the 60 seconds timeout when one player is left"),

                new CheckBox(MenuDisplayMode.Both, "setting:disable_killgrid", "DISABLE MULTI KILLGRIDS")
                .WithGetter(() => Config.DisableMultiKillGridRender)
                .WithSetter((x) => Config.DisableMultiKillGridRender = x)
                .WithDescription("Disables the rendering of the kill grid for every player in multiplayer. The killgrid will only render for you!"),

                new FloatSlider(MenuDisplayMode.Both, "settings:outline_brightness", "ADJUST CAR OUTLINE BRIGHTNESS")
                .WithDefaultValue(1)
                .LimitedByRange(0, 1)
                .WithGetter(() => Config.OutlineBrightness)
                .WithSetter((x) => Config.OutlineBrightness = x)
                .WithDescription("Adjust the brightness of the outlines on online player cars"),

                new ListBox<int>(MenuDisplayMode.Both, "settings:level_of_detail", "MAXIMUM CAR DETAIL")
                .WithEntries(new System.Collections.Generic.Dictionary<string, int>()
                {
                    { "Ultra", 1 },
                    { "Very High", 2 },
                    { "High", 3 },
                    { "Medium", 4 },
                    { "Low", 5 },
                    { "Lowest", 6 }
                })
                .WithGetter(() => Config.MaxLevelOfDetail)
                .WithSetter((x) => Config.MaxLevelOfDetail = x)
                .WithDescription("The maximum detail online cars can have. This will lower the visual quality other cars have online. Ultra is the usual default. For an idea of how this looks, the lowest setting turns off all animations of an online car."),
            };

            Menus.AddNew(MenuDisplayMode.Both, settingsMenu, "ONLINE ADDITIONS", "Settings for the Online Additions mod");
        }

        public void OnConfigChanged(ConfigLogic config)
        {
            //If at any point collision is turned on, disable leaderboard upload
            if (config.EnableCollision || config.EnableCheatsInMultiplayer)
                UploadScore = false;
        }
    }
}



