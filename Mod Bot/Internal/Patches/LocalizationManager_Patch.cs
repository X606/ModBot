using HarmonyLib;
using System.Collections.Generic;

namespace InternalModBot
{
    [HarmonyPatch(typeof(LocalizationManager))]
    static class LocalizationManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LocalizationManager), nameof(LocalizationManager.populateDictionaryForCurrentLanguage))]
        static void populateDictionaryForCurrentLanguage_Postfix(LocalizationManager __instance, Dictionary<string, string> ____translatedStringsDictionary)
        {
            ModBotLocalizationManager.OnLocalizationDictionaryUpdated();
            ModBotLocalizationManager.AddAllLocalizationStringsToDictionary(____translatedStringsDictionary);

            if (ModsManager.Instance != null && ModsManager.Instance.PassOnMod != null)
            {
                ModsManager.Instance.PassOnMod.OnLanguageChanged(ModBotLocalizationManager.CurrentLanguageID, ____translatedStringsDictionary);
            }
        }
    }
}