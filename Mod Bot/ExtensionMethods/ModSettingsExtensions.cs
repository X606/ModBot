using InternalModBot;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace ModLibrary
{
    /// <summary>
    /// Extension methods for mod settings handling
    /// </summary>
    public static class ModSettingsExtensions
    {
        /// <summary>
        /// Gets a <see cref="string"/> saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="name">The name of the setting</param>
        /// <returns>The value of the setting, will be <see langword="null"/> if the option could not be found</returns>
        public static string GetModdedSettingsStringValue(this SettingsManager me, Mod mod, string name)
        {
            string result = OptionsSaver.LoadString(mod, name);
            return result;
        }

        /// <summary>
        /// Gets a <see cref="bool"/> saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="name">The name of the setting</param>
        /// <returns>The value of the setting, will be <see langword="null"/> if the option could not be found</returns>
        public static bool? GetModdedSettingsBoolValue(this SettingsManager me, Mod mod, string name)
        {
            bool? result = OptionsSaver.LoadBool(mod, name);
            return result;
        }

        /// <summary>
        /// Gets a <see cref="float"/> saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="name">The name of the setting</param>
        /// <returns>The value of the setting, will be <see langword="null"/> if the option could not be found</returns>
        public static float? GetModdedSettingsFloatValue(this SettingsManager me, Mod mod, string name)
        {
            float? result = OptionsSaver.LoadFloat(mod, name);
            return result;
        }

        /// <summary>
        /// Gets a <see cref="int"/> saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="name">The name of the setting</param>
        /// <returns>The value of the setting, will be <see langword="null"/> if the option could not be found</returns>
        public static int? GetModdedSettingsIntValue(this SettingsManager me, Mod mod, string name)
        {
            int? result = OptionsSaver.LoadInt(mod, name);
            return result;
        }
    }
}
