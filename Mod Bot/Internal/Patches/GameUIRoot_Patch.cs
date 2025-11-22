using HarmonyLib;
using ModLibrary;

namespace InternalModBot
{
    [HarmonyPatch(typeof(GameUIRoot))]
    static class GameUIRoot_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("RefreshCursorEnabled")]
        static bool GameUIRoot_RefreshCursorEnabled_Prefix()
        {
            if (ModBotUIRoot.Instance && ModBotUIRoot.Instance.AreAnyMenusOpen())
            {
                InputManager.Instance.SetCursorEnabled(true);
                return false;
            }

            ModsManager modsManager = ModsManager.Instance;
            if (modsManager && modsManager.PassOnMod != null && modsManager.PassOnMod.ShouldCursorBeEnabled())
            {
                InputManager.Instance.SetCursorEnabled(true);
                return false;
            }
            return true;
        }
    }
}
