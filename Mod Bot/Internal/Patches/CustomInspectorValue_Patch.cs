using HarmonyLib;
using ModLibrary.LevelEditor;

namespace InternalModBot
{
    [HarmonyPatch(typeof(CustomInspectorValue))]
    static class CustomInspectorValue_Patch
    {
        // fix losing custom values on missing object

        [HarmonyPrefix]
        [HarmonyPatch(nameof(CustomInspectorValue.setValueOnObject))]
        static bool setValueOnObject_Prefix(CustomInspectorValue __instance)
        {
            return __instance._componentInstance && !__instance._componentInstance.GetComponent<LevelEditorMissingObject>();
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(CustomInspectorValue.OnValueSetFromInspector))]
        static bool OnValueSetFromInspector_Prefix(CustomInspectorValue __instance)
        {
            return __instance._componentInstance && !__instance._componentInstance.GetComponent<LevelEditorMissingObject>();
        }
    }
}