using HarmonyLib;
using ModLibrary;

namespace InternalModBot
{
    [HarmonyPatch(typeof(LevelEditorUI))]
    static class LevelEditorUI_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("IsAnyDialogueOpen")]
        static bool IsAnyDialogueOpen_Prefix(ref bool __result)
        {
            if (ModBotUIRoot.Instance.AreAnyMenusOpen())
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
            if (ModBotUIRoot.Instance.AreAnyMenusOpen())
            {
                __result = true;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
