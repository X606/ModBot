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
        [Obsolete("Passing a mod instance is no longer required")]
        public static string GetModdedSettingsStringValue(Mod mod, string id, string defaultValue)
        {
            if (mod == null)
            {
                mod = InternalUtils.GetCallerModInstance();
                if (mod == null)
                    return defaultValue;
            }

            object value = OptionsSaver.LoadSetting(mod, id);
            if (value != null && value is string)
                return value as string;

            return defaultValue;
        }

        /// <summary>
        /// Gets a <see cref="string"/> value saved in the loaded settings.
        /// </summary>
        /// <param name="id">The id of the setting.</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found.</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found.</returns>
        public static string GetModdedSettingsStringValue(string id, string defaultValue)
        {
            Mod caller = InternalUtils.GetCallerModInstance();
            if (caller == null)
                return defaultValue;

            object value = OptionsSaver.LoadSetting(caller, id);
            if (value != null && value is string)
                return value as string;

            return defaultValue;
        }

        /// <summary>
        /// Gets a <see langword="float"/> value saved in the loaded settings
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        [Obsolete("Passing a mod instance is no longer required")]
        public static float GetModdedSettingsFloatValue(Mod mod, string id, float defaultValue)
        {
            if (mod == null)
            {
                mod = InternalUtils.GetCallerModInstance();
                if (mod == null)
                    return defaultValue;
            }

            object value = OptionsSaver.LoadSetting(mod, id);
            if (value != null)
            {
                if (value is float floatValue)
                    return floatValue;

                if (value is double doubleValue)
                    return (float)doubleValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets a <see langword="float"/> value saved in the loaded settings
        /// </summary>
        /// <param name="id">The id of the setting.</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found.</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found.</returns>
        public static float GetModdedSettingsFloatValue(string id, float defaultValue)
        {
            Mod caller = InternalUtils.GetCallerModInstance();
            if (caller == null)
                return defaultValue;

            object value = OptionsSaver.LoadSetting(caller, id);
            if (value != null)
            {
                if (value is float floatValue)
                    return floatValue;

                if (value is double doubleValue)
                    return (float)doubleValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets a <see langword="int"/> value saved in the loaded settings
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        [Obsolete("Passing a mod instance is no longer required")]
        public static int GetModdedSettingsIntValue(Mod mod, string id, int defaultValue)
        {
            if (mod == null)
            {
                mod = InternalUtils.GetCallerModInstance();
                if (mod == null)
                    return defaultValue;
            }

            object value = OptionsSaver.LoadSetting(mod, id);
            if (value != null)
            {
                if (value is int intValue)
                    return intValue;

                if (value is long longValue)
                    return (int)longValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets a <see langword="int"/> value saved in the loaded settings
        /// </summary>
        /// <param name="id">The id of the setting.</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found.</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found.</returns>
        public static int GetModdedSettingsIntValue(string id, int defaultValue)
        {
            Mod caller = InternalUtils.GetCallerModInstance();
            if (caller == null)
                return defaultValue;

            object value = OptionsSaver.LoadSetting(caller, id);
            if (value != null)
            {
                if (value is int intValue)
                    return intValue;

                if (value is long longValue)
                    return (int)longValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets a <see langword="bool"/> value saved in the loaded settings
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        [Obsolete("Passing a mod instance is no longer required")]
        public static bool GetModdedSettingsBoolValue(Mod mod, string id, bool defaultValue)
        {
            if (mod == null)
            {
                mod = InternalUtils.GetCallerModInstance();
                if (mod == null)
                    return defaultValue;
            }

            object value = OptionsSaver.LoadSetting(mod, id);
            if (value != null && value is bool boolValue)
                return boolValue;

            return defaultValue;
        }

        /// <summary>
        /// Gets a <see langword="bool"/> value saved in the loaded settings
        /// </summary>
        /// <param name="id">The id of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        public static bool GetModdedSettingsBoolValue(string id, bool defaultValue)
        {
            Mod caller = InternalUtils.GetCallerModInstance();
            if (caller == null)
                return defaultValue;

            object value = OptionsSaver.LoadSetting(caller, id);
            if (value != null && value is bool boolValue)
                return boolValue;

            return defaultValue;
        }

        /// <summary>
        /// Gets a <see cref="KeyCode"/> value saved in the loaded settings
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        [Obsolete("Passing a mod instance is no longer required")]
        public static KeyCode GetModdedSettingsKeyCodeValue(Mod mod, string id, KeyCode defaultValue)
        {
            if (mod == null)
            {
                mod = InternalUtils.GetCallerModInstance();
                if (mod == null)
                    return defaultValue;
            }

            object value = OptionsSaver.LoadSetting(mod, id);
            if (value != null && value is int intValue)
                return (KeyCode)intValue;

            return defaultValue;
        }

        /// <summary>
        /// Gets a <see cref="KeyCode"/> value saved in the loaded settings
        /// </summary>
        /// <param name="id">The id of the setting</param>
        /// <param name="defaultValue">The value that should be returned if no saved setting could be found</param>
        /// <returns>The value of the setting, will be the value of defaultValue if the option could not be found</returns>
        public static KeyCode GetModdedSettingsKeyCodeValue(string id, KeyCode defaultValue)
        {
            Mod caller = InternalUtils.GetCallerModInstance();
            if (caller == null)
                return defaultValue;

            object value = OptionsSaver.LoadSetting(caller, id);
            if (value != null && value is int intValue)
                return (KeyCode)intValue;

            return defaultValue;
        }

        /// <summary>
        /// Sets a <see cref="string"/> value in the modded settings 
        /// </summary>
        /// <param name="owner">The <see cref="Mod"/> that owns this setting</param>
        /// <param name="saveID">The id of the setting</param>
        /// <param name="value">The value you want to set the setting to</param>
        /// <param name="writeToFile"><see langword="true"/> if the setting should be written to the save file immediately, <see langword="false"/> if not. It is recommended to set this to <see langword="false"/> and then calling <see cref="WriteSettingsToFile"/> if you set values often</param>
        [Obsolete("Passing a mod instance is no longer required")]
        public static void SetModdedSettingsStringValue(Mod owner, string saveID, string value, bool writeToFile = true)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (owner == null)
            {
                owner = InternalUtils.GetCallerModInstance();
                if (owner == null)
                    return;
            }

            OptionsSaver.SetSetting(owner, saveID, value, writeToFile);
        }

        /// <summary>
        /// Sets a <see cref="string"/> value in the modded settings 
        /// </summary>
        /// <param name="saveID">The id of the setting</param>
        /// <param name="value">The value you want to set the setting to</param>
        /// <param name="writeToFile"><see langword="true"/> if the setting should be written to the save file immediately, <see langword="false"/> if not. It is recommended to set this to <see langword="false"/> and then calling <see cref="WriteSettingsToFile"/> if you set values often</param>
        public static void SetModdedSettingsStringValue(string saveID, string value, bool writeToFile = true)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Mod caller = InternalUtils.GetCallerModInstance();
            if (caller == null)
                return;

            OptionsSaver.SetSetting(caller, saveID, value, writeToFile);
        }

        /// <summary>
        /// Sets a <see langword="float"/> value in the loaded settings 
        /// </summary>
        /// <param name="owner">The <see cref="Mod"/> that owns this setting</param>
        /// <param name="saveID">The id of the setting</param>
        /// <param name="value">The value you want to set the setting to</param>
        /// <param name="writeToFile"><see langword="true"/> if the setting should be written to the save file immediately, <see langword="false"/> if not. It is recommended to set this to <see langword="false"/> and then calling <see cref="WriteSettingsToFile"/> if you set values often</param>
        [Obsolete("Passing a mod instance is no longer required")]
        public static void SetModdedSettingsFloatValue(Mod owner, string saveID, float value, bool writeToFile = true)
        {
            if (owner == null)
            {
                owner = InternalUtils.GetCallerModInstance();
                if (owner == null)
                    return;
            }

            OptionsSaver.SetSetting(owner, saveID, value, writeToFile);
        }

        /// <summary>
        /// Sets a <see langword="float"/> value in the loaded settings 
        /// </summary>
        /// <param name="saveID">The id of the setting</param>
        /// <param name="value">The value you want to set the setting to</param>
        /// <param name="writeToFile"><see langword="true"/> if the setting should be written to the save file immediately, <see langword="false"/> if not. It is recommended to set this to <see langword="false"/> and then calling <see cref="WriteSettingsToFile"/> if you set values often</param>
        public static void SetModdedSettingsFloatValue(string saveID, float value, bool writeToFile = true)
        {
            Mod caller = InternalUtils.GetCallerModInstance();
            if (caller == null)
                return;

            OptionsSaver.SetSetting(caller, saveID, value, writeToFile);
        }

        /// <summary>
        /// Sets a <see langword="int"/> value in the loaded settings 
        /// </summary>
        /// <param name="owner">The <see cref="Mod"/> that owns this setting</param>
        /// <param name="saveID">The id of the setting</param>
        /// <param name="value">The value you want to set the setting to</param>
        /// <param name="writeToFile"><see langword="true"/> if the setting should be written to the save file immediately, <see langword="false"/> if not. It is recommended to set this to <see langword="false"/> and then calling <see cref="WriteSettingsToFile"/> if you set values often</param>
        [Obsolete("Passing a mod instance is no longer required")]
        public static void SetModdedSettingsIntValue(Mod owner, string saveID, int value, bool writeToFile = true)
        {
            if (owner == null)
            {
                owner = InternalUtils.GetCallerModInstance();
                if (owner == null)
                    return;
            }

            OptionsSaver.SetSetting(owner, saveID, value, writeToFile);
        }

        /// <summary>
        /// Sets a <see langword="int"/> value in the loaded settings 
        /// </summary>
        /// <param name="saveID">The id of the setting</param>
        /// <param name="value">The value you want to set the setting to</param>
        /// <param name="writeToFile"><see langword="true"/> if the setting should be written to the save file immediately, <see langword="false"/> if not. It is recommended to set this to <see langword="false"/> and then calling <see cref="WriteSettingsToFile"/> if you set values often</param>
        public static void SetModdedSettingsIntValue(string saveID, int value, bool writeToFile = true)
        {
            Mod caller = InternalUtils.GetCallerModInstance();
            if (caller == null)
                return;

            OptionsSaver.SetSetting(caller, saveID, value, writeToFile);
        }

        /// <summary>
        /// Sets a <see langword="bool"/> value in the loaded settings 
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns this setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="value">The value you want to set the setting to</param>
        /// <param name="writeToFile"><see langword="true"/> if the setting should be written to the save file immediately, <see langword="false"/> if not. It is recommended to set this to <see langword="false"/> and then calling <see cref="WriteSettingsToFile"/> if you set values often</param>
        [Obsolete("Passing a mod instance is no longer required")]
        public static void SetModdedSettingsBoolValue(Mod mod, string id, bool value, bool writeToFile = true)
        {
            if (mod == null)
            {
                mod = InternalUtils.GetCallerModInstance();
                if (mod == null)
                    return;
            }

            OptionsSaver.SetSetting(mod, id, value, writeToFile);
        }

        /// <summary>
        /// Sets a <see langword="bool"/> value in the loaded settings 
        /// </summary>
        /// <param name="id">The id of the setting</param>
        /// <param name="value">The value you want to set the setting to</param>
        /// <param name="writeToFile"><see langword="true"/> if the setting should be written to the save file immediately, <see langword="false"/> if not. It is recommended to set this to <see langword="false"/> and then calling <see cref="WriteSettingsToFile"/> if you set values often</param>
        public static void SetModdedSettingsBoolValue(string id, bool value, bool writeToFile = true)
        {
            Mod caller = InternalUtils.GetCallerModInstance();
            if (caller == null)
                return;

            OptionsSaver.SetSetting(caller, id, value, writeToFile);
        }

        /// <summary>
        /// Sets a <see cref="KeyCode"/> value in the loaded settings 
        /// </summary>
        /// <param name="mod">The <see cref="Mod"/> that owns this setting</param>
        /// <param name="id">The id of the setting</param>
        /// <param name="value">The value you want to set the setting to</param>
        /// <param name="writeToFile"><see langword="true"/> if the setting should be written to the save file immediately, <see langword="false"/> if not. It is recommended to set this to <see langword="false"/> and then calling <see cref="WriteSettingsToFile"/> if you set values often</param>
        [Obsolete("Passing a mod instance is no longer required")]
        public static void SetModdedSettingsKeyCodeValue(Mod mod, string id, KeyCode value, bool writeToFile = true)
        {
            if (mod == null)
            {
                mod = InternalUtils.GetCallerModInstance();
                if (mod == null)
                    return;
            }

            OptionsSaver.SetSetting(mod, id, (int)value, writeToFile);
        }

        /// <summary>
        /// Sets a <see cref="KeyCode"/> value in the loaded settings 
        /// </summary>
        /// <param name="id">The id of the setting</param>
        /// <param name="value">The value you want to set the setting to</param>
        /// <param name="writeToFile"><see langword="true"/> if the setting should be written to the save file immediately, <see langword="false"/> if not. It is recommended to set this to <see langword="false"/> and then calling <see cref="WriteSettingsToFile"/> if you set values often</param>
        public static void SetModdedSettingsKeyCodeValue(string id, KeyCode value, bool writeToFile = true)
        {
            Mod caller = InternalUtils.GetCallerModInstance();
            if (caller == null)
                return;

            OptionsSaver.SetSetting(caller, id, (int)value, writeToFile);
        }

        /// <summary>
        /// Saves all settings to the settings file, use this to manually write to the file if you passed <see langword="false"/> to any of the SetModdedSettings_____Value methods
        /// </summary>
        public static void WriteSettingsToFile()
        {
            OptionsSaver.SaveToFile();
        }

        /// <summary>
        /// Returns if a setting with the given owner and ID is saved
        /// </summary>
        /// <param name="owner">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="saveID">The ID of the setting</param>
        [Obsolete("Passing a mod instance is no longer required")]
        public static bool HasSetting(Mod owner, string saveID)
        {
            if (owner == null)
            {
                owner = InternalUtils.GetCallerModInstance();
                if (owner == null)
                    return false;
            }

            return OptionsSaver.HasSettingSaved(owner, saveID);
        }

        /// <summary>
        /// Returns if a setting with the given owner and ID is saved
        /// </summary>
        /// <param name="saveID">The ID of the setting</param>
        [Obsolete("Passing a mod instance is no longer required")]
        public static bool HasSetting(string saveID)
        {
            Mod caller = InternalUtils.GetCallerModInstance();
            if (caller == null)
                return false;

            return OptionsSaver.HasSettingSaved(caller, saveID);
        }
    }
}
