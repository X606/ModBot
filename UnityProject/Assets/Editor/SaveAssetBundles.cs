using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SaveAssetBundles : EditorWindow {

	public const string ASSET_BUNDLES_FOLDER_NAME = "AssetBundles";

	public static readonly string[] AssetBundlesToExport = new string[]
	{
		"modbot"
	};

	[MenuItem("Window/Save AssetBundles")]
	public static void Open(){

		string AssetBundlesFolderPath = Application.dataPath + "/" + ASSET_BUNDLES_FOLDER_NAME;

		Debug.Log("Deleting old files...");
		string[] oldFiles = Directory.GetFiles(AssetBundlesFolderPath);
		for (int i = 0; i < oldFiles.Length; i++)
		{
			File.Delete(oldFiles[i]);
		}
		Debug.Log("Done deleting old files!");

		Debug.Log("Building asset bundles...");
		BuildPipeline.BuildAssetBundles ("Assets/" + ASSET_BUNDLES_FOLDER_NAME + "/", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
		AssetDatabase.Refresh();
		Debug.Log("Done Building asset bundles!");

		string outputPath = new DirectoryInfo(Application.dataPath).Parent.Parent.FullName + "/" + "built version/Clone Drone in the Danger Zone_Data";

		Debug.Log("Copying files...");
		for (int i = 0; i < AssetBundlesToExport.Length; i++)
		{
			string source = AssetBundlesFolderPath + "/" + AssetBundlesToExport[i];
			string destination = outputPath + "/" + AssetBundlesToExport[i];

			File.Copy(source, destination, true);
			Debug.Log("Copied \"<color=#ff0000>" + source + "</color>\" to \"<color=#00ff00>" + destination + "</color>\"");
		}
		Debug.Log("Done copying files!");



	}
}
