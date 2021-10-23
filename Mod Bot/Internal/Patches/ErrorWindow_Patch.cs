using HarmonyLib;
using UnityEngine.UI;

namespace InternalModBot
{
    [HarmonyPatch(typeof(ErrorWindow))]
    static class ErrorWindow_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Show")]
        static void Show_Prefix(ErrorWindow __instance)
        {
            Text descriptionLabel = __instance.transform.GetChild(2).GetChild(1).GetComponent<Text>();
            Text titleLabel = __instance.transform.GetChild(0).GetChild(1).GetComponent<Text>();

            descriptionLabel.text = ModBotLocalizationManager.GetLocalizedModBotString("crashscreen_customdescription");
            titleLabel.text = ModBotLocalizationManager.GetLocalizedModBotString("crashscreen_customtitle");
        }
    }
}