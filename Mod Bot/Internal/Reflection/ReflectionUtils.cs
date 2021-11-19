using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace InternalModBot
{
	internal static class ReflectionUtils
    {
		public static string GetAccessDescription(this MethodBase methodBase)
		{
			if (methodBase is null)
				return string.Empty;

			StringBuilder builder = new StringBuilder();

			if (methodBase.IsPublic)
			{
				builder.Append("public");
			}
			else if (methodBase.IsPrivate)
			{
				builder.Append("private");
			}
			else if (methodBase.IsFamily)
			{
				builder.Append("protected");
			}
			else if (methodBase.IsAssembly)
			{
				builder.Append("internal");
			}
			else if (methodBase.IsFamilyOrAssembly)
			{
				builder.Append("protected internal");
			}
			else if (methodBase.IsFamilyAndAssembly)
			{
				builder.Append("private protected");
			}
			else
			{
				throw new Exception($"Unknown access type for {methodBase.FullDescription()}");
			}

			if (methodBase.IsAbstract)
			{
				builder.Append(" abstract");
			}
			else if (methodBase.IsVirtual)
			{
				builder.Append(" virtual");
			}

			if (methodBase.IsStatic)
				builder.Append(" static");

			return builder.ToString();
		}

		public static string GetFullDescription(this PropertyInfo propertyInfo)
		{
			if (propertyInfo is null)
				return "null";

			StringBuilder builder = new StringBuilder();

			builder.Append(propertyInfo.PropertyType.FullDescription());
			builder.Append(" ");
			builder.Append(propertyInfo.Name);

			if (propertyInfo.GetIndexParameters().Length > 0)
				builder.Append("[" + propertyInfo.GetIndexParameters().Types().Join(t => t.FullDescription()) + "]");

			builder.Append(" { ");

			if (!(propertyInfo.GetMethod is null))
				builder.Append(GetAccessDescription(propertyInfo.GetMethod) + " get; ");

			if (!(propertyInfo.SetMethod is null))
				builder.Append(GetAccessDescription(propertyInfo.SetMethod) + " set; ");

			builder.Append("}");

			return builder.ToString();
		}

		public static string GetFullDescription(this MemberInfo memberInfo)
        {
			if (memberInfo is null)
				return "null";

			if (memberInfo is MethodBase methodBase)
            {
				return methodBase.FullDescription();
            }
			else if (memberInfo is PropertyInfo propertyInfo)
            {
				return propertyInfo.GetFullDescription();
            }
			else if (memberInfo is FieldInfo fieldInfo)
            {
				return fieldInfo.FieldType.FullDescription() + " " + fieldInfo.Name;
            }
			else if (memberInfo is TypeInfo typeInfo)
            {
				return typeInfo.FullDescription();
            }
            else
            {
				throw new ArgumentException($"Unsupported member type: {memberInfo.GetType().Name}");
            }
        }

		public static MatchType GetMatchType(object[] arguments, Type[] argumentTypeOverrides, int index)
		{
			Type overrideType = null;
			if (argumentTypeOverrides != null && index < argumentTypeOverrides.Length && argumentTypeOverrides[index] != null)
				overrideType = argumentTypeOverrides[index];

			bool hasArgument = false;
			Type argumentType = null;
			if (arguments != null && index < arguments.Length)
			{
				hasArgument = true;

				if (arguments[index] != null)
					argumentType = arguments[index].GetType();
			}

			if (overrideType != null)
			{
				if (argumentType != null && !overrideType.IsAssignableFrom(argumentType))
				{
					throw new ArgumentException($"Argument at index {index} ({arguments[index]}, type: {argumentType.FullDescription()}) is not assignable to override type {overrideType.FullDescription()}");
				}
				else if (hasArgument && arguments[index] == null)
				{
					throw new ArgumentException($"Argument at index {index} is null, but {overrideType.FullDescription()} is not a nullable type");
				}
			}

			if (overrideType != null)
			{
				return new MatchType(overrideType, false);
			}
			else if (argumentType != null)
			{
				return new MatchType(argumentType, true);
			}
			else
			{
				return null;
			}
		}
	}
}
