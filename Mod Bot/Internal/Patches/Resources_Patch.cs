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

        [HarmonyPostfix]
        static Object Load_Postfix(Object __result, string path, System.Type systemTypeInstance)
        {
            Object overrideResource;

            // get transform or preview image of level editor custom object
            if (systemTypeInstance == typeof(Transform))
            {
                overrideResource = CustomLevelEditorManager.GetTransform(path);
            }
            else if (systemTypeInstance == typeof(Texture2D))
            {
                overrideResource = CustomLevelEditorManager.GetTexture(path);
            }
            else
            {
                overrideResource = null;
            }

            if (overrideResource != null)
            {
                return overrideResource;
            }

            if (ModsManager.Instance != null)
            {
                PassOnToModsManager passOnMod = ModsManager.Instance.PassOnMod;
                overrideResource = passOnMod.OnResourcesLoad(path, systemTypeInstance);
                if (overrideResource == null)
                {
                    overrideResource = passOnMod.OnResourcesLoad(path);
                }

                if (overrideResource != null)
                {
                    return overrideResource;
                }
            }

            return __result;
        }
    }
}