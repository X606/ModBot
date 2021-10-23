using HarmonyLib;
using InternalModBot;
using System;
using System.Reflection;

namespace ModLibrary
{
	/// <summary>
	/// Patches the target property with whatever method it's applied to as the postfix. See <see href="https://harmony.pardeike.net/articles/patching-postfix.html"/> for details about how postfixes work.<para/>Generic methods are not officially supported by 0Harmony and cannot be patched by using this attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Obsolete("InjectionPostfixTargetAttribute is obsolete, use standard 0Harmony syntax for patching")]
	public class InjectionPostfixTargetAttribute : InjectionTargetAttribute
	{
		/// <summary>
		/// Initializes the <see cref="InjectionPostfixTargetAttribute"/> attribute with a target method
		/// </summary>
		/// <param name="type">The type the target method is attached to</param>
		/// <param name="methodName">The name of the target method</param>
		/// <param name="methodParameters">The parameter types on the target method</param>
		public InjectionPostfixTargetAttribute(Type type, string methodName, Type[] methodParameters = null) : base(FindMethod(type, methodName, methodParameters), HarmonyPatchType.Postfix)
		{
		}
	}
}
