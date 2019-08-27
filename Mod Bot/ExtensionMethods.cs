using System;
using InternalModBot;

namespace ModLibrary
{
    /// <summary>
    /// Dont call these methods directly from here
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Instead of having to filter the object array yourself you can use this method to get the object at a specific index in a much safer way
        /// </summary>
        /// <typeparam name="T">The type of the object at the index</typeparam>
        /// <param name="moddedObject"></param>
        /// <param name="index">The index of the object you want to get</param>
        /// <returns>The object you asked for</returns>
        public static T GetObject<T>(this ModdedObject moddedObject, int index) where T : UnityEngine.Object
        {
            if (index < 0 || index >= moddedObject.objects.Count)
                return null;

            if (!(moddedObject.objects[index] is T))
            {
                throw new InvalidCastException("Object at index " + index + " was not of type " + typeof(T).ToString());
            }

            return moddedObject.objects[index] as T;
        }

        /// <summary>
        /// Sets the icon of the upgrade to a image from a url, this needs a internet connection (NOTE: this has a cache so if you want to change picture you might want to remove the cache in the mods directory)
        /// </summary>
        /// <param name="upgradeDescription"></param>
        /// <param name="url">the url of the image you want to set the object to</param>
        public static void SetIconFromURL(this UpgradeDescription upgradeDescription, string url)
        {
            UpgradeIconDownloader.Instance.AddUpgradeIcon(upgradeDescription, url);
        }
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
        /// <summary>
        /// Returns true of the mod is enbaled, false if its disabled
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static bool IsModEnabled(this Mod mod)
        {
            bool? isModDeactivated = ModsManager.Instance.IsModDeactivated(mod);

            if (!isModDeactivated.HasValue)
                throw new Exception("Mod \"" + mod.GetModName() + "\" with unique id \"" + mod.GetUniqueID() + "\" not found in modsmanager list of mods!");

            return !isModDeactivated.Value;

        }

    }
}
