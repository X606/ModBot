using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using InternalModBot;
using UnityEngine;

namespace InternalModBot.LevelEditor
{
	public static class ModBotCustomLevelEditorManager
	{
		public static void Init()
		{
			GameObject scriptableObject = InternalAssetBundleReferences.ModBot.GetObject("ScriptableObject");
			Texture2D texture = InternalAssetBundleReferences.ModBot.GetObject<Texture2D>("script");
			scriptableObject.AddComponent<Scriptable>();
			scriptableObject.AddComponent<LevelEditorToolRestriction>().DisallowedTools = new List<LevelEditorToolType>() { LevelEditorToolType.Rotate, LevelEditorToolType.Scale };
			scriptableObject.AddComponent<LevelEditorComponentDescription>().Description = "Runs a bit of code when the level starts";
			LevelEditorObjectAdder.AddObject("Mod-Bot", "ScriptableObject", scriptableObject.transform, texture);

		}
	}
}
