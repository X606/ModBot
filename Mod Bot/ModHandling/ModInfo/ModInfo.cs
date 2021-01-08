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
	[Serializable]
	public class ModInfo
	{
		public bool AreAllEssentialFieldsAssigned(out string errorMessage)
		{
			if(string.IsNullOrWhiteSpace(DisplayName))
			{
				errorMessage = "ModInfo.json: DisplayName not assigned";
				return false;
			}

			if(string.IsNullOrWhiteSpace(UniqueID))
			{
				errorMessage = "ModInfo.json: UniqueID not assigned";
				return false;
			}

			if(string.IsNullOrWhiteSpace(MainDLLFileName))
			{
				errorMessage = "ModInfo.json: MainDLLFileName not assigned";
				return false;
			}

			if(string.IsNullOrWhiteSpace(Author))
			{
				errorMessage = "ModInfo.json: Author not assigned";
				return false;
			}

			errorMessage = null;
			return true;
		}

		public void FixFieldValues()
		{
			if(!MainDLLFileName.EndsWith(".dll"))
				MainDLLFileName += ".dll";

			if(ModDependencies == null)
				ModDependencies = new string[0];
		}

		[JsonRequired]
		[JsonProperty]
		public string DisplayName { get; internal set; }

		[JsonRequired]
		[JsonProperty]
		public string UniqueID { get; internal set; }

		[JsonRequired]
		[JsonProperty]
		public string MainDLLFileName { get; internal set; }

		[JsonRequired]
		[JsonProperty]
		public string Author { get; internal set; }

		[JsonRequired]
		[JsonProperty]
		public uint Version { get; internal set; }

		[JsonProperty]
		public string ImageFileName { get; internal set; }

		[JsonProperty]
		public string Description { get; internal set; }

		[JsonProperty]
		public string[] ModDependencies { get; internal set; }

		[JsonProperty]
		public string[] Tags { get; internal set; }

		public string FolderPath { get; internal set; }

		public string DLLPath => FolderPath + MainDLLFileName;

		public bool HasImage => !string.IsNullOrWhiteSpace(ImageFileName);

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