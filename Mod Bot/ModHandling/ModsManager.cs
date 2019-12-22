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

            List<string> errors = new List<string>();
            List<string> invalidModsFilePaths = new List<string>();

            string[] files = Directory.GetFiles(getModsFolderPath());
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".dll"))
                {
                    byte[] modData = File.ReadAllBytes(files[i]);
                    if (!LoadMod(modData, true, out string error))
                    {
                        errors.Add(error);
                        invalidModsFilePaths.Add(files[i]);
                    }
                }
            }

            if (errors.Count > 0)
                StartCoroutine(showModInvalidMessage(invalidModsFilePaths, errors));
        }

        static IEnumerator showModInvalidMessage(List<string> filePaths, List<string> errors)
        {
            if (filePaths.Count != errors.Count)
                throw new ArgumentException(nameof(filePaths) + " and " + nameof(errors) + " lists must have the same length");

            for (int i = 0; i < filePaths.Count; i++)
            {
                string fileName = Path.GetFileName(filePaths[i]);
                new Generic2ButtonDialogue("Mod \"" + fileName + "\" could not be loaded, do you want to remove the file? Error: " + errors[i],
                    "Yes",
                    delegate
                    {
                        File.Delete(filePaths[i]);
                    },
                    "No", null);

                yield return new WaitWhile(delegate { return Generic2ButtonDialogue.IsWindowOpen; });
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
        /// <param name="errorMessage"></param>
        public bool LoadMod(byte[] assemblyData, bool hasFile, out string errorMessage)
        {
            Type[] types = Assembly.Load(assemblyData).GetTypes();
            Type mainType = null;
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].Name.ToLower() == "main")
                {
                    mainType = types[i];
                }
            }

            if (mainType == null)
            {
                errorMessage = "Main class not found";
                return false;
            }

            object obj = Activator.CreateInstance(mainType);

            Mod modToLoad = obj as Mod;
            foreach (LoadedMod mod in _mods)
            {
                if (mod.Mod.GetUniqueID() == modToLoad.GetUniqueID())
                {
                    errorMessage = "Mod has the same UniqueID as \"" + mod.Mod.GetModName() + "\"";
                    return false;
                }
            }

            LoadedMod loadedMod = new LoadedMod(modToLoad, assemblyData, hasFile);
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
                    throw new Exception("Caught exception in OnModRefreshed or OnModEnabled for mod \"" + modToLoad.GetModName() + "\" with ID \"" + modToLoad.GetUniqueID() + "\": " + exception.Message);
                }
            }

            errorMessage = null;
            return true;
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
                Logger.Instance.Log(LocalizationManager.Instance.GetTranslatedString("clear_cache_success"));
                return;
            }
            Logger.Instance.Log(LocalizationManager.Instance.GetTranslatedString("clear_cache_fail"));
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