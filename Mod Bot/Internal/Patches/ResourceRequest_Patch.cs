using HarmonyLib;
using ModLibrary;
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
            UnityEngine.Object moddedResource = LevelEditorObjectAdder.GetObjectData(___m_Path);
            if (moddedResource != null)
                return moddedResource;

            if (ModsManager.Instance != null)
            {
                moddedResource = ModsManager.Instance.PassOnMod.OnResourcesLoad(___m_Path);
                if (moddedResource != null)
                    return moddedResource;
            }

            return __result;
        }
    }
}