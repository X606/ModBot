/*
 * New mod loading system
 */

using ICSharpCode.SharpZipLib.Zip;
using ModLibrary;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace InternalModBot
{
    /// <summary>
    /// Handles mod loading, disableing and enableing.
    /// </summary>
    public class ModsManager : Singleton<ModsManager>
    {
        /// <summary>
        /// The name of the json file containing the mod data
        /// </summary>
        public const string MOD_INFO_FILE_NAME = "ModInfo.json";

        /// <summary>
        /// The name of the mods folder
        /// </summary>
        public const string MOD_FOLDER_NAME = "mods";

        private static readonly Dictionary<string, ModAssemblyCache> _cachedAssemblies = new Dictionary<string, ModAssemblyCache>();
        private static List<ModInfo> _cachedModInfos;

        private static bool _hasLoadedModsBefore;

        private readonly List<LoadedModInfo> _loadedMods = new List<LoadedModInfo>();
        private readonly List<Mod> _allLoadedActiveMods = new List<Mod>();

        /// <summary>
        /// The "pass on mod" that calls everything called on it on all loaded mods
        /// </summary>
        public PassOnToModsManager PassOnMod = new PassOnToModsManager();

        /// <summary>
        /// Gets the mod folder path
        /// </summary>
        public string ModFolderPath => Path.Combine(InternalUtils.GetSubdomain(Application.dataPath), MOD_FOLDER_NAME + "/");

        /// <summary>
        /// Initializes the mods manager
        /// </summary>
        public void Initialize()
        {
            ReloadMods();
        }

        private void Update()
        {
            PassOnMod.GlobalUpdate();
            ThreadedDelegateScheduler.Update();
        }

        /// <summary>
        /// Reloads all loaded mods
        /// </summary>
        public void ReloadMods()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            List<ModLoadError> errors = new List<ModLoadError>();

            bool reloadMods = _hasLoadedModsBefore;
            if (reloadMods)
            {
                unloadAllLoadedMods();
                loadMods(_cachedModInfos, ref errors);
            }
            else
            {
                _hasLoadedModsBefore = true;
                prepareAndLoadMods(ref errors);
            }

            stopwatch.Stop();
            debug.Log($"{(reloadMods ? "Reloaded" : "Loaded")} {_loadedMods.Count} mods in {stopwatch.Elapsed.TotalSeconds} seconds with {errors.Count} errors");

            if (errors.Count != 0) StartCoroutine(showErrorsCoroutine(errors));
        }

        /// <summary>
        /// Load only unpacked mods
        /// </summary>
        public void LoadNewMods()
        {
            List<ModLoadError> errors = new List<ModLoadError>();

            loadNewMods(ref errors);

            if (errors.Count != 0) StartCoroutine(showErrorsCoroutine(errors));
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

        private List<string> unpackMods()
        {
            List<string> unpackedModPaths = new List<string>();

            string[] zipFiles = Directory.GetFiles(ModFolderPath, "*.zip");
            foreach (string zipFilePath in zipFiles)
            {
                string newDirectory = Path.Combine(ModFolderPath, Path.GetFileNameWithoutExtension(zipFilePath));
                unpackedModPaths.Add(newDirectory);

                Directory.CreateDirectory(newDirectory);
                FastZip fastZip = new FastZip();
                fastZip.ExtractZip(zipFilePath, newDirectory, null);
                File.Delete(zipFilePath);

                debug.Log("Unpacked " + System.IO.Path.GetFileName(zipFilePath) + "...");
            }

            return unpackedModPaths;
        }

        private List<ModInfo> readModInfos(ref List<ModLoadError> errors, List<string> forceDirectories)
        {
            List<ModInfo> modInfos = new List<ModInfo>();

            List<string> directories = forceDirectories == null ? new List<string>(Directory.GetDirectories(ModFolderPath)) : forceDirectories;
            foreach (string folderPath in directories)
            {
                if (!loadModInfo(folderPath, out ModInfo newModInfo, out ModLoadError error))
                {
                    if (error != null) errors.Add(error);
                    continue;
                }

                // check if we're loading the same mod or its old/new version
                bool hasSameModBeenAlreadyLoaded = false;
                ModInfo versionToRemove = null;
                foreach (ModInfo installedModInfo in modInfos)
                {
                    if (newModInfo.UniqueID != installedModInfo.UniqueID) continue;

                    if (newModInfo.Version > installedModInfo.Version) versionToRemove = installedModInfo;
                    else if (newModInfo.Version < installedModInfo.Version) versionToRemove = newModInfo;
                    else if (newModInfo.Version == installedModInfo.Version) hasSameModBeenAlreadyLoaded = true;

                    break;
                }
                if (versionToRemove != null)
                {
                    if (versionToRemove == newModInfo) continue;

                    modInfos.Remove(versionToRemove);
                }

                if (hasSameModBeenAlreadyLoaded)
                {
                    errors.Add(new ModLoadError(newModInfo, "Mod with the same ID has already been loaded"));
                    continue;
                }

                modInfos.Add(newModInfo);
            }

            return modInfos;
        }

        private void checkModDependencies(ref List<ModLoadError> errors, List<ModInfo> modsToLoad)
        {
            List<ModInfo> allMods = new List<ModInfo>(modsToLoad);
            for (int i = modsToLoad.Count - 1; i >= 0; i--)
            {
                ModInfo modInfo = modsToLoad[i];
                if (!modInfo.IsModEnabled) continue;

                string[] dependencies = modInfo.ModDependencies;
                if (dependencies == null || dependencies.Length == 0) continue;

                List<string> missingMods = new List<string>();
                foreach (string dependencyId in dependencies)
                {
                    ModInfo dependecy = getModInfoFromList(dependencyId, ref modsToLoad);
                    if (dependecy == null || !dependecy.IsModEnabled)
                        missingMods.Add(dependencyId);
                }

                if (missingMods.Count > 0)
                {
                    //TODO: get the mod name from the site
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine($"\"{modInfo.DisplayName}\" requires following mods to be installed and enabled:");
                    foreach (string id in missingMods)
                    {
                        ModInfo info = null;
                        foreach (ModInfo mi in allMods)
                            if (mi.UniqueID == id)
                            {
                                info = mi;
                                break;
                            }

                        if (info == null)
                        {
                            stringBuilder.AppendLine($"A mod with id {id} (missing)".AddColor(Color.red));
                        }
                        else
                        {
                            stringBuilder.AppendLine($"{info.DisplayName} (disabled)".AddColor(Color.yellow));
                        }
                    }
                    stringBuilder.Append("Check mod's description for information or ask around Discord how to fix this.");
                    stringBuilder.AppendLine();

                    errors.Add(new ModLoadError(modInfo, stringBuilder.ToString()));

                    modsToLoad.Remove(modInfo);
                }
            }
        }

        private List<ModInfo> sortMods(List<ModInfo> modsToLoad)
        {
            List<ModInfo> modsToLoadSorted = new List<ModInfo>();
            for (int i = 0; i < modsToLoad.Count; i++)
            {
                ModInfo modInfo = modsToLoad[i];
                addModDependenciesRecursive(modInfo, ref modsToLoad, ref modsToLoadSorted);
            }
            return modsToLoadSorted;
        }

        private void prepareAndLoadMods(ref List<ModLoadError> errors)
        {
            // First we take a pass to see if there any zip files we want to convert into folders
            unpackMods();

            // Get all mod infos
            List<ModInfo> modInfos = readModInfos(ref errors, null);

            // Check if there're any missing mods
            checkModDependencies(ref errors, modInfos);

            _cachedModInfos = sortMods(modInfos);
            loadMods(_cachedModInfos, ref errors);
        }

        private void loadNewMods(ref List<ModLoadError> errors)
        {
            List<string> newMods = unpackMods();

            List<ModInfo> modInfos = readModInfos(ref errors, newMods);

            checkModDependencies(ref errors, modInfos);

            List<ModInfo> list = sortMods(modInfos);
            _cachedModInfos.AddRange(list);

            loadMods(list, ref errors);
        }

        private void loadMods(List<ModInfo> modsToLoad, ref List<ModLoadError> errors)
        {
            for (int i = 0; i < modsToLoad.Count; i++)
            {
                ModInfo modInfo = modsToLoad[i];
                if (!modInfo.IsModEnabled)
                {
                    _loadedMods.Add(new LoadedModInfo(null, modInfo));
                    continue;
                }

                if (!loadMod(modInfo, out ModLoadError error))
                    errors.Add(error);
            }
            RefreshAllLoadedActiveMods();
        }

        private bool loadModInfo(string folderPath, out ModInfo modInfo, out ModLoadError error)
        {
            if (folderPath.EndsWith("/") || folderPath.EndsWith("\\"))
                folderPath = folderPath.Remove(folderPath.Length - 1);

            if (!Directory.Exists(folderPath))
            {
                error = new ModLoadError(folderPath, "Could not find folder");
                modInfo = null;
                return false;
            }

            string modInfoFilePath = Path.Combine(folderPath, MOD_INFO_FILE_NAME);
            if (!File.Exists(modInfoFilePath))
            {
                //error = new ModLoadError(folderPath, "Could not find the \"" + MOD_INFO_FILE_NAME + "\" file");
                error = null; // if the folder doesnt have a mod info file we can just treat it as some random folder we don't care about.
                modInfo = null;
                return false;
            }

            string modInfoJson = File.ReadAllText(modInfoFilePath);
            try
            {
                modInfo = JsonConvert.DeserializeObject<ModInfo>(modInfoJson);
            }
            catch (JsonException e)
            {
                error = new ModLoadError(folderPath, "Error deserializing " + MOD_INFO_FILE_NAME + ": \"" + e.Message + "\"");
                modInfo = null;
                return false;
            }

            if (!modInfo.AreAllEssentialFieldsAssigned(out string msg))
            {
                error = new ModLoadError(folderPath, msg);
                return false;
            }
            modInfo.FixFieldValues();
            modInfo.FolderPath = folderPath + "/";

            error = null;
            return true;
        }

        internal void LoadMod(ModInfo modInfo)
        {
            if (!loadMod(modInfo, out ModLoadError error)) showError(error);
        }

        private bool loadMod(ModInfo modInfo, out ModLoadError error)
        {
            string dllPath = modInfo.DLLPath;
            if (!File.Exists(dllPath))
            {
                error = new ModLoadError(modInfo, "The file \"" + modInfo.MainDLLFileName + "\" could not be found");
                return false;
            }

            Assembly loadedAssembly = null;
            Type mainType = null;
            if (_cachedAssemblies.ContainsKey(modInfo.UniqueID))
            {
                ModAssemblyCache modAssemblyCache = _cachedAssemblies[modInfo.UniqueID];
                loadedAssembly = modAssemblyCache.LoadedAssembly;
                mainType = modAssemblyCache.MainClassType;
            }
            else
            {
                try
                {
                    loadedAssembly = Assembly.LoadFile(dllPath);
                }
                catch
                {
                    error = new ModLoadError(modInfo, "Could not load \"" + modInfo.MainDLLFileName + "\"");
                    return false;
                }

                Type[] types = loadedAssembly.GetTypes();
                foreach (Type type in types)
                {
                    if (type.GetCustomAttribute<MainModClassAttribute>(true) != null)
                    {
                        mainType = type;
                    }
                }

                if (mainType == null)
                {
                    error = new ModLoadError(modInfo, "Could not find type with the " + nameof(MainModClassAttribute) + " attribute");
                    return false;
                }

                if (mainType.BaseType != typeof(Mod))
                {
                    error = new ModLoadError(modInfo, "The type with the " + nameof(MainModClassAttribute) + " attribute was not of type " + nameof(Mod));
                    return false;
                }

                string relativePath = InternalUtils.GetRelativePathFromFullPath(modInfo.FolderPath);
                DataSaver.TryLoadDataFromFile(relativePath);

                ModAssemblyCache modAssemblyCache = new ModAssemblyCache()
                {
                    LoadedAssembly = loadedAssembly,
                    MainClassType = mainType
                };
                _cachedAssemblies.Add(modInfo.UniqueID, modAssemblyCache);
            }

            Mod loadedMod = (Mod)Activator.CreateInstance(mainType);
            loadedMod.SourceAssembly = loadedAssembly;

            LoadedModInfo loadedModInfo = GetLoadedModWithID(modInfo.UniqueID);
            if (loadedModInfo == null)
            {
                loadedModInfo = new LoadedModInfo(loadedMod, modInfo);
                _loadedMods.Add(loadedModInfo);
            }
            else
            {
                loadedModInfo.ModReference = loadedMod;
            }

            try
            {
                loadedModInfo.AutoInject();
            }
            catch (Exception e)
            {
                error = new ModLoadError(modInfo, "Caught exception while applying patches, exception details: " + e.ToString());
                return false;
            }

            try
            {
                loadedMod.OnModLoaded();
            }
            catch (Exception e)
            {
                error = new ModLoadError(modInfo, "Caught exception in OnModLoaded, exception details: " + e.ToString());
                return false;
            }

            StartCoroutine(callOnModRefreshedNextFrame(loadedModInfo));

            error = null;
            return true;
        }

        private void unloadAllLoadedMods()
        {
            foreach (LoadedModInfo loadedMod in _loadedMods)
            {
                if (loadedMod != null && loadedMod.ModReference != null)
                    loadedMod.ModReference.OnModDeactivated();
            }

            _loadedMods.Clear();
        }

        private void addModDependenciesRecursive(ModInfo modInfo, ref List<ModInfo> modsToLoad, ref List<ModInfo> sortedList)
        {
            if (sortedList.Contains(modInfo)) return;

            if (modInfo.ModDependencies != null && modInfo.ModDependencies.Length != 0)
                foreach (string dependencyId in modInfo.ModDependencies)
                {
                    ModInfo dependencyInfo = getModInfoFromList(dependencyId, ref modsToLoad);
                    if (dependencyInfo == null) continue;

                    addModDependenciesRecursive(dependencyInfo, ref modsToLoad, ref sortedList);
                }

            sortedList.Add(modInfo);
        }

        private ModInfo getModInfoFromList(string id, ref List<ModInfo> modInfos)
        {
            if (modInfos == null || modInfos.Count == 0) return null;

            for (int i = 0; i < modInfos.Count; i++)
            {
                ModInfo modInfo = modInfos[i];
                if (modInfo.UniqueID == id) return modInfo;
            }
            return null;
        }

        private static IEnumerator callOnModRefreshedNextFrame(LoadedModInfo mod)
        {
            yield return null;

            try
            {
                mod.ModReference.OnModRefreshed();
            }
            catch (Exception exception)
            {
                throw new Exception("Exception in OnModRefreshed for \"" + mod.OwnerModInfo.DisplayName + "\" (ID: " + mod.OwnerModInfo.UniqueID + ")", exception);
            }

            try
            {
                mod.ModReference.OnModEnabled();
            }
            catch (Exception exception)
            {
                throw new Exception("Exception in OnModEnabled for \"" + mod.OwnerModInfo.DisplayName + "\" (ID: " + mod.OwnerModInfo.UniqueID + ")", exception);
            }
        }

        private IEnumerator showErrorsCoroutine(List<ModLoadError> errors)
        {
            for (int i = 0; i < errors.Count; i++)
            {
                showError(errors[i]);

                while (Generic2ButtonDialogue.IsWindowOpen) yield return null;
            }
        }

        private void showError(ModLoadError error)
        {
            ColorUtility.TryParseHtmlString(LocalModInfoDisplay.VERSION_COLOR, out Color color);

            if (error.Info == null)
            {
                if (!string.IsNullOrEmpty(error.FolderPath) && Directory.Exists(error.FolderPath))
                {
                    new Generic2ButtonDialogue($"Mod \"{error.ModName.AddColor(color)}\" could not be loaded. Error:\n{error.ErrorMessage}", "Ok", null, "View mod's folder", delegate
                    {
                        _ = Process.Start(error.FolderPath);
                    });
                }
                else
                {
                    new Generic2ButtonDialogue($"Mod \"{error.ModName.AddColor(color)}\" could not be loaded. Error:\n{error.ErrorMessage}", "Ok", null, "Ok", null);
                }
            }
            else
            {
                new Generic2ButtonDialogue($"Mod \"{error.ModName.AddColor(color)}\" could not be loaded. Error:\n{error.ErrorMessage}\nDo you want to disable the mod?", "Yes", delegate
                {
                    try
                    {
                        error.Info.IsModEnabled = false;
                    }
                    catch { }
                }, "No", null);
            }
        }

        /// <summary>
        /// Returns the <see cref="ModInfo"/> assosiated with a specific mod, returns null if the mod is not loaded
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public ModInfo GetInfo(Mod mod)
        {
            foreach (LoadedModInfo modInfo in _loadedMods)
            {
                if (modInfo.ModReference == mod)
                    return modInfo.OwnerModInfo;
            }
            return null;
        }

        /// <summary>
        /// Refreshes the cache for what mods are active and loaded
        /// </summary>
        public void RefreshAllLoadedActiveMods()
        {
            _allLoadedActiveMods.Clear();

            foreach (LoadedModInfo modInfo in _loadedMods)
            {
                if (modInfo.IsEnabled)
                {
                    _allLoadedActiveMods.Add(modInfo.ModReference);
                }
            }
        }

        internal List<LoadedModInfo> GetAllMods()
        {
            return _loadedMods;
        }

        /// <summary>
        /// Gets all the currently loaded mods that are not disabled
        /// </summary>
        /// <returns></returns>
        public List<Mod> GetAllLoadedActiveMods()
        {
            return _allLoadedActiveMods;
        }

        /// <summary>
        /// Returns infos of all active mods
        /// </summary>
        /// <returns></returns>
        public List<ModInfo> GetActiveModInfos()
        {
            List<ModInfo> modInfos = new List<ModInfo>();
            foreach (LoadedModInfo modInfo in _loadedMods)
            {
                if (modInfo.OwnerModInfo.IsModEnabled)
                    modInfos.Add(modInfo.OwnerModInfo);
            }

            return modInfos;
        }

        internal LoadedModInfo GetLoadedModWithID(string modID)
        {
            foreach (LoadedModInfo modInfo in _loadedMods)
            {
                if (modInfo.OwnerModInfo.UniqueID == modID)
                    return modInfo;
            }

            return null;
        }

        internal LoadedModInfo GetLoadedModInstanceForAssembly(Assembly assembly)
        {
            foreach (LoadedModInfo loadedMod in _loadedMods)
            {
                // Only search through loaded mods
                if (!loadedMod.IsEnabled)
                    continue;

                if (loadedMod.ModReference.SourceAssembly == assembly)
                    return loadedMod;
            }

            return null;
        }

        internal class ModAssemblyCache
        {
            public Assembly LoadedAssembly;

            public Type MainClassType;
        }
    }
}