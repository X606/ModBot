/*using HarmonyLib;
using InternalModBot.LevelEditor;
using System.Collections.Generic;
using UnityEngine;

namespace InternalModBot
{
    [HarmonyPatch(typeof(LevelEditorBaseTrigger))]
    static class LevelEditorBaseTrigger_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(LevelEditorBaseTrigger.createEnableAILetterDisaplayIfNull))]
        private static bool createEnableAILetterDisaplayIfNull_Prefix(LevelEditorBaseTrigger __instance)
        {
            return __instance.EnableAILetterDisplayPrefab;
        }
    }
}*/