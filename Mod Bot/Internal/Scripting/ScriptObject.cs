using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;
using Jint.Native;
using ModLibrary;
using System.Reflection;
using System.Linq.Expressions;
using UnityEngine;

namespace InternalModBot.Scripting
{

	public abstract class ScriptObject
	{
		protected void Initialize()
		{
			ImportEnum<KeyCode>();

			MethodInfo[] methods = typeof(GlobalScriptFunctions).GetMethods(BindingFlags.Public | BindingFlags.Static);
			for (int i = 0; i < methods.Length; i++)
			{
				string name = methods[i].Name;
				Delegate func = CreateDelegate(methods[i], null);

				SetGlobal(name, func);
			}

			FieldInfo[] fields = typeof(GlobalScriptFunctions).GetFields(BindingFlags.Public | BindingFlags.Static);
			for (int i = 0; i < fields.Length; i++)
			{
				string name = fields[i].Name;
				object value = fields[i].GetValue(null);

				SetGlobal(name, value);
			}

		}

		public void ImportEnum<T>() where T : struct, IConvertible
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (T enumVal in Enum.GetValues(typeof(T)))
			{
				string key = enumVal.ToString();
				int value = Convert.ToInt32(Enum.Parse(typeof(T), enumVal.ToString()) as Enum);

				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, value);
				}
			}
			SetGlobal(typeof(T).Name, dictionary);
		}

		static Delegate CreateDelegate(MethodInfo methodInfo, object target)
		{
			Func<Type[], Type> getType;
			var isAction = methodInfo.ReturnType.Equals((typeof(void)));
			var types = methodInfo.GetParameters().Select(p => p.ParameterType);

			if (isAction)
			{
				getType = Expression.GetActionType;
			}
			else
			{
				getType = Expression.GetFuncType;
				types = types.Concat(new[] { methodInfo.ReturnType });
			}

			if (methodInfo.IsStatic)
			{
				return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
			}

			return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
		}

		public event Action<ScriptErrorType,string> OnError;
		internal void TriggerOnError(ScriptErrorType errorType, string error)
		{
			OnError?.Invoke(errorType, error);
		}

		public abstract void SetGlobal(string name, bool value);
		public abstract void SetGlobal(string name, Delegate value);
		public abstract void SetGlobal(string name, double value);
		public abstract void SetGlobal(string name, object value);
		public abstract void SetGlobal(string name, string value);

		public abstract ScriptValue GetGlobal(string name);

		public abstract void RunCode(string code);

		public abstract void Call(string function, params object[] parameters);
		public abstract ScriptValue CallWithReturn(string function, params object[] parameters);

		public abstract ScriptLanguage ScriptLanguage { get; }
	}

	public abstract class ScriptValue
	{
		public abstract bool IsNull { get; }

		public abstract bool AsBool { get; }
		public abstract string AsString { get; }
		public abstract double AsNumber { get; }

		public abstract ScriptValue this[string index] { get; }
		public abstract int ArrayLength { get; }
		public abstract ScriptValue this[int index] { get; }

		public abstract void CallAsFunction(params object[] parameters);
		public abstract ScriptValue CallAsFunctionWithReturn(params object[] parameters);
	}

	public enum ScriptLanguage
	{
		Javascript,
		Lua
	}
	public enum ScriptErrorType
	{
		SyntaxError,
		RuntimeError,
		OtherError
	}

}
