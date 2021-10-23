using HarmonyLib;
using UnityEngine;

namespace InternalModBot
{
    [HarmonyPatch(typeof(ErrorManager))]
    static class ErrorManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("SendDataToLoggly")]
        static void SendDataToLoggly_Prefix(WWWForm form)
        {
            // Allow game developers to filter out crash logs on modded clients
            form.AddField("IsModdedClient", "true");
        }
    }
}