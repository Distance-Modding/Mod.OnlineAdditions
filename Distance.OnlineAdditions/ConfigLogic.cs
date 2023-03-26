using System;
using Reactor.API.Configuration;
using UnityEngine;

namespace Distance.OnlineAdditions
{
    public class ConfigLogic : MonoBehaviour
    {
        #region Properties
        public bool EnableCollision
        {
            get { return Get<bool>("EnableCollision"); }
            set { Set("EnableCollision", value); }
        }

        public bool EnableCheatsInMultiplayer
        {
            get { return Get<bool>("EnableCheatsInMultiplayer"); }
            set { Set("EnableCheatsInMultiplayer", value); }
        }

        public bool HideChat
        {
            get { return Get<bool>("HideChat"); }
            set { Set("HideChat", value); }
        }

        public bool HidePlayerNames
        {
            get { return Get<bool>("HidePlayerNames"); }
            set { Set("HidePlayerNames", value); }
        }

        public bool DisableCarAudio
        {
            get { return Get<bool>("DisableCarAudio"); }
            set { Set("DisableCarAudio", value); }
        }

        public bool DisableTimeout
        {
            get { return Get<bool>("DisableTimeout"); }
            set { Set("DisableTimeout", value); }
        }

        public int TimeLimitAmount
        {
            get { return Get<int>("TimeLimitAmount"); }
            set { Set("TimeLimitAmount", value); }
        }
        #endregion

        internal Settings Config;

        public event Action<ConfigLogic> OnChanged;

        //Initialize Config
        private void Load()
        {
            Config = new Settings("Config");
        }

        public void Awake()
        {
            Load();
            //Setting Defaults
            Get("EnableCollision", false);
            Get("EnableCheatsInMultiplayer", false);
            Get("HideChat", false);
            Get("HidePlayerNames", false);
            Get("DisableCarAudio", false);
            Get("DisableTimeout", false);
            Get("TimeLimitAmount", 60);
            //Save settings to Config.json
            Save();
        }

        public T Get<T>(string key, T @default = default(T))
        {
            return Config.GetOrCreate(key, @default);
        }

        public void Set<T>(string key, T value)
        {
            Config[key] = value;
            Save();
        }

        public void Save()
        {
            Config?.Save();
            OnChanged?.Invoke(this);
        }
    }
}
