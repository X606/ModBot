using InternalModBot;
using ModLibrary;
using System;
using System.Collections;
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

            foreach (LoadedMod mod in _mods)
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
            _mods.Clear();
            PassOnMod = new PassOnToModsManager();
            string[] files = Directory.GetFiles(getModsFolderPath());
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
                        string file = files[i];
                        DelegateScheduler.Instance.Schedule(delegate
                        {
                            Debug.LogError("Mod '" + file + "' is not working, make sure that it is set up right: " + ex.Message);
                        }, 0.5f);
                    }
                }
            }
        }

        void Update()
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
            LoadedMod loadedMod = null;
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
                    throw new Exception("Could not find class 'main'");

                object obj = Activator.CreateInstance(type);

                Mod modToLoad = obj as Mod;
                _mods.ForEach(delegate (LoadedMod mod)
                {
                    if (mod.Mod.GetUniqueID() == modToLoad.GetUniqueID())
                    {
                        throw new Exception("2 or more mods (\"" + mod.Mod.GetModName() + "\" and \"" + modToLoad.GetModName() + "\" have the same UniqueID!");
                    }
                });

                loadedMod = new LoadedMod(modToLoad, assemblyData, hasFile);
                _mods.Add(loadedMod);

                bool isActive = PlayerPrefs.GetInt(modToLoad.GetUniqueID(), 1) != 0;
                if (!isActive)
                {
                    DisableMod(modToLoad);
                }
                else
                {
                    try
                    {
                        StartCoroutine(callOnModRefreshedNextFrame(modToLoad));
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Caught exception in OnModRefreshed for mod \"" + modToLoad.GetModName() + "\" with ID \"" + modToLoad.GetUniqueID() + "\": " + exception.Message);
                    }
                }

            }
            catch (Exception e)
            {
                if (!hasFile && loadedMod != null)
                {
                    if (loadedMod.Mod != null) // Just in case
                    {
                        loadedMod.Mod.OnModDeactivated(); // Called so that the mod will (hopefully) clean up anything it has already done
                    }

                    _mods.Remove(loadedMod); // If the mod was sent over the network and something went wrong, unload the mod
                }

                throw new Exception("The mod you are trying to load isn't valid: " + e.Message);
            }

        }

        static IEnumerator callOnModRefreshedNextFrame(Mod mod)
        {
            yield return 0;
            mod.OnModRefreshed();
            mod.OnModEnabled();
        }

        /// <summary>
        /// Adds the mod to the mods folder
        /// </summary>
        /// <param name="mod"></param>
        public void WriteDllFileToModFolder(Mod mod)
        {
            if (GetIsModOnlyLoadedInMemory(mod) == false)
                return;

            string filename = verifyName(mod.GetModName());
            string fullPath = getModsFolderPath() + filename + ".dll";
            if (File.Exists(fullPath))
            {
                debug.Log("The file \"" + fullPath + "\" already existed", Color.red);
                return;
            }

            File.WriteAllBytes(fullPath, GetModData(mod));

            foreach (LoadedMod _mod in _mods)
            {
                if (_mod.Mod == mod)
                {
                    _mod.IsOnlyLoadedInMemory = false;
                    return;
                }
            }
        }

        static string getModsFolderPath()
        {
            return AssetLoader.GetSubdomain(Application.dataPath) + "mods/";
        }

        static string verifyName(string oldName)
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
            foreach (LoadedMod mod in _mods)
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
            foreach (LoadedMod mod in _mods)
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

            for (int i = 0; i < _mods.Count; i++)
            {
                if (_mods[i].Mod == mod)
                {
                    _mods[i].IsDeactivated = true;
                    break;
                }
            }
            CustomUpgradeManager.NextClicked();
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

            for (int i = 0; i < _mods.Count; i++)
            {
                if (_mods[i].Mod == mod)
                {
                    _mods[i].IsDeactivated = false;
                    break;
                }
            }

            mod.OnModEnabled();
        }

        /// <summary>
        /// Checks if a mod is deactivated
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public bool? IsModDeactivated(Mod mod)
        {
            for (int i = 0; i < _mods.Count; i++)
            {
                if (_mods[i].Mod == mod)
                {
                    return _mods[i].IsDeactivated;
                }
            }

            return null;
        }

        internal byte[] GetModData(Mod mod)
        {
            for (int i = 0; i < _mods.Count; i++)
            {
                if (_mods[i].Mod == mod)
                {
                    return _mods[i].RawAssemblyData;
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
            for (int i = 0; i < _mods.Count; i++)
            {
                if (_mods[i].Mod == mod)
                {
                    return _mods[i].IsOnlyLoadedInMemory;
                }
            }
            return false;
        }

        List<LoadedMod> _mods = new List<LoadedMod>();

        /// <summary>
        /// A very special mod that will call all mods the most functions passed to it on all mods
        /// </summary>
        public Mod PassOnMod = new PassOnToModsManager();
    }
}