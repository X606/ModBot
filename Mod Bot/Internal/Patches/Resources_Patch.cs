using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace InternalModBot
{
    [HarmonyPatch]
    static class Resources_Load_Patch
    {
        /// <summary>
        /// Finds <see cref="Resources.Load(string, System.Type)"/>
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
                    if (parameters.Length == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(System.Type))
                    {
                        return method;
                    }
                }
            }
            return null;
        }

        [HarmonyPrefix]
        static bool Load_Prefix(ref Object __result, string path, System.Type systemTypeInstance)
        {
            Object overrideResource = OverrideResourceManager.GetObjectOverride(path, systemTypeInstance);
            if (overrideResource != null)
            {
                __result = overrideResource;
                return false;
            }
            return true;
        }
    }
}