using HarmonyLib;
using UnityEngine;

namespace InternalModBot
{
    [HarmonyPatch(typeof(ObjectPlacedInLevel))]
    static class ObjectPlacedInLevel_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ObjectPlacedInLevel.Initialize))]
        static void Initialize_Prefix(ObjectPlacedInLevel __instance, Transform levelRoot)
        {
            ModsManager.Instance.PassOnMod.OnObjectPlacedInLevelInitialized(__instance, levelRoot);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ObjectPlacedInLevel.Initialize))]
        static void Initialize_Postfix(ObjectPlacedInLevel __instance, Transform levelRoot)
        {
            ModsManager.Instance.PassOnMod.AfterObjectPlacedInLevelInitialized(__instance, levelRoot);
        }
    }
}