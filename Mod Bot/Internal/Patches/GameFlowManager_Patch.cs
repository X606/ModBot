using HarmonyLib;
using System.Collections.Generic;

namespace InternalModBot
{
    [HarmonyPatch(typeof(GameFlowManager))]
    static class GameFlowManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameFlowManager.Start))]
        static void Start_Prefix()
        {
            StartupManager.OnStartUp();
        }
    }
}