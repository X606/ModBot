using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class ModBotWindow : EditorWindow
{
    const string MOD_DATA_ASSET_PATH = "Assets/Mod-Bot Unity plugin/GeneratedAssets/ModsData.asset";

    [MenuItem("Mod-Bot/Editor")]
    public static void ShowWindow()
    {
        ModBotWindow window = (ModBotWindow)EditorWindow.GetWindow(typeof(ModBotWindow));
        ModsData modsData = AssetDatabase.LoadAssetAtPath<ModsData>(MOD_DATA_ASSET_PATH);
        if (modsData == null)
        {
            modsData = ScriptableObject.CreateInstance<ModsData>();
            window.Data = modsData;
            AssetDatabase.CreateAsset(window.Data, MOD_DATA_ASSET_PATH);
        }
        else
        {
            window.Data = modsData;
        }

        
    }

    bool showFoldout = false;
    Vector2 scroll = new Vector2(0,0);

    public ModsData Data;

    void OnGUI()
    {
        scroll = GUILayout.BeginScrollView(scroll);

        titleContent = new GUIContent("Mod-Bot");
        GUILayout.Label("Mods", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        showFoldout = EditorGUILayout.Foldout(showFoldout, "aaaa");
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("-", GUILayout.Width(24)))
        {
            Data.Mods.RemoveAt(Data.Mods.Count - 1);
        }
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("+", GUILayout.Width(24)))
        {
            Data.Mods.Add(new ModData("aa"));
        }
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();

        if (showFoldout)
        {
            for (int j = 0; j < Data.Mods.Count; j++)
            {
                ModData mod = Data.Mods[j];

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(mod.ModName);
                //mod.Value = (GameObject)EditorGUILayout.ObjectField(mod.Value, typeof(GameObject), false);
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();
    }
}

[Serializable]
public class ModData
{
    public ModData(string modName)
    {
        ModName = modName;
    }

    public string ModName;
    public ModAssets Assets = new ModAssets();
    public ModCustomFlowCharts CustomFlowCharts = new ModCustomFlowCharts();

}
[Serializable]
public class ModAssets
{

}
[Serializable]
public class ModCustomFlowCharts
{

}