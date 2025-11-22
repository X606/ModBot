using HarmonyLib;
using UnityEngine;

namespace InternalModBot
{
    [HarmonyPatch(typeof(ResourceRequest))]
    static class ResourceRequest_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("asset", MethodType.Getter)]
        static UnityEngine.Object asset_Getter_Postfix(UnityEngine.Object __result, string ___m_Path)
        {
            UnityEngine.Object overrideResource;

            if (ModsManager.Instance != null)
            {
                overrideResource = ModsManager.Instance.PassOnMod.OnResourcesLoad(___m_Path);
                if (overrideResource != null)
                    return overrideResource;
            }

            return __result;
        }
    }
}