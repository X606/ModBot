using HarmonyLib;
using ModLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace InternalModBot
{
    /// <summary>
    /// Base attribute type for 0Harmony patch targets
    /// </summary>
	[Obsolete("InjectionTargetAttribute is obsolete, use standard 0Harmony syntax for patching")]
	public class InjectionTargetAttribute : Attribute
	{
		internal MethodBase TargetMethod;
		internal HarmonyPatchType PatchType;

		internal InjectionTargetAttribute(MethodBase targetMethod, HarmonyPatchType patchType)
        {
            TargetMethod = targetMethod;
            PatchType = patchType;
        }

		internal static List<InjectionInfo> GetInjectionTargetsInAssembly(Assembly assembly)
        {
            const BindingFlags PATCH_METHODS_FLAGS = BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

			List<InjectionInfo> injections = new List<InjectionInfo>();

			Type[] types = assembly.GetTypes();

			foreach (Type type in types)
			{
                MethodInfo[] methods = type.GetMethods(PATCH_METHODS_FLAGS);
				foreach (MethodInfo method in methods)
                {
                    Attribute[] attributes = GetCustomAttributes(method, typeof(InjectionTargetAttribute));
					foreach (Attribute attribute in attributes)
                    {
                        if (attribute is InjectionTargetAttribute targetAttribute)
                        {
                            injections.Add(new InjectionInfo(targetAttribute, method));
                        }
                    }
                }
            }

			return injections;
        }

        #region Reflection Helpers
        internal static bool ParametersMatch(ParameterInfo[] parameterInfos, Type[] parameterTypes)
        {
			if (parameterInfos.Length != parameterTypes.Length)
				return false;

			for (int i = 0; i < parameterTypes.Length; i++)
			{
				if (parameterTypes[i] != parameterInfos[i].ParameterType)
					return false;
			}

			return true;
		}

		internal static MethodInfo FindMethod(Type type, string methodName, Type[] methodParameters)
		{
			MethodInfo[] methodInfos = type.GetMethods(Injector.FLAGS).Where(delegate (MethodInfo methodInfo)
			{
				if (methodInfo.Name != methodName || methodInfo.ContainsGenericParameters || methodInfo.IsGenericMethod || methodInfo.IsGenericMethodDefinition)
					return false;

				// Some methods have no body, and thus cannot be patched
				if (methodInfo.GetMethodBody() == null)
					return false;

				if (methodParameters != null)
				{
					ParameterInfo[] parameterInfos = methodInfo.GetParameters();
					if (!ParametersMatch(parameterInfos, methodParameters))
						return false;
				}

				return true;
			}).ToArray();

			if (methodInfos.Length == 0)
			{
				throw new MissingMethodException(type.Name, methodName);
			}
			else if (methodInfos.Length == 1)
			{
				return methodInfos[0];
			}
			else
			{
                string parameters = methodParameters?.Join((t) => t.FullDescription()) ?? string.Empty;
                throw new AmbiguousMatchException("AmbiguousMatchException while searching for method " + methodName + " with parameters [" + parameters + "] in type " + type.FullDescription());
			}
        }
        #endregion
    }
}
