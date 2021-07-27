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
	/// <summary>
	/// Abstract class used to wrap javascript and lua engines in
	/// </summary>
	internal abstract class ScriptObject
	{
		/// <summary>
		/// Initializes the engine, stuff like setting up global functions
		/// </summary>
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

		/// <summary>
		/// Imports a enum to the code engine
		/// </summary>
		/// <typeparam name="T"></typeparam>
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

		/// <summary>
		/// Invokes when a error occurs in the engine
		/// </summary>
		public event Action<ScriptErrorType,string> OnError;
		internal void TriggerOnError(ScriptErrorType errorType, string error)
		{
			OnError?.Invoke(errorType, error);
		}

		/// <summary>
		/// Sets a global bool value in the engine
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public abstract void SetGlobal(string name, bool value);
		/// <summary>
		/// Sets a global Delegate value in the engine
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public abstract void SetGlobal(string name, Delegate value);
		/// <summary>
		/// Sets a global double value in the engine
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public abstract void SetGlobal(string name, double value);
		/// <summary>
		/// Sets a global object value in the engine
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public abstract void SetGlobal(string name, object value);
		/// <summary>
		/// Sets a global string value in the engine
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public abstract void SetGlobal(string name, string value);

		/// <summary>
		/// Sets a global value in the engine
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public abstract ScriptValue GetGlobal(string name);

		/// <summary>
		/// Run a bit of raw code
		/// </summary>
		/// <param name="code"></param>
		public abstract void RunCode(string code);

		/// <summary>
		/// Calls a global function
		/// </summary>
		/// <param name="function"></param>
		/// <param name="parameters"></param>
		public abstract void Call(string function, params object[] parameters);

		/// <summary>
		/// Calls a global function, with a return value
		/// </summary>
		/// <param name="function"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public abstract ScriptValue CallWithReturn(string function, params object[] parameters);

		/// <summary>
		/// The language of the code engine
		/// </summary>
		public abstract ScriptLanguage ScriptLanguage { get; }
	}

	/// <summary>
	/// Abstract class to wrap a value in a code engine
	/// </summary>
	public abstract class ScriptValue
	{
		/// <summary>
		/// if the value is null
		/// </summary>
		public abstract bool IsNull { get; }

		/// <summary>
		/// The value as a bool
		/// </summary>
		public abstract bool AsBool { get; }
		/// <summary>
		/// The value as a string
		/// </summary>
		public abstract string AsString { get; }
		/// <summary>
		/// The value as a double
		/// </summary>
		public abstract double AsNumber { get; }

		/// <summary>
		/// Get a field on the object
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public abstract ScriptValue this[string index] { get; }
		/// <summary>
		/// Gets the lengh of the object
		/// </summary>
		public abstract int ArrayLength { get; }
		/// <summary>
		/// Gets a index from the object
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public abstract ScriptValue this[int index] { get; }

		/// <summary>
		/// Calls the value as a function
		/// </summary>
		/// <param name="parameters"></param>
		public abstract void CallAsFunction(params object[] parameters);
		/// <summary>
		/// Calls the value as a function, with a return value
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public abstract ScriptValue CallAsFunctionWithReturn(params object[] parameters);
	}

	/// <summary>
	/// The supported scripting languages
	/// </summary>
	public enum ScriptLanguage
	{
		/// <summary>
		/// Javascript
		/// </summary>
		Javascript,
		/// <summary>
		/// Lua
		/// </summary>
		Lua
	}

	/// <summary>
	/// The different types of possible errors in scripting language code
	/// </summary>
	public enum ScriptErrorType
	{
		/// <summary>
		/// When the provided syntax was invalid
		/// </summary>
		SyntaxError,
		/// <summary>
		/// When something goes wrong duing the runtime
		/// </summary>
		RuntimeError,
		/// <summary>
		/// All other possible script errors
		/// </summary>
		OtherError
	}

}
