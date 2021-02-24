using ModLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using InternalModBot.Scripting;
using System.Diagnostics;

namespace InternalModBot.LevelEditor
{
	/// <summary>
	/// The component used in the editor to handle custom scriptable objects
	/// </summary>
	public class Scriptable : MonoBehaviour, ITriggerActivationReceiver, IDropdownOptions
	{
		/// <summary>
		/// The actual field that saves the code that should run
		/// </summary>
		[IncludeInLevelEditor(true, true)]
		public string Code;

		/// <summary>
		/// What programing language this code is in, js or lua
		/// </summary>
		[IncludeInLevelEditor(false, false)]
		public int Language;

		FileSystemWatcher _fileSystemWatcher;

		static Sprite JavascriptSprite = null;
		static Sprite LuaSprite = null;

		ScriptObject _scriptEngine;
		ScriptValue _triggers;

		/// <summary>
		/// Sets the code and tells the level editor that the value was changed
		/// </summary>
		/// <param name="code"></param>
		public void SetCode(string code)
		{
			Code = code;
			GetComponent<ObjectPlacedInLevel>().SetCustomInspectorStringValue(GetType().Name, "Code", Code);
			GlobalEventManager.Instance.Dispatch(GlobalEvents.LevelEditorLevelChanged);
		}

		/// <summary>
		/// Method exposed to the editor, called when user clicks button in editor
		/// </summary>
		[IncludeInLevelEditor]
		public void OpenCodeEditor()
		{
			string tempPath = Path.GetTempPath() + "/" + getFileName();
			File.WriteAllText(tempPath, Code);

			Process.Start("notepad++.exe", "\"" + tempPath + "\"");
			_fileSystemWatcher.Path = Path.GetTempPath();
			_fileSystemWatcher.Filter = getFileName();
			_fileSystemWatcher.EnableRaisingEvents = true;
		}

		void OnCodeFileChanged(object sender, FileSystemEventArgs e)
		{
			ThreadedDelegateScheduler.CallActionNextUpdate(delegate
			{
				string tempPath = Path.GetTempPath() + "/" + getFileName();
				SetCode(File.ReadAllText(tempPath));
				LevelEditorDataManager.Instance.SaveLevel();
			});
		}

		void Start()
		{
			if (JavascriptSprite == null)
				JavascriptSprite = InternalAssetBundleReferences.ModBot.GetObject<Sprite>("javascript");
			if (LuaSprite == null)
				LuaSprite = InternalAssetBundleReferences.ModBot.GetObject<Sprite>("lua");

			GameObject renderer = transform.GetChild(0).gameObject;
			if (GameFlowManager.Instance.IsInEditorMode())
			{
				renderer.SetActive(true);
				_fileSystemWatcher = new FileSystemWatcher();
				_fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
				_fileSystemWatcher.Changed += OnCodeFileChanged;
			}
			else
			{
				renderer.SetActive(false);

				List<ToggleListOption> zoneOptions = AIManager.Instance.GetEnableAIZoneIDOptions();
				string[] zones = new string[zoneOptions.Count];
				for (int i = 0; i < zoneOptions.Count; i++)
				{
					zones[i] = zoneOptions[i].Value;
				}
				AIManager.Instance.AddReceiverToTriggerZones(this, zones);

				runScript();
			}


		}
		void OnDestroy()
		{
			if (GameFlowManager.Instance.IsInEditorMode())
			{
				_fileSystemWatcher.EnableRaisingEvents = false;
				_fileSystemWatcher.Dispose();

				string tempPath = Path.GetTempPath() + "/" + getFileName();
				File.Delete(tempPath);

			}

		}

		void Update()
		{
			if (_scriptEngine != null)
			{
				ScriptValue update = _scriptEngine.GetGlobal("Update");
				if (!update.IsNull)
					update.CallAsFunction();
			}
		}

		void runScript()
		{
			if (Language == (int)ScriptLanguage.Javascript)
			{
				_scriptEngine = new JavascriptScriptObject();
			}
			else if (Language == (int)ScriptLanguage.Lua)
			{
				_scriptEngine = new LuaScriptObject();
			}
			else
			{
				throw new Exception("Unknown language (" + Language + ")");
			}
			_scriptEngine.OnError += delegate (ScriptErrorType error, string message)
			{
				debug.Log("(" + error + ") " + message);
			};

			_scriptEngine.RunCode(Code);

			_triggers = _scriptEngine.GetGlobal("Triggers");
			
		}

		string _cachedFileName = null;
		static int _nextScriptID = 0;
		string getFileName()
		{
			string fileName;
			if (_cachedFileName != null)
			{
				fileName = _cachedFileName;
			}
			else
			{
				fileName = "Script" + _nextScriptID.ToString();
				_cachedFileName = fileName;
				_nextScriptID++;
			}
			
			string extension = ".txt";
			if (Language == (int)ScriptLanguage.Javascript)
				extension = ".js";
			if (Language == (int)ScriptLanguage.Lua)
				extension = ".lua";

			return fileName + extension;
		}

		/// <summary>
		/// Part of the <see cref="ITriggerActivationReceiver"/> interface
		/// </summary>
		/// <param name="zoneID"></param>
		public void ActivateFromTrigger(string zoneID)
		{
			if (_triggers.IsNull)
				return;

			string zoneName = getTriggerDisplayName(zoneID);

			ScriptValue selectedEvent = _triggers[zoneName];
			if (!selectedEvent.IsNull)
				selectedEvent.CallAsFunction();


		}

		LevelEditorBaseTrigger[] _cachedTriggers = null;
		string getTriggerDisplayName(string zoneID)
		{
			if (_cachedTriggers == null)
				_cachedTriggers = TransformUtils.FindObjectsOfTypeAll<LevelEditorBaseTrigger>().ToArray();

			for (int i = 0; i < _cachedTriggers.Length; i++)
			{
				if (_cachedTriggers[i].ZoneID == zoneID)
					return _cachedTriggers[i].DisplayName;
			}

			throw new Exception("Unable to find trigger with id \"" + zoneID + "\"");
		}

		/// <summary>
		/// Part of the <see cref="ITriggerActivationReceiver"/> interface
		/// </summary>
		public int GetTriggerPriority() => 0;

		/// <summary>
		/// Part of the <see cref="IDropdownOptions"/> interface
		/// </summary>
		/// <param name="fieldName"></param>
		public List<Dropdown.OptionData> GetDropdownOptions(string fieldName)
		{
			if (fieldName == "Language")
			{
				return new List<Dropdown.OptionData>()
				{
					new Dropdown.OptionData(ScriptLanguage.Javascript.ToString(), JavascriptSprite),
					new Dropdown.OptionData(ScriptLanguage.Lua.ToString(), LuaSprite)
				};
			}

			throw new NotImplementedException("Unknown fieldName: " + fieldName);
		}

		/// <summary>
		/// Part of the <see cref="IDropdownOptions"/> interface
		/// </summary>
		/// <param name="fieldName"></param>
		public bool ShouldShowDropdownOptions(string fieldName) => fieldName == "Language";

		/// <summary>
		/// Part of the <see cref="IDropdownOptions"/> interface
		/// </summary>
		/// <param name="fieldName"></param>
		public bool HasDropDownForValue(string fieldName) => fieldName == "Language";
	}
}
