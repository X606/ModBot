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

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class InjectionTargetAttribute : Attribute
	{
		internal MethodInfo SelectedMethod;
		public InjectionType InjectionType;

		public InjectionTargetAttribute(Type type, string methodName, InjectionType injectionType, Type[] methodParameters = null)
		{
			MethodInfo selectedMethod = null;
			if (methodParameters == null)
			{
				selectedMethod = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			} 
			else
			{
				selectedMethod = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodParameters, null);
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
	public enum InjectionType
	{
		Prefix,
		Postfix
	}

}
