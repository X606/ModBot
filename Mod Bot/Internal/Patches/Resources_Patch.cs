using HarmonyLib;
using InternalModBot.LevelEditor;
using System.Reflection;
using UnityEngine;

namespace InternalModBot
{
    [HarmonyPatch]
    static class Resources_Load_Patch
    {
        /// <summary>
        /// Finds <see cref="Resources.Load(string)"/>
        /// </summary>
        /// <returns></returns>
        static MethodBase TargetMethod()
        {
            MethodInfo[] methods = typeof(Resources).GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (MethodInfo method in methods)
            {
                if (!method.IsGenericMethod && method.Name == nameof(Resources.Load))
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
                    {
                        return method;
                    }
                }
            }
            return null;
        }

        [HarmonyPostfix]
        static Object Load_Postfix(Object __result, string path)
        {
            Object overrideResource = CustomLevelEditorManager.GetResourceObject(path);

            if (overrideResource != null)
            {
                return overrideResource;
            }

            if (ModsManager.Instance != null)
            {
                overrideResource = ModsManager.Instance.PassOnMod.OnResourcesLoad(path);
                if (overrideResource != null)
                {
                    return overrideResource;
                }
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