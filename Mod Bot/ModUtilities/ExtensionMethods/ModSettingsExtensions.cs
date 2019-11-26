using InternalModBot;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using UnityEngine;

namespace ModLibrary
{
    /// <summary>
    /// Extension methods for mod settings handling
    /// </summary>
    public static class ModSettingsExtensions
    {
        /// <summary>
        /// Gets a <see cref="string"/> value saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        [Obsolete("Use the ModdedSettings class instead")]
        public static string GetModdedSettingsStringValue(this SettingsManager me, Mod mod, string name, string defaultValue)
        {
            string value = OptionsSaver.LoadString(mod, name);
            if(value == null)
                return defaultValue;

            return value;
        }
        /// <summary>
        /// Gets a <see cref="bool"/> value saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        [Obsolete("Use the ModdedSettings class instead")]
        public static bool GetModdedSettingsBoolValue(this SettingsManager me, Mod mod, string name, bool defaultValue)
        {
            bool? value = OptionsSaver.LoadBool(mod, name);
            if(!value.HasValue)
                return defaultValue;

            return value.Value;
        }
        /// <summary>
        /// Gets a <see cref="float"/> value saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        [Obsolete("Use the ModdedSettings class instead")]
        public static float GetModdedSettingsFloatValue(this SettingsManager me, Mod mod, string name, float defaultValue)
        {
            float? value = OptionsSaver.LoadFloat(mod, name);
            if(!value.HasValue)
                return defaultValue;

            return value.Value;
        }
        /// <summary>
        /// Gets an <see cref="int"/> value saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        [Obsolete("Use the ModdedSettings class instead")]
        public static int GetModdedSettingsIntValue(this SettingsManager me, Mod mod, string name, int defaultValue)
        {
            int? value = OptionsSaver.LoadInt(mod, name);
            if(!value.HasValue)
                return defaultValue;

            return value.Value;
        }
        /// <summary>
        /// Gets a <see cref="KeyCode"/> value saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        [Obsolete("Use the ModdedSettings class instead")]
        public static KeyCode GetModdedSettingsKeyCodeValue(this SettingsManager me, Mod mod, string name, KeyCode defaultValue)
        {
            int? value = OptionsSaver.LoadInt(mod, name);
            if(!value.HasValue)
                return defaultValue;

            return (KeyCode)value.Value;
        }

        /// <summary>
        /// Gets a <see cref="string"/> value saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="name">The name of the setting</param>
        /// <returns>The value of the setting, will be <see langword="null"/> if the option could not be found</returns>
        [Obsolete("Use the ModdedSettings class instead")]
        public static string GetModdedSettingsStringValue(this SettingsManager me, Mod mod, string name)
        {
            return OptionsSaver.LoadString(mod, name);
        }

        /// <summary>
        /// Gets a <see cref="bool"/> value saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="name">The name of the setting</param>
        /// <returns>The value of the setting, will be <see langword="null"/> if the option could not be found</returns>
        [Obsolete("Use the ModdedSettings class instead")]
        public static bool? GetModdedSettingsBoolValue(this SettingsManager me, Mod mod, string name)
        {
            return OptionsSaver.LoadBool(mod, name);
        }

        /// <summary>
        /// Gets a <see cref="float"/> value saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="name">The name of the setting</param>
        /// <returns>The value of the setting, will be <see langword="null"/> if the option could not be found</returns>
        [Obsolete("Use the ModdedSettings class instead")]
        public static float? GetModdedSettingsFloatValue(this SettingsManager me, Mod mod, string name)
        {
            return OptionsSaver.LoadFloat(mod, name);
        }

        /// <summary>
        /// Gets an <see cref="int"/> value saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="name">The name of the setting</param>
        /// <returns>The value of the setting, will be <see langword="null"/> if the option could not be found</returns>
        [Obsolete("Use the ModdedSettings class instead")]
        public static int? GetModdedSettingsIntValue(this SettingsManager me, Mod mod, string name)
        {
            return OptionsSaver.LoadInt(mod, name);
        }

    }
}
