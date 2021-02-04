using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using MoonSharp.Interpreter;

namespace InternalModBot.Scripting
{
	public class LuaScriptObject : ScriptObject
	{
		Script _luaEngine;


		public LuaScriptObject()
		{
			UserData.RegisterAssembly();

			_luaEngine = new Script();

			Initialize();
		}

		public override ScriptLanguage ScriptLanguage => ScriptLanguage.Lua;

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

	public class LuaScriptValue : ScriptValue
	{
		Script _script;
		DynValue _value;

		public LuaScriptValue(DynValue value, Script script)
		{
			_value = value;
			_script = script;
		}

		public override ScriptValue this[string index]
		{
			get
			{
				return new LuaScriptValue(_value.Table.Get(index), _script);
			}
		}

		public override ScriptValue this[int index]
		{
			get
			{
				return new LuaScriptValue(_value.Table.Get(index), _script);
			}
		}

		public override bool AsBool
		{
			get
			{
				return _value.Boolean;
			}
		}

		public override string AsString
		{
			get
			{
				return _value.String;
			}
		}

		public override double AsNumber
		{
			get
			{
				return _value.Number;
			}
		}

		public override int ArrayLength
		{
			get
			{
				return (int)_value.GetLength().Number;
			}
		}

		public override bool IsNull => _value.IsNilOrNan() || _value.IsVoid();

		public override void CallAsFunction(params object[] parameters)
		{
			_script.Call(_value, parameters);
			
		}

		public override ScriptValue CallAsFunctionWithReturn(params object[] parameters)
		{
			return new LuaScriptValue(_script.Call(_value, parameters), _script);
		}
	}
}
