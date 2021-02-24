using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using MoonSharp.Interpreter;

namespace InternalModBot.Scripting
{
	/// <summary>
	/// A instance of a lua script engine
	/// </summary>
	public class LuaScriptObject : ScriptObject
	{
		Script _luaEngine;

		/// <summary>
		/// Sets up the lua script engine
		/// </summary>
		public LuaScriptObject()
		{
			UserData.RegisterAssembly();

			_luaEngine = new Script();

			Initialize();
		}

		/// <summary>
		/// Tells us this is a lua engine
		/// </summary>
		public override ScriptLanguage ScriptLanguage => ScriptLanguage.Lua;

		/// <summary>
		/// Calls a function defined in the lua code
		/// </summary>
		/// <param name="function"></param>
		/// <param name="parameters"></param>
		public override void Call(string function, params object[] parameters)
		{
			try
			{
				DynValue target = _luaEngine.DoString(function);
				if (target.Type == DataType.Function)
				{
					target.Function.Call(parameters);
				}
				else
				{
					throw new Exception("Could not find function \"" + function + "\"");
				}
			} catch(SyntaxErrorException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			} catch(ScriptRuntimeException e)
			{
				TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			} catch(Exception e)
			{
				TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}
		}

		/// <summary>
		/// Calls a function defined in the lua code, and gets the return value it returns
		/// </summary>
		/// <param name="function"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public override ScriptValue CallWithReturn(string function, params object[] parameters)
		{
			try
			{
				DynValue target = _luaEngine.DoString(function);
				if (target.Type == DataType.Function)
				{
					return new LuaScriptValue(target.Function.Call(parameters), _luaEngine);
				}
				else
				{
					throw new Exception("Could not find function \"" + function + "\"");
				}
			}
			catch (SyntaxErrorException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (ScriptRuntimeException e)
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
		/// Gets a global value in the lua engine
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override ScriptValue GetGlobal(string name)
		{
			try
			{
				return new LuaScriptValue(_luaEngine.Globals.Get(name), _luaEngine);

			}
			catch (SyntaxErrorException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (ScriptRuntimeException e)
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
		/// Runs a bit of lua code
		/// </summary>
		/// <param name="code"></param>
		public override void RunCode(string code)
		{
			try
			{
				_luaEngine.DoString(code);
			}
			catch (SyntaxErrorException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (ScriptRuntimeException e)
			{
				TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}
		}

		/// <summary>
		/// Sets a global bool value in the lua engine
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void SetGlobal(string name, bool value)
		{
			try
			{
				_luaEngine.Globals[name] = value;
			}
			catch (SyntaxErrorException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (ScriptRuntimeException e)
			{
				TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}
		}

		/// <summary>
		/// Sets a global Delegate value in the lua engine
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void SetGlobal(string name, Delegate value)
		{
			try 
			{
				_luaEngine.Globals[name] = value;
			}
			catch (SyntaxErrorException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (ScriptRuntimeException e)
			{
				TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}
		}

		/// <summary>
		/// Sets a global double value in the lua engine
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void SetGlobal(string name, double value)
		{
			try
			{
				_luaEngine.Globals[name] = value;
			}
			catch (SyntaxErrorException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (ScriptRuntimeException e)
			{
				TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}
		}

		/// <summary>
		/// Sets a global object value in the lua engine
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void SetGlobal(string name, object value)
		{
			try 
			{
				_luaEngine.Globals[name] = value;
			}
			catch (SyntaxErrorException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (ScriptRuntimeException e)
			{
				TriggerOnError(ScriptErrorType.RuntimeError, e.Message);

			}
			catch (Exception e)
			{
				TriggerOnError(ScriptErrorType.OtherError, e.Message);
			}
		}

		/// <summary>
		/// Sets a global string value in the lua engine
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void SetGlobal(string name, string value)
		{
			try 
			{
				_luaEngine.Globals[name] = value;
			}
			catch (SyntaxErrorException e)
			{
				TriggerOnError(ScriptErrorType.SyntaxError, e.Message);
			}
			catch (ScriptRuntimeException e)
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
	/// A value in lua code, wrapped so we can easily interact with it
	/// </summary>
	public class LuaScriptValue : ScriptValue
	{
		Script _script;
		DynValue _value;

		/// <summary>
		/// Creates the wrapper from a dyn value
		/// </summary>
		/// <param name="value"></param>
		/// <param name="script"></param>
		public LuaScriptValue(DynValue value, Script script)
		{
			_value = value;
			_script = script;
		}

		/// <summary>
		/// Gets the object at the provided index out of the lua table
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public override ScriptValue this[string index]
		{
			get
			{
				return new LuaScriptValue(_value.Table.Get(index), _script);
			}
		}

		/// <summary>
		/// Gets the object at the provided index out of the lua table
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public override ScriptValue this[int index]
		{
			get
			{
				return new LuaScriptValue(_value.Table.Get(index), _script);
			}
		}

		/// <summary>
		/// Gets the lua value as a bool
		/// </summary>
		public override bool AsBool
		{
			get
			{
				return _value.Boolean;
			}
		}

		/// <summary>
		/// Gets the lua value as a string
		/// </summary>
		public override string AsString
		{
			get
			{
				return _value.String;
			}
		}

		/// <summary>
		/// Gets the lua value as a double
		/// </summary>
		public override double AsNumber
		{
			get
			{
				return _value.Number;
			}
		}

		/// <summary>
		/// Gets the length of the lua table
		/// </summary>
		public override int ArrayLength
		{
			get
			{
				return (int)_value.GetLength().Number;
			}
		}

		/// <summary>
		/// Gets if the lua value is null 
		/// </summary>
		public override bool IsNull => _value.IsNilOrNan() || _value.IsVoid();

		/// <summary>
		/// Calls the value as a function
		/// </summary>
		/// <param name="parameters"></param>
		public override void CallAsFunction(params object[] parameters)
		{
			_script.Call(_value, parameters);
			
		}

		/// <summary>
		/// Calls the value as a function, and gets the return value from it
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public override ScriptValue CallAsFunctionWithReturn(params object[] parameters)
		{
			return new LuaScriptValue(_script.Call(_value, parameters), _script);
		}
	}
}
