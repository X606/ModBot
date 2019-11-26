using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternalModBot;
using UnityEngine;

namespace ModLibrary
{
    /// <summary>
    /// Used to save and set modded settings
    /// </summary>
    public static class ModdedSettings
    {
        /// <summary>
        /// Gets a <see cref="string"/> value saved in the loaded settings
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        public static string GetModdedSettingsStringValue(Mod mod, string id, string defaultValue)
        {
            string value = OptionsSaver.LoadString(mod, id);
            if(value == null)
                return defaultValue;

            return value;
        }

        /// <summary>
        /// Gets a <see langword="float"/> value saved in the loaded settings
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        public static float GetModdedSettingsFloatValue(Mod mod, string id, float defaultValue)
        {
            float? value = OptionsSaver.LoadFloat(mod, id);
            if(!value.HasValue)
                return defaultValue;

            return value.Value;
        }

        /// <summary>
        /// Gets a <see langword="int"/> value saved in the loaded settings
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        public static int GetModdedSettingsIntValue(Mod mod, string id, int defaultValue)
        {
            int? value = OptionsSaver.LoadInt(mod, id);
            if(!value.HasValue)
                return defaultValue;

            return value.Value;
        }

        /// <summary>
        /// Gets a <see langword="bool"/> value saved in the loaded settings
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        public static bool GetModdedSettingsBoolValue(Mod mod, string id, bool defaultValue)
        {
            bool? value = OptionsSaver.LoadBool(mod, id);
            if(!value.HasValue)
                return defaultValue;

            return value.Value;
        }

        /// <summary>
        /// Gets a <see cref="KeyCode"/> value saved in the loaded settings
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        public static KeyCode GetModdedSettingsKeyCodeValue(Mod mod, string id, KeyCode defaultValue)
        {
            int? value = OptionsSaver.LoadInt(mod, id);
            if(!value.HasValue)
                return defaultValue;

            return (KeyCode)value.Value;
        }


        /// <summary>
        /// Sets a <see cref="string"/> value in the loaded settings 
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns this setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="value">The value you want to set the setting to</param>
        public static void SetModdedSettingsStringValue(Mod mod, string id, string value)
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value) + " cannot be null");

            OptionsSaver.SaveString(mod, id, value);
        }

        /// <summary>
        /// Sets a <see langword="float"/> value in the loaded settings 
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns this setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="value">The value you want to set the setting to</param>
        public static void SetModdedSettingsFloatValue(Mod mod, string id, float value)
        {
            OptionsSaver.SaveFloat(mod, id, value);
        }

        /// <summary>
        /// Sets a <see langword="int"/> value in the loaded settings 
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns this setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="value">The value you want to set the setting to</param>
        public static void SetModdedSettingsIntValue(Mod mod, string id, int value)
        {
            OptionsSaver.SaveInt(mod, id, value);
        }

        /// <summary>
        /// Sets a <see langword="bool"/> value in the loaded settings 
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns this setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="value">The value you want to set the setting to</param>
        public static void SetModdedSettingsBoolValue(Mod mod, string id, bool value)
        {
            OptionsSaver.SaveBool(mod, id, value);
        }

        /// <summary>
        /// Sets a <see cref="KeyCode"/> value in the loaded settings 
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns this setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="value">The value you want to set the setting to</param>
        public static void SetModdedSettingsKeyCodeValue(Mod mod, string id, KeyCode value)
        {
            OptionsSaver.SaveInt(mod, id, (int)value);
        }
    }
}
