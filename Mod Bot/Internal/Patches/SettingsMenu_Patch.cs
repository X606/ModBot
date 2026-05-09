using HarmonyLib;
using UnityEngine;

namespace InternalModBot
{
    [HarmonyPatch(typeof(SettingsMenu))]
    static class SettingsMenu_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("populateSettings")]
        static void populateSettings_Postfix()
        {
            ModBotSettingsManager.CreateSettingsWindow(new ModBotSettingsManager.ModBotSettingsBuilder(ModBotSettingsManager.GetSettingsPageModdedObject()));
        }
    }
}