using InternalModBot;
using ModLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class ModsManager : Singleton<ModsManager>
{
    public void Start()
    {
        ReloadMods();
        PassOnMod.OnSceneChanged(GameMode.None);
    }

    public void ReloadMods()
    {
        UpgradeCosts.Reset();
        UpgradePagesManager.Reset();
        ClearCache();
        AssetLoader.ClearCache();
        Mods.Clear();
        PassOnMod = new PassOnToModsManager();
        string[] files = Directory.GetFiles(AssetLoader.GetSubdomain(Application.dataPath) + "mods/");
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".dll"))
            {
                try
                {
                    Type[] types = Assembly.Load(File.ReadAllBytes(files[i])).GetTypes();
                    Type type = null;
                    for (int j = 0; j < types.Length; j++)
                    {
                        if (types[j].Name.ToLower() == "main")
                        {
                            type = types[j];
                        }
                    }
                    if (type == null)
                    {
                        throw new Exception("could not find class 'main'");
                    }
                    object obj = Activator.CreateInstance(type);
                    Mods.Add((Mod)obj);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Mod '" + files[i] + "' is not working, make sure that it is set up right: " + ex.Message);
                }
            }
        }
        PassOnMod.OnModRefreshed();
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.R))
        {
            ReloadMods();
        }
    }

    public void LoadMod(byte[] assebly)
    {
        try
        {
            Type[] types = Assembly.Load(assebly).GetTypes();
            Type type = null;
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].Name == "main")
                {
                    type = types[i];
                }
            }
            if (type == null)
            {
                throw new Exception("could not find class 'main'");
            }
            object obj = Activator.CreateInstance(type);
            Mods.Add((Mod)obj);
            ((Mod)obj).OnModRefreshed();
        }
        catch
        {
            Debug.LogError("The mod you are trying to load isn't valid");
        }
    }

    public static void ClearCache()
    {
        if (Caching.ClearCache())
        {
            Singleton<InternalModBot.Logger>.Instance.Log("Successfully cleared the cache.");
            return;
        }
        Singleton<InternalModBot.Logger>.Instance.Log("Cache is being used.");
    }

    public List<Mod> Mods = new List<Mod>();

    public Mod PassOnMod = new PassOnToModsManager();
}