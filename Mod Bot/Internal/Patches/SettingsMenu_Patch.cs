using HarmonyLib;
using UnityEngine;

namespace InternalModBot
{
    [HarmonyPatch(typeof(SettingsMenu))]
    static class SettingsMenu_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(SettingsMenu.populateSettings))]
        static void populateSettings_Postfix()
        {
            ModBotSettingsManager.CreateSettingsWindow(new ModBotSettingsManager.ModBotSettingsBuilder(ModBotSettingsManager.GetSettingsPageModdedObject()));
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(SettingsMenu.Hide))]
        static void Hide_Postfix()
        {
            PlayerPrefs.Save();
        }
    }
}