using HarmonyLib;

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
            ModBotUIRoot ui = ModBotUIRoot._instance;
            return ui.Generic2ButtonDialogeUI.UIRoot.activeInHierarchy ||
                ui.ModBotSignInUI.WindowObject.activeInHierarchy ||
                ui.DownloadWindow.gameObject.activeInHierarchy ||
                ui.ModOptionsWindow.WindowObject.activeInHierarchy ||
                ui.ModList.gameObject.activeInHierarchy;
        }
    }
}
