using HarmonyLib;
using InternalModBot;
using System;
using System.Reflection;

namespace ModLibrary
{
    /// <summary>
    /// Patches the target method with whatever method it's applied to as the prefix. See <see href="https://harmony.pardeike.net/articles/patching-prefix.html"/> for details about how prefixes work.<para/>Generic methods are not officially supported by 0Harmony and cannot be patched by using this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Obsolete("InjectionPrefixTargetAttribute is obsolete, use standard 0Harmony syntax for patching")]
	public class InjectionPrefixTargetAttribute : InjectionTargetAttribute
    {
		/// <summary>
		/// Initializes the <see cref="InjectionPrefixTargetAttribute"/> attribute with a target method
		/// </summary>
		/// <param name="type">The type the target method is attached to</param>
		/// <param name="methodName">The name of the target method</param>
		/// <param name="methodParameters">The parameter types on the target method</param>
		public InjectionPrefixTargetAttribute(Type type, string methodName, Type[] methodParameters = null) : base(FindMethod(type, methodName, methodParameters), HarmonyPatchType.Prefix)
        {
        }
	}
}
