using HarmonyLib;

namespace InternalModBot
{
    [HarmonyPatch(typeof(FirstPersonMover))]
    static class FirstPersonMover_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("RefreshUpgrades")]
        static void RefreshUpgrades_Prefix(FirstPersonMover __instance)
        {
            if (__instance == null || __instance.gameObject == null || !__instance.IsAlive() || __instance.GetCharacterModel() == null)
                return;

            UpgradeCollection upgrade = __instance.GetComponent<UpgradeCollection>();
            ModsManager.Instance.PassOnMod.OnUpgradesRefreshed(__instance, upgrade);
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateCharacterModel")]
        static void CreateCharacterModel_Postfix(FirstPersonMover __instance)
        {
            ModsManager.Instance.PassOnMod.OnCharacterModelCreated(__instance);
        }
    }
}