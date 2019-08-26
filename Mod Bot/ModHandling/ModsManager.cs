using InternalModBot;
using ModLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace InternalModBot
{
    public class ModsManager : Singleton<ModsManager>
    {
        public void Start()
        {
            ReloadMods();

            foreach (LoadedMod mod in mods)
            {
                bool isActive = PlayerPrefs.GetInt(mod.Mod.GetUniqueID(), 1) != 0;
                if (!isActive)
                {
                    DisableMod(mod.Mod);
                }
            }

        }

        public void ReloadMods()
        {
            UpgradeCosts.Reset();
            UpgradePagesManager.Reset();
            ClearCache();
            AssetLoader.ClearCache();
            mods.Clear();
            PassOnMod = new PassOnToModsManager();
            string[] files = Directory.GetFiles(AssetLoader.GetSubdomain(Application.dataPath) + "mods/");
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".dll"))
                {
                    try
                    {
                        byte[] modData = File.ReadAllBytes(files[i]);
                        LoadMod(modData);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Mod '" + files[i] + "' is not working, make sure that it is set up right: " + ex.Message);
                    }
                }
            }
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.R))
            {
                ReloadMods();
            }
            PassOnMod.GlobalUpdate();
        }

        public void LoadMod(byte[] assemblyData)
        {
            try
            {
                Type[] types = Assembly.Load(assemblyData).GetTypes();
                Type type = null;
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].Name.ToLower() == "main")
                    {
                        type = types[i];
                    }
                }
                if (type == null)
                {
                    throw new Exception("could not find class 'main'");
                }
                object obj = Activator.CreateInstance(type);
                

                Mod loadedMod = obj as Mod;
                mods.ForEach(delegate (LoadedMod mod)
                {
                    if (mod.Mod.GetUniqueID() == loadedMod.GetUniqueID())
                    {
                        throw new Exception("2 or more mods (\"" + mod.Mod.GetModName() + "\" and \"" + loadedMod.GetModName() + "\" have the same UniqueID!");
                    }
                });

                mods.Add(new LoadedMod((Mod)obj));

                bool isActive = PlayerPrefs.GetInt(loadedMod.GetUniqueID(), 1) != 0;
                if (!isActive)
                {
                    DisableMod(loadedMod);
                } else
                {
                    ((Mod)obj).OnModRefreshed();
                }

                
            }
            catch(Exception e)
            {
                throw new Exception("The mod you are trying to load isn't valid: " + e.Message);
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

        public List<Mod> GetAllLoadedMods()
        {
            List<Mod> mods = new List<Mod>();
            foreach(LoadedMod mod in this.mods)
            {
                if (!mod.IsDeactivated)
                {
                    mods.Add(mod.Mod);
                }
            }
            return mods;
        }
        public List<Mod> GetAllMods()
        {
            List<Mod> mods = new List<Mod>();
            foreach (LoadedMod mod in this.mods)
            {
                mods.Add(mod.Mod);
            }
            return mods;
        }

        public void DisableMod(Mod mod)
        {
            PlayerPrefs.SetInt(mod.GetUniqueID(), 0);

            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].Mod == mod)
                {
                    mods[i].IsDeactivated = true;
                    break;
                }
            }
            CustomUpgradeManager.Instance.NextClicked();
            UpgradePagesManager.RemoveModdedUpgradesFor(mod);
            mod.OnModDeactivated();
        }
        public void EnableMod(Mod mod)
        {
            PlayerPrefs.SetInt(mod.GetUniqueID(), 1);

            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].Mod == mod)
                {
                    mods[i].IsDeactivated = false;
                    break;
                }
            }

            mod.OnModRefreshed();
        }
        public bool? IsModDeactivated(Mod mod)
        {
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].Mod == mod)
                {
                    return mods[i].IsDeactivated;
                }
            }

            return null;
        }
        private List<LoadedMod> mods = new List<LoadedMod>();

        public Mod PassOnMod = new PassOnToModsManager();
    }

    public class LoadedMod
    {
        public LoadedMod(Mod _mod)
        {
            Mod = _mod;
            IsDeactivated = false;
        }

        public Mod Mod;
        public bool IsDeactivated;
    }
}