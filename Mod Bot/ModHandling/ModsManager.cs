using InternalModBot;
using ModLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace InternalModBot
{
    /// <summary>
    /// Handles mod loading, disableing and enableing.
    /// </summary>
    public class ModsManager : Singleton<ModsManager>
    {
        private void Start()
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

        /// <summary>
        /// Clears all loaded mods and loads them again
        /// </summary>
        public void ReloadMods()
        {
            UpgradePagesManager.Reset();
            ClearCache();
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

        private void Update()
        {
            if (Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.R))
            {
                ReloadMods();
            }
            PassOnMod.GlobalUpdate();
        }

        /// <summary>
        /// Loads a mod from only the bytes making up the assembly
        /// </summary>
        /// <param name="assemblyData"></param>
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

        /// <summary>
        /// Clears all mod cache (including the AssetLoader cache)
        /// </summary>
        public static void ClearCache()
        {
            AssetLoader.ClearCache();
            if (Caching.ClearCache())
            {
                Logger.Instance.Log("Successfully cleared the cache.");
                return;
            }
            Logger.Instance.Log("Cache is being used.");
        }

        /// <summary>
        /// Gets a list of all mods that should currently be active
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Gets a list of all mods currently loaded, even mods that arent currently active
        /// </summary>
        /// <returns></returns>
        public List<Mod> GetAllMods()
        {
            List<Mod> mods = new List<Mod>();
            foreach (LoadedMod mod in this.mods)
            {
                mods.Add(mod.Mod);
            }
            return mods;
        }

        /// <summary>
        /// Disables a mod, this will call on OnModDeactivated on the mod, and Mod-Bot will not make any more calls to the mod until activated again 
        /// </summary>
        /// <param name="mod"></param>
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
        /// <summary>
        /// Enables a mod, this will make Mod-Bot start calling it again and also call OnModRefreshed on it
        /// </summary>
        /// <param name="mod"></param>
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
        /// <summary>
        /// Checks if a mod is deactivated
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
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

        /// <summary>
        /// A very special mod that will call all mods the most functions passed to it on all mods
        /// </summary>
        public Mod PassOnMod = new PassOnToModsManager();
    }

    /// <summary>
    /// Class used to keep both a mod and bool that decides if the mod is active in same list
    /// </summary>
    public class LoadedMod
    {
        /// <summary>
        /// Sets the mod field to the passed mod, and will not deactivate the mod
        /// </summary>
        /// <param name="_mod"></param>
        public LoadedMod(Mod _mod)
        {
            Mod = _mod;
            IsDeactivated = false;
        }
        /// <summary>
        /// The Mod object the class is holding
        /// </summary>
        public Mod Mod;
        /// <summary>
        /// Decides if the mod is deactivated.
        /// </summary>
        public bool IsDeactivated;
    }
}