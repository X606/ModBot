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
	/// <summary>
	/// Handles what keys are associated with what actions in mod-bot
	/// </summary>
	public static class ModBotInputManager
	{
		/// <summary>
		/// All the input options in mod-bot
		/// </summary>
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

		/// <summary>
		/// Gets the key associated with a specifc <see cref="ModBotInputType"/>
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Class for holding information about a input
		/// </summary>
		public class InputOption
		{
			/// <summary>
			/// <see cref="ModBotInputType"/> we want this input to be
			/// </summary>
			public ModBotInputType Type;
			/// <summary>
			/// The defualt key for the input
			/// </summary>
			public KeyCode DefaultKey;
			/// <summary>
			/// The display name for the key
			/// </summary>
			public string DisplayName;

			KeyCode? _value = null;

			/// <summary>
			/// Creates a new <see cref="InputOption"/>
			/// </summary>
			/// <param name="type"></param>
			/// <param name="defaultKey"></param>
			/// <param name="displayName"></param>
			public InputOption(ModBotInputType type, KeyCode defaultKey, string displayName)
			{
				Type = type;
				DefaultKey = defaultKey;
				DisplayName = displayName;
			}

			/// <summary>
			/// Gets or sets the key we want to associate with this input
			/// </summary>
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
	/// <summary>
	/// Different actions we want to accociate keys with
	/// </summary>
	public enum ModBotInputType
	{
		/// <summary>
		/// The key for opening the console
		/// </summary>
		OpenConsole,
		/// <summary>
		/// The key for toggling the fps label in the corner
		/// </summary>
		ToggleFPSLabel
	}

}
