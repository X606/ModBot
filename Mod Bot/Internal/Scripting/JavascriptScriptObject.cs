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
	public class JavascriptScriptObject : ScriptObject
	{
		Engine javascriptEngine;

		public JavascriptScriptObject()
		{
			javascriptEngine = new Engine();

			Initialize();
		}

		public override ScriptLanguage ScriptLanguage => ScriptLanguage.Javascript;

		public override void Call(string function, params object[] parameters)
		{
			try
			{
				javascriptEngine.Invoke(function, parameters);
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

		public override ScriptValue CallWithReturn(string function, params object[] parameters)
		{
			try
			{
				return new JavascriptScriptValue(javascriptEngine.Invoke(function, parameters), this);
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

		public override ScriptValue GetGlobal(string name)
		{
			try
			{
				return new JavascriptScriptValue(javascriptEngine.GetValue(name), this);
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

		public override void RunCode(string code)
		{
			try
			{
				javascriptEngine.Execute(code);
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

		public override void SetGlobal(string name, bool value)
		{
			try
			{
				javascriptEngine.SetValue(name, value);
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

		public override void SetGlobal(string name, Delegate value)
		{
			try
			{
				javascriptEngine.SetValue(name, value);
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

		public override void SetGlobal(string name, double value)
		{
			try
			{
				javascriptEngine.SetValue(name, value);
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

		public override void SetGlobal(string name, object value)
		{
			try
			{
				javascriptEngine.SetValue(name, value);
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

		public override void SetGlobal(string name, string value)
		{
			try
			{
				javascriptEngine.SetValue(name, value);
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

	public class JavascriptScriptValue : ScriptValue
	{
		JsValue _value;
		JavascriptScriptObject _scriptObject;

		public JavascriptScriptValue(JsValue value, JavascriptScriptObject scriptObject)
		{
			_value = value;
			_scriptObject = scriptObject;
		}

		public override ScriptValue this[string index]
		{
			get
			{
				return new JavascriptScriptValue(_value.AsObject().Get(index), _scriptObject);
			}
		}

		public override ScriptValue this[int index]
		{
			get
			{
				return new JavascriptScriptValue(_value.AsArray().Get(index.ToString()), _scriptObject);
			}
		}

		public override bool AsBool
		{
			get
			{
				return _value.AsBoolean();
			}
		}

		public override string AsString
		{
			get
			{
				return _value.AsString();
			}
		}

		public override double AsNumber
		{
			get
			{
				return _value.AsNumber();
			}
		}

		public override int ArrayLength
		{
			get
			{
				return (int)_value.AsArray().Length;
			}
		}

		public override bool IsNull => _value.IsNull() || _value.IsUndefined();

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
