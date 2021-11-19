using HarmonyLib;
using ModLibrary;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace InternalModBot
{
    [HarmonyPatch]
    static class Resources_Load_Patch
    {
        static MethodBase TargetMethod()
        {
            return typeof(Resources).GetMethods(BindingFlags.Public | BindingFlags.Static).Single((m) => m.Name == "Load" && m.ReturnType == typeof(UnityEngine.Object) && m.GetMethodBody() != null);
        }

        [HarmonyPostfix]
        static UnityEngine.Object Load_Postfix(UnityEngine.Object __result, string path)
        {
            UnityEngine.Object overrideResource;

#if MODDED_LEVEL_OBJECTS
            overrideResource = LevelEditorObjectAdder.GetObjectData(path);
            if (overrideResource != null)
                return overrideResource;
#endif

            if (ModsManager.Instance != null)
            {
                overrideResource = ModsManager.Instance.PassOnMod.OnResourcesLoad(path);
                if (overrideResource != null)
                    return overrideResource;
            }

            return __result;
        }

        /* Harmony REALLY does not like generic methods, I have given up on trying to make this work, it will continue being in Injector.exe
        [ExtraInjectionData(Namespace = "UnityEngine", HasGenericParameters = true, GenericParameterTypes = new Type[] { typeof(UnityEngine.Object) }, ArgumentTypes = new Type[] { typeof(string) })]
        public static UnityEngine.Object Resources_Load_Postfix_T(UnityEngine.Object __result, string path)
        {
            UnityEngine.Object moddedResource = LevelEditorObjectAdder.GetObjectData(path);
            if (moddedResource != null)
                return moddedResource;

            if (ModsManager.Instance != null)
            {
                moddedResource = ModsManager.Instance.PassOnMod.OnResourcesLoad(path);
                if (moddedResource != null)
                    return moddedResource;
            }

            return __result;
        }
        */
    }
}