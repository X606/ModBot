using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace ModLibrary
{
	/// <summary>
	/// The base type for injectiontargets used by mod-bot, use <see cref="InjectionPrefixTargetAttribute"/> or <see cref="InjectionPostfixTargetAttribute"/> instead
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public abstract class InjectionTargetAttribute : Attribute
	{
		internal MethodInfo SelectedMethod;
		internal InjectionType InjectionType;

		internal void Init(Type type, string methodName, InjectionType injectionType, Type[] methodParameters = null)
        {
			MethodInfo selectedMethod = null;
			if (methodParameters == null)
			{
				selectedMethod = type.GetMethod(methodName, Injector.FLAGS);
			}
			else
			{
				selectedMethod = type.GetMethod(methodName, Injector.FLAGS, null, methodParameters, null);
			}
			SelectedMethod = selectedMethod;

			InjectionType = injectionType;
		}

		internal static KeyValuePair<InjectionTargetAttribute, MethodInfo>[] GetInjectionTargetsInAssembly(Assembly assembly)
		{
			List<KeyValuePair<InjectionTargetAttribute, MethodInfo>> output = new List<KeyValuePair<InjectionTargetAttribute, MethodInfo>>();

			Type[] types = assembly.GetTypes();

			for (int i = 0; i < types.Length; i++)
			{
				MethodInfo[] methods = types[i].GetMethods(Injector.FLAGS);
				for (int j = 0; j < methods.Length; j++)
				{
					if (!methods[j].IsStatic)
						continue;

					InjectionTargetAttribute targetAttribute = (InjectionTargetAttribute)GetCustomAttribute(methods[j], typeof(InjectionTargetAttribute));
					if (targetAttribute == null)
						continue;

					output.Add(new KeyValuePair<InjectionTargetAttribute, MethodInfo>(targetAttribute, methods[j]));
				}
			}

			return output.ToArray();
		}
	}

	/// <summary>
	/// Used to make mod-bot inject into the game code, and make it call the function this function is attached to before it runs the code in the target method
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class InjectionPrefixTargetAttribute : InjectionTargetAttribute
    {
		/// <summary>
		/// Used to make mod-bot inject into the game code, and make it call the function this function is attached to before it runs the code in the target method
		/// </summary>
		/// <param name="type">The type the target method is attached to</param>
		/// <param name="methodName">The name of the taget method</param>
		/// <param name="methodParameters">The parameter types on the target method</param>
		public InjectionPrefixTargetAttribute(Type type, string methodName, Type[] methodParameters = null)
        {
			Init(type, methodName, InjectionType.Prefix, methodParameters);
        }
    }

	/// <summary>
	/// Used to make mod-bot inject into the game code, and make it call the function this function is attached to after it runs the code in the target method
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class InjectionPostfixTargetAttribute : InjectionTargetAttribute
	{
		/// <summary>
		/// Used to make mod-bot inject into the game code, and make it call the function this function is attached to after it runs the code in the target method
		/// </summary>
		/// <param name="type">The type the target method is attached to</param>
		/// <param name="methodName">The name of the taget method</param>
		/// <param name="methodParameters">The parameter types on the target method</param>
		public InjectionPostfixTargetAttribute(Type type, string methodName, Type[] methodParameters = null)
		{
			Init(type, methodName, InjectionType.Postfix, methodParameters);
		}
	}

	internal enum InjectionType
	{
		Prefix,
		Postfix
	}

}
