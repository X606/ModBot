using HarmonyLib;

namespace InternalModBot
{
    [HarmonyPatch(typeof(EscMenu))]
    static class EscMenu_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(EscMenu.Show))]
        static bool Show_Prefix()
        {
            return !GameUIRoot.Instance._isEscMenuDisabled;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(EscMenu.Hide))]
        static bool Hide_Prefix()
        {
            return !GameUIRoot.Instance._isEscMenuDisabled;
        }
    }
}