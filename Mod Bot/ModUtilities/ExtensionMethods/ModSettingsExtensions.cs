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
        /// Gets a <see cref="string"/> value saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The <see cref="Mod"/> that owns the setting</param>
        /// <param name="name">The name of the setting</param>
        /// <returns>The value of the setting, will be <see langword="null"/> if the option could not be found</returns>
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
        public static int? GetModdedSettingsIntValue(this SettingsManager me, Mod mod, string name)
        {
            return OptionsSaver.LoadInt(mod, name);
        }

    }
}
