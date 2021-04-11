using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InternalModBot;

namespace ModLibrary
{
	/// <summary>
	/// Contains all relevant non-runtime info about a mod
	/// </summary>
	[Serializable]
	public class ModInfo
	{
		/// <summary>
		/// The name of the .lua file in a mod
		/// </summary>
		public const string MAIN_LUA_FILE_NAME = "main.lua";

		/// <summary>
		/// The name to use when displaying this mod in the menus
		/// </summary>
		[JsonRequired]
		[JsonProperty]
		public string DisplayName { get; internal set; }

		/// <summary>
		/// A unique itentifier for this mod, should be a GUID or UUID
		/// </summary>
		[JsonRequired]
		[JsonProperty]
		public string UniqueID { get; internal set; }

		/// <summary>
		/// The mod type
		/// </summary>
		[JsonRequired]
		[JsonProperty]
		public ModType Type { get; internal set; }

		/// <summary>
		/// The name of the main mod .dll file, only relevant if <see cref="Type"/> is set to <see cref="ModType.CSharp"/>
		/// </summary>
		[JsonProperty]
		public string MainDLLFileName { get; internal set; }

		/// <summary>
		/// The creator of this mod
		/// </summary>
		[JsonRequired]
		[JsonProperty]
		public string Author { get; internal set; }

		/// <summary>
		/// An unsigned integer representing the version of the mod, used to check for mod updates, increment for every mod update
		/// </summary>
		[JsonRequired]
		[JsonProperty]
		public uint Version { get; internal set; }

		/// <summary>
		/// The file name of the image to display in the menus
		/// </summary>
		[JsonProperty]
		public string ImageFileName { get; internal set; }

		/// <summary>
		/// A description of the mod
		/// </summary>
		[JsonProperty]
		public string Description { get; internal set; }

		/// <summary>
		/// A list of mod IDs this mod is dependent on
		/// </summary>
		[JsonProperty]
		public string[] ModDependencies { get; internal set; }

		/// <summary>
		/// A list of tags to categorize this mod
		/// </summary>
		[JsonProperty]
		public string[] Tags { get; internal set; }

		/// <summary>
		/// Gets or sets the full path to this mod's containing folder
		/// </summary>
		public string FolderPath { get; internal set; }

		/// <summary>
		/// Gets the full path to this mod's main .dll file, only relevant if <see cref="Type"/> is set to <see cref="ModType.CSharp"/>
		/// </summary>
		public string DLLPath => FolderPath + MainDLLFileName;

		/// <summary>
		/// Gets the full path to this mod's main.lua file, only relevant if <see cref="Type"/> is set to <see cref="ModType.LUA"/>
		/// </summary>
		public string MainLuaFilePath => FolderPath + MAIN_LUA_FILE_NAME;

		/// <summary>
		/// Checks to see if all the required fields are assigned and generates an error message if any required field is not assigned
		/// </summary>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		public bool AreAllEssentialFieldsAssigned(out string errorMessage)
		{
			if (string.IsNullOrWhiteSpace(DisplayName))
			{
				errorMessage = "ModInfo.json: DisplayName not assigned";
				return false;
			}

			if (string.IsNullOrWhiteSpace(UniqueID))
			{
				errorMessage = "ModInfo.json: UniqueID not assigned";
				return false;
			}

			if (Type == ModType.CSharp && string.IsNullOrWhiteSpace(MainDLLFileName))
			{
				errorMessage = "ModInfo.json: MainDLLFileName not assigned";
				return false;
			}

			if (string.IsNullOrWhiteSpace(Author))
			{
				errorMessage = "ModInfo.json: Author not assigned";
				return false;
			}

			errorMessage = null;
			return true;
		}

		/// <summary>
		/// Checks and fixes field values to make other parts of Mod-Bot be able to handle them properly
		/// </summary>
		public void FixFieldValues()
		{
			if (Type == ModType.CSharp && !MainDLLFileName.EndsWith(".dll"))
				MainDLLFileName += ".dll";

			if (ModDependencies == null)
				ModDependencies = new string[0];

			if (Tags == null)
				Tags = new string[0];
		}

		/// <summary>
		/// NOTE: setting this value only sets the playerPrefs, use <see cref="LoadedModInfo.IsEnabled"/> to work with loaded mods
		/// </summary>
		internal bool IsModEnabled
		{
			get
			{
				return PlayerPrefs.GetInt(UniqueID, 1) == 1;
			}
			set
			{
				PlayerPrefs.SetInt(UniqueID, value ? 1 : 0);
			}
		}
	}
}