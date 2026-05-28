using HarmonyLib;
using UnityEngine;

namespace InternalModBot
{
    [HarmonyPatch(typeof(ResourceRequest))]
    static class ResourceRequest_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ResourceRequest.asset), MethodType.Getter)]
        static bool asset_Getter_Prefix(UnityEngine.Object __result, string ___m_Path, System.Type ___m_Type)
        {
            Object overrideResource = OverrideResourceManager.GetObjectOverride(___m_Path, ___m_Type);
            if (overrideResource != null)
            {
                __result = overrideResource;
                return false;
            }
            return true;
        }
    }
}