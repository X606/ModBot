using ModLibrary;
using Rewired;
using Rewired.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InternalModBot
{
	public static class ModBotInputManager
	{
		public static readonly InputOption[] InputOptions = new InputOption[]
		{
			new InputOption(ModBotInputType.OpenConsole, KeyCode.F1, "Open Console Key"),
			new InputOption(ModBotInputType.ToggleFPSLabel, KeyCode.F3, "Toggle FPS Label Key")
		};

		static Dictionary<ModBotInputType, InputOption> _cachedDictionary = null;
		static void populateCachedDictionary()
		{
			_cachedDictionary = new Dictionary<ModBotInputType, InputOption>();
			foreach (InputOption inputOption in InputOptions)
			{
				_cachedDictionary.Add(inputOption.Type, inputOption);
			}

		}

		public static KeyCode GetKeyCode(ModBotInputType type)
		{
			if (_cachedDictionary == null)
				populateCachedDictionary();

			if (_cachedDictionary.TryGetValue(type, out InputOption value))
			{
				return value.Key;
			}

			throw new Exception(type.ToString() + " not connected to anything");
		}

		public class InputOption
		{
			public ModBotInputType Type;
			public KeyCode DefaultKey;
			public string DisplayName;

			KeyCode? _value = null;

			public InputOption(ModBotInputType type, KeyCode defaultKey, string displayName)
			{
				Type = type;
				DefaultKey = defaultKey;
				DisplayName = displayName;
			}


			public KeyCode Key
			{
				get
				{
					if (_value != null)
						return _value.Value;

					KeyCode keyCode = (KeyCode)PlayerPrefs.GetInt("ModBot_Keys_" + Type.ToString(), (int)DefaultKey);
					_value = keyCode;
					return keyCode;
				}
				set
				{
					_value = value;
					PlayerPrefs.SetInt("ModBot_Keys_" + Type.ToString(), (int)value);
				}
			}

		}
	}
	public enum ModBotInputType
	{
		OpenConsole,
		ToggleFPSLabel
	}

}
