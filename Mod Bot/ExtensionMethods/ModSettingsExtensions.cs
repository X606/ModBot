using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternalModBot;

namespace ModLibrary
{
    /// <summary>
    /// Extension methods for mod settings handling
    /// </summary>
    public static class ModSettingsExtensions
    {
        /// <summary>
        /// Gets the settings saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The mod you opend the window as</param>
        /// <param name="name">the name you gave the setting</param>
        /// <returns></returns>
        public static string GetModdedSettingsStringValue(this SettingsManager me, Mod mod, string name)
        {
            string result = OptionsSaver.LoadString(mod, name);
            return result;
        }

        /// <summary>
        /// Gets the settings saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The mod you opend the window as</param>
        /// <param name="name">the name you gave the setting</param>
        /// <returns></returns>
        public static bool? GetModdedSettingsBoolValue(this SettingsManager me, Mod mod, string name)
        {
            bool? result = OptionsSaver.LoadBool(mod, name);
            return result;
        }

        /// <summary>
        /// Gets the settings saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The mod you opend the window as</param>
        /// <param name="name">the name you gave the setting</param>
        /// <returns></returns>
        public static float? GetModdedSettingsFloatValue(this SettingsManager me, Mod mod, string name)
        {
            float? result = OptionsSaver.LoadFloat(mod, name);
            return result;
        }

        /// <summary>
        /// Gets the settings saved in the loaded settings
        /// </summary>
        /// <param name="me"></param>
        /// <param name="mod">The mod you opend the window as</param>
        /// <param name="name">the name you gave the setting</param>
        /// <returns></returns>
        public static int? GetModdedSettingsIntValue(this SettingsManager me, Mod mod, string name)
        {
            int? result = OptionsSaver.LoadInt(mod, name);
            return result;
        }
    }
}
