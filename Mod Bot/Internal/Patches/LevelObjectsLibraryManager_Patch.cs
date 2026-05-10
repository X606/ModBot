using HarmonyLib;
using InternalModBot.LevelEditor;
using System.Collections.Generic;

namespace InternalModBot
{
    [HarmonyPatch(typeof(LevelObjectsLibraryManager))]
    static class LevelObjectsLibraryManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(LevelObjectsLibraryManager.Initialize))]
        static void Initialize_Postfix(LevelObjectsLibraryManager __instance)
        {
            List<LevelObjectEntry> list = CustomLevelEditorManager.GetLevelObjectEntries();

            // to be extra sure custom entries dont get duplicated by some mod
            CustomLevelEditorManager.AddMissingEntries(list, __instance._levelObjects);
            CustomLevelEditorManager.AddMissingEntries(list, __instance._visibleLevelObjects);
        }
    }
}