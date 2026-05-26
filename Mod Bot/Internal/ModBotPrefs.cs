using System;
using System.Collections.Generic;
using UnityEngine;

namespace InternalModBot
{
    /// <summary>
    /// Handles Mod-Bot settings
    /// </summary>
    internal static class ModBotPrefs
    {
        /// <summary>
        /// All the input options in mod-bot
        /// </summary>
        public static readonly InputOption[] InputOptions = new InputOption[]
        {
            new InputOption(ModBotInputType.OpenConsole, KeyCode.F1, "Open Console Key"),
            new InputOption(ModBotInputType.ToggleFPSLabel, KeyCode.F3, "Toggle FPS Label Key")
        };

        private static bool s_hideCustomTags;
        public static bool HideCustomTags
        {
            get => s_hideCustomTags;
            set
            {
                PlayerPrefs.SetInt("ModBot_HideCustomTags", value ? 1 : 0);
                s_hideCustomTags = value;
            }
        }

        private static bool s_showMaxFPS;
        public static bool ShowMaxFPS
        {
            get => s_showMaxFPS;
            set
            {
                PlayerPrefs.SetInt("ModBot_ShowMaxFPS", value ? 1 : 0);
                s_showMaxFPS = value;
            }
        }

        private static Dictionary<ModBotInputType, InputOption> s_cachedDictionary = null;

        private static bool s_hasInitialized;

        internal static void Initialize()
        {
            if (s_hasInitialized) return;
            s_hasInitialized = true;

            s_cachedDictionary = new Dictionary<ModBotInputType, InputOption>();
            foreach (InputOption inputOption in InputOptions)
            {
                s_cachedDictionary.Add(inputOption.Type, inputOption);
            }

            s_hideCustomTags = PlayerPrefs.GetInt("ModBot_HideCustomTags", 0) == 0 ? false : true;
            s_showMaxFPS = PlayerPrefs.GetInt("ModBot_ShowMaxFPS", 0) == 0 ? false : true;
        }

        /// <summary>
        /// Gets the key associated with a specifc <see cref="ModBotInputType"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static KeyCode GetKeyCode(ModBotInputType type)
        {
            if (s_cachedDictionary.TryGetValue(type, out InputOption value))
            {
                return value.Key;
            }

            throw new Exception(type.ToString() + " not connected to anything");
        }

        /// <summary>
        /// Class for holding information about a input
        /// </summary>
        public class InputOption
        {
            /// <summary>
            /// <see cref="ModBotInputType"/> we want this input to be
            /// </summary>
            public ModBotInputType Type;
            /// <summary>
            /// The defualt key for the input
            /// </summary>
            public KeyCode DefaultKey;
            /// <summary>
            /// The display name for the key
            /// </summary>
            public string DisplayName;

            KeyCode? _value = null;

            /// <summary>
            /// Creates a new <see cref="InputOption"/>
            /// </summary>
            /// <param name="type"></param>
            /// <param name="defaultKey"></param>
            /// <param name="displayName"></param>
            public InputOption(ModBotInputType type, KeyCode defaultKey, string displayName)
            {
                Type = type;
                DefaultKey = defaultKey;
                DisplayName = displayName;
            }

            /// <summary>
            /// Gets or sets the key we want to associate with this input
            /// </summary>
            public KeyCode Key
            {
                get
                {
                    if (_value != null)
                        return _value.Value;

                    KeyCode keyCode = (KeyCode)PlayerPrefs.GetInt("ModBot_Keys_" + Type.ToString(), (int)DefaultKey);
                    _value = keyCode;
                    return keyCode;
                }
                set
                {
                    _value = value;
                    PlayerPrefs.SetInt("ModBot_Keys_" + Type.ToString(), (int)value);
                }
            }
        }
    }

    /// <summary>
    /// Different actions we want to accociate keys with
    /// </summary>
    public enum ModBotInputType
    {
        /// <summary>
        /// The key for opening the console
        /// </summary>
        OpenConsole,
        /// <summary>
        /// The key for toggling the fps label in the corner
        /// </summary>
        ToggleFPSLabel
    }
}
