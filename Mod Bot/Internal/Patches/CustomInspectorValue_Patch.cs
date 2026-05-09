using HarmonyLib;

namespace InternalModBot
{
    [HarmonyPatch(typeof(CustomInspectorValue))]
    static class CustomInspectorValue_Patch
    {
        // fix losing custom values on missing object

        [HarmonyPrefix]
        [HarmonyPatch(nameof(CustomInspectorValue.setValueOnObject))]
        private static bool setValueOnObject_Prefix(CustomInspectorValue __instance)
        {
            return __instance._componentInstance && !__instance._componentInstance.GetComponent<LevelEditorMissingObject>();
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(CustomInspectorValue.OnValueSetFromInspector))]
        private static bool OnValueSetFromInspector_Prefix(CustomInspectorValue __instance)
        {
            return __instance._componentInstance && !__instance._componentInstance.GetComponent<LevelEditorMissingObject>();
        }
    }
}