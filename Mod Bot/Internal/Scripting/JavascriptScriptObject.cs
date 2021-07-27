using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;
using Jint.Runtime;
using Jint.Native;
using System.Reflection;
using Esprima;

namespace InternalModBot.Scripting
{
	/// <summary>
	/// Implements a javascript code engine to the <see cref="ScriptObject"/> wrapper
	/// </summary>
	internal class JavascriptScriptObject : ScriptObject
	{
		readonly Engine _javascriptEngine;

		/// <summary>
		/// Creates a new javascript engine
		/// </summary>
		public JavascriptScriptObject()
		{
			_javascriptEngine = new Engine();

			Initialize();
		}

		/// <summary>
		/// What language this engine is in
		/// </summary>
		public override ScriptLanguage ScriptLanguage => ScriptLanguage.Javascript;

		/// <summary>
		/// Calls a global function
		/// </summary>
		/// <param name="function"></param>
		/// <param name="parameters"></param>
		public override void Call(string function, params object[] parameters)
		{
			try
			{
				_javascriptEngine.Invoke(function, parameters);
			}
			catch (ParserException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (JavaScriptException e)
			{
				TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}
		}

		/// <summary>
		/// Calls a global function with a return value
		/// </summary>
		/// <param name="function"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public override ScriptValue CallWithReturn(string function, params object[] parameters)
		{
			try
			{
				return new JavascriptScriptValue(_javascriptEngine.Invoke(function, parameters), this);
			}
			catch (ParserException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (JavaScriptException e)
			{
				TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}

			return null;
		}

		/// <summary>
		/// Gets a global value
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override ScriptValue GetGlobal(string name)
		{
			try
			{
				return new JavascriptScriptValue(_javascriptEngine.GetValue(name), this);
			}
			catch (ParserException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (JavaScriptException e)
			{
				TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}

			return null;
		}

		/// <summary>
		/// Runs a bit of raw code
		/// </summary>
		/// <param name="code"></param>
		public override void RunCode(string code)
		{
			try
			{
				_javascriptEngine.Execute(code);
			}
			catch (ParserException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (JavaScriptException e)
			{
				TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}
		}

		/// <summary>
		/// Sets a global bool value
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void SetGlobal(string name, bool value)
		{
			try
			{
				_javascriptEngine.SetValue(name, value);
			}
			catch (ParserException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (JavaScriptException e)
			{
				TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}
		}

		/// <summary>
		/// Sets a global delegate value
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void SetGlobal(string name, Delegate value)
		{
			try
			{
				_javascriptEngine.SetValue(name, value);
			}
			catch (ParserException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (JavaScriptException e)
			{
				TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}
		}

		/// <summary>
		/// Sets a global number value
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void SetGlobal(string name, double value)
		{
			try
			{
				_javascriptEngine.SetValue(name, value);
			}
			catch (ParserException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (JavaScriptException e)
			{
				TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}
		}

		/// <summary>
		/// Sets a global object value
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void SetGlobal(string name, object value)
		{
			try
			{
				_javascriptEngine.SetValue(name, value);
			}
			catch (ParserException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (JavaScriptException e)
			{
				TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}
		}

		/// <summary>
		/// Sets a global string value
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void SetGlobal(string name, string value)
		{
			try
			{
				_javascriptEngine.SetValue(name, value);
			}
			catch (ParserException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (JavaScriptException e)
			{
				TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}
		}
	}

	/// <summary>
	/// A wrapper for a javascript value
	/// </summary>
	internal class JavascriptScriptValue : ScriptValue
	{
		JsValue _value;
		JavascriptScriptObject _scriptObject;

		/// <summary>
		/// Sets up the wrapper for a javascript value
		/// </summary>
		/// <param name="value"></param>
		/// <param name="scriptObject"></param>
		public JavascriptScriptValue(JsValue value, JavascriptScriptObject scriptObject)
		{
			_value = value;
			_scriptObject = scriptObject;
		}

		/// <summary>
		/// Gets a value out of the value, at the provided index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public override ScriptValue this[string index]
		{
			get
			{
				return new JavascriptScriptValue(_value.AsObject().Get(index), _scriptObject);
			}
		}

		/// <summary>
		/// Gets a value out of the value, at the provided index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public override ScriptValue this[int index]
		{
			get
			{
				return new JavascriptScriptValue(_value.AsArray().Get(index.ToString()), _scriptObject);
			}
		}

		/// <summary>
		/// Gets the value as a bool
		/// </summary>
		public override bool AsBool
		{
			get
			{
				return _value.AsBoolean();
			}
		}

		/// <summary>
		/// Gets the value as a string
		/// </summary>
		public override string AsString
		{
			get
			{
				return _value.AsString();
			}
		}

		/// <summary>
		/// Gets the value as a number
		/// </summary>
		public override double AsNumber
		{
			get
			{
				return _value.AsNumber();
			}
		}

		/// <summary>
		/// Gets the length of the object
		/// </summary>
		public override int ArrayLength
		{
			get
			{
				return (int)_value.AsArray().Length;
			}
		}

		/// <summary>
		/// Gets if the object is null
		/// </summary>
		public override bool IsNull => _value.IsNull() || _value.IsUndefined();

		/// <summary>
		/// Gets if the provided object is a number
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsNumber(object value)
		{
			return value is sbyte
					|| value is byte
					|| value is short
					|| value is ushort
					|| value is int
					|| value is uint
					|| value is long
					|| value is ulong
					|| value is float
					|| value is double
					|| value is decimal;
		}

		/// <summary>
		/// Calls the value as a function
		/// </summary>
		/// <param name="parameters"></param>
		public override void CallAsFunction(params object[] parameters)
		{
			try
			{
				JsValue[] arguments = new JsValue[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					if (parameters[i] is string str)
					{
						arguments[i] = str;
					}
					else if (parameters[i] is bool bol)
					{
						arguments[i] = bol;
					}
					else if (IsNumber(parameters[i]))
					{
						arguments[i] = Convert.ToDouble(parameters[i]);
					}
					else
					{
						throw new Exception("Unsupported type (" + parameters[i].GetType().ToString() + ")");
					}
				}

				_value.Invoke(arguments);
			}
			catch (ParserException e)
			{
				_scriptObject.TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (JavaScriptException e)
			{
				_scriptObject.TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				_scriptObject.TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}
		}

		/// <summary>
		/// Calls the value as a function, with a return value
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public override ScriptValue CallAsFunctionWithReturn(params object[] parameters)
		{
			try
			{
				JsValue[] arguments = new JsValue[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					if (parameters[i] is string str)
					{
						arguments[i] = str;
					}
					else if (parameters[i] is bool bol)
					{
						arguments[i] = bol;
					}
					else if (IsNumber(parameters[i]))
					{
						arguments[i] = Convert.ToDouble(parameters[i]);
					}
					else
					{
						throw new Exception("Unsupported type (" + parameters[i].GetType().ToString() + ")");
					}
				}

				return new JavascriptScriptValue(_value.Invoke(arguments), _scriptObject);
			}
			catch (ParserException e)
			{
				_scriptObject.TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (JavaScriptException e)
			{
				_scriptObject.TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				_scriptObject.TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}

			return null;
		}

	}
}
