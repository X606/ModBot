using HarmonyLib;
using ModLibrary;

namespace InternalModBot
{
    [HarmonyPatch(typeof(UpgradeDescription))]
    static class UpgradeDescription_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(UpgradeDescription.GetAngleOffset))]
        static float GetAngleOffset_Postfix(float __result, UpgradeDescription __instance)
        {
            return UpgradePagesManager.IsCurrentlyShowingModdedUpgrades ? UpgradePagesManager.GetUpgradeAngle(__instance.UpgradeType, __instance.Level) : __result;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(UpgradeDescription.IsUpgradeCurrentlyVisible))]
        static bool IsUpgradeCurrentlyVisible_Postfix(bool __result, UpgradeDescription __instance)
        {
            return UpgradePagesManager.IsCurrentlyShowingModdedUpgrades ? UpgradePagesManager.IsUpgradeOnCurrentPage(__instance.UpgradeType, __instance.Level) : __result && !__instance.IsModdedUpgradeType();
        }
    }
}