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
        /// <summary>
        /// Loads all mods from the mods directory and deactivates remembered deactivated mods
        /// </summary>
        public void Initialize()
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
            string[] files = Directory.GetFiles(GetModsFolderPath());
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".dll"))
                {
                    try
                    {
                        byte[] modData = File.ReadAllBytes(files[i]);
                        LoadMod(modData, true);
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
        /// <param name="hasFile"></param>
        public void LoadMod(byte[] assemblyData, bool hasFile)
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

                mods.Add(new LoadedMod(loadedMod, assemblyData, hasFile));

                bool isActive = PlayerPrefs.GetInt(loadedMod.GetUniqueID(), 1) != 0;
                if (!isActive)
                {
                    DisableMod(loadedMod);
                }
                else
                {
                    try
                    {
                        loadedMod.OnModRefreshed();
                    }
                    catch(Exception exception)
                    {
                        throw new Exception("Caught exception in OnModRefreshed for mod \"" + loadedMod.GetModName() + "\" with ID \"" + loadedMod.GetUniqueID() + "\": " + exception.Message);
                    }
                }

                
            }
            catch(Exception e)
            {
                throw new Exception("The mod you are trying to load isn't valid: " + e.Message);
            }

            
        }


        /// <summary>
        /// Adds the mod to the mods folder
        /// </summary>
        /// <param name="mod"></param>
        public void WriteDllFileToModFolder(Mod mod)
        {
            if (GetIsModOnlyLoadedInMemory(mod) == false)
                return;

            string filename = VerifyName(mod.GetModName());
            string fullPath = GetModsFolderPath() + filename + ".dll";
            if (File.Exists(fullPath))
            {
                debug.Log("The file \"" + fullPath + "\" already existed", Color.red);
                return;
            }

            File.WriteAllBytes(fullPath, GetModData(mod));

            foreach (LoadedMod _mod in mods)
            {
                if (_mod.Mod == mod)
                {
                    _mod.IsOnlyLoadedInMemory = false;
                    return;
                }
            }
        }

        private string GetModsFolderPath()
        {
            return AssetLoader.GetSubdomain(Application.dataPath) + "mods/";
        }

        private string VerifyName(string oldName)
        {
            return oldName.Trim("<>:\"\\/|?*".ToCharArray());
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

        internal byte[] GetModData(Mod mod)
        {
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].Mod == mod)
                {
                    return mods[i].RawAssemblyData;
                }
            }
            return null;
        }
        /// <summary>
        /// Returns true if the passed mod doesnt have a file to load from
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public bool GetIsModOnlyLoadedInMemory(Mod mod)
        {
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].Mod == mod)
                {
                    return mods[i].IsOnlyLoadedInMemory;
                }
            }
            return false;
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
        private LoadedMod() // this will prevent people from createing now LoadedMod instances in mods
        {
        }
        /// <summary>
        /// Sets the mod field to the passed mod, and will not deactivate the mod
        /// </summary>
        /// <param name="_mod"></param>
        /// <param name="_rawAssemblyData"></param>
        /// <param name="isLoadedFromFile"></param>
        internal LoadedMod(Mod _mod, byte[] _rawAssemblyData, bool isLoadedFromFile)
        {
            Mod = _mod;
            IsDeactivated = false;
            RawAssemblyData = _rawAssemblyData;
            IsOnlyLoadedInMemory = !isLoadedFromFile;
        }
        /// <summary>
        /// The Mod object the class is holding
        /// </summary>
        public Mod Mod;
        /// <summary>
        /// Decides if the mod is deactivated.
        /// </summary>
        public bool IsDeactivated;

        /// <summary>
        /// If this mod doesnt have a file
        /// </summary>
        public bool IsOnlyLoadedInMemory;

        internal readonly byte[] RawAssemblyData;
    }
}