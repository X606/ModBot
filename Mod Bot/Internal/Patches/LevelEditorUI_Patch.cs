using HarmonyLib;
using ModLibrary;
using System.Collections.Generic;
using System.Reflection;

namespace InternalModBot
{
    [HarmonyPatch(typeof(LevelEditorUI))]
    static class LevelEditorUI_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("IsAnyDialogueOpen")]
        static bool IsAnyDialogueOpen_Prefix(ref bool __result)
        {
            if (areAnyModdedDialogsOpen())
            {
                __result = true;
                return false;
            }
            else
            {
                return true;
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch("AreAnyDialogsOpen")]
        static bool AreAnyDialogsOpen_Prefix(ref bool __result)
        {
            if (areAnyModdedDialogsOpen())
            {
                __result = true;
                return false;
            }
            else
            {
                return true;
            }
        }
        private static bool areAnyModdedDialogsOpen()
        {
            var ui = ModBotUIRoot._instance;
            return ui.Generic2ButtonDialogeUI.UIRoot.activeSelf ||
                ui.ModBotSignInUI.WindowObject.activeSelf ||
                ui.ModCreationWindow.TheGameObject.activeSelf ||
                ui.ModDownloadPage.WindowObject.activeSelf ||
                ui.ModOptionsWindow.WindowObject.activeSelf ||
                ui.ModsWindow.WindowObject.activeSelf;
        }
    }
}
