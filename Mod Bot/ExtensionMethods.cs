using System;
using InternalModBot;

namespace ModLibrary
{
    public static class ExtensionMethods
    {
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

        public static void SetIconFromURL(this UpgradeDescription upgradeDescription, string url)
        {
            UpgradeIconDownloader.Instance.AddUpgradeIcon(upgradeDescription, url);
        }
        public static string GetModdedSettingsStringValue(this SettingsManager me, Mod mod, string name)
        {
            string result = OptionsSaver.LoadString(mod, name);
            return result;
        }
        public static bool? GetModdedSettingsBoolValue(this SettingsManager me, Mod mod, string name)
        {
            bool? result = OptionsSaver.LoadBool(mod, name);
            return result;
        }
        public static float? GetModdedSettingsFloatValue(this SettingsManager me, Mod mod, string name)
        {
            float? result = OptionsSaver.LoadFloat(mod, name);
            return result;
        }
        public static int? GetModdedSettingsIntValue(this SettingsManager me, Mod mod, string name)
        {
            int? result = OptionsSaver.LoadInt(mod, name);
            return result;
        }
        public static bool IsModEnabled(this Mod mod)
        {
            bool? isModDeactivated = ModsManager.Instance.IsModDeactivated(mod);

            if (!isModDeactivated.HasValue)
                throw new Exception("Mod \"" + mod.GetModName() + "\" with unique id \"" + mod.GetUniqueID() + "\" not found in modsmanager list of mods!");

            return !isModDeactivated.Value;

        }

    }
}
