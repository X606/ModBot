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

            __instance._levelObjects.AddRange(list);
            __instance._visibleLevelObjects.AddRange(list);
        }
    }
}