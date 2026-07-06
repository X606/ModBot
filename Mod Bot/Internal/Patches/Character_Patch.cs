using HarmonyLib;

namespace InternalModBot
{
    [HarmonyPatch(typeof(Character))]
    static class Character_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Character.Start))]
        static void Start_Prefix(Character __instance)
        {
            ModsManager.Instance.PassOnMod.OnCharacterSpawned(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Character.Update))]
        static void Update_Prefix(Character __instance)
        {
            ModsManager.Instance.PassOnMod.OnCharacterUpdate(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Character.onDeath))]
        static void onDeath_Prefix(Character __instance, Character killer, DamageSourceType damageSourceType, int attackID)
        {
            ModsManager.Instance.PassOnMod.OnCharacterKilled(__instance, killer, damageSourceType, attackID);
        }
    }
}