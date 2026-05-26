using HarmonyLib;
using System;

namespace InternalModBot
{
    [HarmonyPatch(typeof(MultiplayerPlayerInfoState))]
    static class MultiplayerPlayerInfoState_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(MultiplayerPlayerInfoState.GetOrPrepareSafeDisplayName))]
        static void GetOrPrepareSafeDisplayName_Prefix(MultiplayerPlayerInfoState __instance, ref Action<string> onSafeDisplayNameReceived)
        {
            if (onSafeDisplayNameReceived != null)
            {
                // Create new callback delegate that invokes the original with the name given by MultiplayerPlayerNameManager
                Action<string> callbackCopy = onSafeDisplayNameReceived;
                onSafeDisplayNameReceived = delegate (string safeDisplayName)
                {
                    string name;
                    if (MultiplayerPlayerNameManager.Instance != null)
                    {
                        name = MultiplayerPlayerNameManager.Instance.GetCompleteNameForPlayer(__instance, safeDisplayName);
                    }
                    else
                    {
                        name = safeDisplayName;
                    }

                    callbackCopy(name);
                };
            }
        }
    }
}