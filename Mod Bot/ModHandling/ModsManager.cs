using InternalModBot;
using ModLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using Newtonsoft.Json;

namespace InternalModBot
{
    /// <summary>
    /// Handles mod loading, disableing and enableing.
    /// </summary>
    public class ModsManager : Singleton<ModsManager>
    {
        /// <summary>
        /// A very special mod that will call all mods the most functions passed to it on all mods
        /// </summary>
        internal Mod PassOnMod = new PassOnToModsManager();

        static Dictionary<Mod, LoadedModInfo> _modsData = new Dictionary<Mod, LoadedModInfo>();

        /// <summary>
        /// Loads all mods from the mods directory and deactivates remembered deactivated mods
        /// </summary>
        public void Initialize()
        {
            ReloadMods();
        }

        /// <summary>
        /// Clears all loaded mods and loads them again
        /// </summary>
        public void ReloadMods()
        {
            UpgradePagesManager.Reset();
            ClearCache();

            foreach (LoadedModInfo modData in _modsData.Values)
            {
                modData.Unload();
            }

            _modsData.Clear();

            PassOnMod = new PassOnToModsManager();

            List<ModLoadError> errors = new List<ModLoadError>();

            string[] modFolders = Directory.GetDirectories(getModsFolderPath());
            foreach (string modFolder in modFolders)
            {
                string modInfoFile = modFolder + "/ModInfo.json";
                if (!File.Exists(modInfoFile))
                    continue;

                if (!tryLoadModFromFolder(modFolder, out ModLoadError loadError))
                    errors.Add(loadError);
            }

            /*
            string[] files = Directory.GetFiles(getModsFolderPath());
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".dll"))
                {
                    byte[] modData = File.ReadAllBytes(files[i]);
                    if (!LoadMod(modData, Path.GetFileNameWithoutExtension(files[i]), true, out string error))
                    {
                        errors.Add(error);
                        invalidModsFilePaths.Add(files[i]);
                    }
                }
            }
            */

            if (errors.Count > 0)
                StartCoroutine(showModInvalidMessage(errors));
        }

        static IEnumerator showModInvalidMessage(List<ModLoadError> errors)
        {
            for (int i = 0; i < errors.Count; i++)
            {
                new Generic2ButtonDialogue("Mod \"" + errors[i].ModName + "\" could not be loaded (" + errors[i].ErrorMessage + "). Do you want to remove the mod?",
                    "Yes",
                    delegate
                    {
                        Directory.Delete(errors[i].FolderPath, true);
                    },
                    "No", null);

                yield return new WaitWhile(delegate { return Generic2ButtonDialogue.IsWindowOpen; });
            }
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.R))
                ReloadMods();

            PassOnMod.GlobalUpdate();
        }

        bool tryLoadModFromFolder(string folderPath, out ModLoadError error)
        {
            string modInfoContent = File.ReadAllText(folderPath + "/ModInfo.json");

            ModInfo modInfo;
            try
            {
                modInfo = JsonConvert.DeserializeObject<ModInfo>(modInfoContent);
            }
            catch (Exception e)
            {
                error = new ModLoadError(folderPath, "ModInfo.json: Caught exception while deserializing: " + e.Message);
                return false;
            }

            if (modInfo == null)
            {
                error = new ModLoadError(folderPath, "ModInfo.json: Deserialized to null value");
                return false;
            }

            if (!modInfo.AreAllEssentialFieldsAssigned(out string errorMessage))
            {
                if (!string.IsNullOrWhiteSpace(modInfo.DisplayName))
                {
                    error = new ModLoadError(folderPath, modInfo.DisplayName, errorMessage);
                }
                else
                {
                    error = new ModLoadError(folderPath, errorMessage);
                }

                return false;
            }

            modInfo.FixFieldValues();
            modInfo.FolderPath = folderPath + "/";

            if (!File.Exists(modInfo.DLLPath))
            {
                error = new ModLoadError(modInfo, "ModInfo.json: Main dll does not exist");
                return false;
            }

            if (!LoadMod(modInfo, out error))
                return false;

            error = null;
            return true;
        }

        internal AppDomain GetAppDomainForMod(ModInfo info)
        {
            foreach (KeyValuePair<Mod, LoadedModInfo> modDataPair in _modsData)
            {
                if (modDataPair.Key.modInfo.DisplayName == info.DisplayName)
                {
                    if (modDataPair.Value.AppDomain == null)
                        break;

                    return modDataPair.Value.AppDomain;
                }
            }

            AppDomainSetup domainInfo = new AppDomainSetup
            {
                ApplicationBase = info.FolderPath
            };

            AppDomain domain = AppDomain.CreateDomain(info.DisplayName, null, domainInfo);
            return domain;
        }

        public bool LoadMod(ModInfo modInfo, out ModLoadError error)
        {
            AppDomain appDomain = GetAppDomainForMod(modInfo);
            Assembly assembly = appDomain.Load(File.ReadAllBytes(modInfo.DLLPath));

            Type[] types = assembly.GetTypes();
            Type mainType = null;
            foreach (Type type in types)
            {
                if (type.BaseType == typeof(Mod) && type.Name.ToLower() == "main")
                {
                    mainType = type;
                    break;
                }
            }

            if (mainType == null)
            {
                error = new ModLoadError(modInfo, "Could not find type \"Main\"");
                return false;
            }

            object modObj = Activator.CreateInstance(mainType);
            if (!(modObj is Mod modInstance))
            {
                error = new ModLoadError(modInfo, "Type \"Main\" was not of type \"Mod\"");
                return false;
            }

            foreach (LoadedModInfo loadedMod in _modsData.Values)
            {
                if (!loadedMod.IsEnabled)
                    continue;

                if (loadedMod.OwnerModInfo.UniqueID == modInfo.UniqueID)
                {
                    error = new ModLoadError(modInfo, "Mod has same UniqueID as \"" + loadedMod.OwnerModInfo.DisplayName + "\"");
                    return false;
                }
            }

            LoadedModInfo loadedModInfo = new LoadedModInfo(modInstance, modInfo, appDomain);
            _modsData.Add(modInstance, loadedModInfo);

            if (loadedModInfo.IsEnabled)
            {
                try
                {
                    modInstance.OnModLoaded();
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Exception in OnModLoaded for \"" + modInfo.DisplayName + "\" (ID: " + modInfo.UniqueID + "), mod not loaded");
                    Console.WriteLine(exception);
                    error = new ModLoadError(modInfo, "Caught exception in OnModLoaded: (" + exception.Message + "), full exception message written to output_log.txt");
                    return false;
                }

                StartCoroutine(callOnModRefreshedNextFrame(modInstance));
            }

            error = null;
            return true;
        }

        static IEnumerator callOnModRefreshedNextFrame(Mod mod)
        {
            yield return 0;

            try
            {
                mod.OnModRefreshed();
            }
            catch (Exception exception)
            {
                throw new Exception("Exception in OnModRefreshed for \"" + mod.modInfo.DisplayName + "\" (ID: " + mod.modInfo.UniqueID + ")", exception);
            }

            try
            {
                mod.OnModEnabled();
            }
            catch (Exception exception)
            {
                throw new Exception("Exception in OnModEnabled for \"" + mod.modInfo.DisplayName + "\" (ID: " + mod.modInfo.UniqueID + ")", exception);
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

            string filename = verifyName(mod.GetModName());
            string fullPath = getModsFolderPath() + filename + ".dll";
            if (File.Exists(fullPath))
            {
                debug.Log("The file \"" + fullPath + "\" already existed", Color.red);
                return;
            }

            File.WriteAllBytes(fullPath, GetModData(mod));

            foreach (LoadedModInfo _mod in _mods)
            {
                if (_mod._modReference == mod)
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
                ModBotLocalizationManager.LogLocalizedStringOnceLocalizationManagerInitialized("clear_cache_success");
                return;
            }
            ModBotLocalizationManager.LogLocalizedStringOnceLocalizationManagerInitialized("clear_cache_fail");
        }

        /// <summary>
        /// Gets a list of all mods that should currently be active
        /// </summary>
        /// <returns></returns>
        public List<Mod> GetAllLoadedMods()
        {
            List<Mod> mods = new List<Mod>();
            foreach (LoadedModInfo mod in _mods)
            {
                if (mod.IsEnabled)
                {
                    mods.Add(mod._modReference);
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
            foreach (LoadedModInfo mod in _mods)
            {
                mods.Add(mod._modReference);
            }
            return mods;
        }

        /// <summary>
        /// Disables a mod, this will call on OnModDeactivated on the mod, and Mod-Bot will not make any more calls to the mod until activated again 
        /// </summary>
        /// <param name="mod"></param>
        public static void DisableMod(Mod mod)
        {
            if (_modsData != null && _modsData.TryGetValue(mod, out LoadedModInfo modInfo))
                modInfo.IsEnabled = false;
        }
        
        /// <summary>
        /// Enables a mod, this will make Mod-Bot start calling it again and also call OnModRefreshed on it
        /// </summary>
        /// <param name="mod"></param>
        public static void EnableMod(Mod mod)
        {
            if (_modsData != null && _modsData.TryGetValue(mod, out LoadedModInfo modInfo))
                modInfo.IsEnabled = true;
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

        public static bool IsModEnabled(Mod mod)
        {
            if (_modsData != null && _modsData.TryGetValue(mod, out LoadedModInfo modInfo))
                return modInfo.IsEnabled;

            return false;
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
    }
}