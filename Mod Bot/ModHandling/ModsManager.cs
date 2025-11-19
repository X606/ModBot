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
using UnityEngine;
using UnityEngine.Networking;

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

        private readonly List<LoadedModInfo> _loadedMods = new List<LoadedModInfo>();
        private static readonly Dictionary<string, uint> _firstLoadedVersionOfMod = new Dictionary<string, uint>(); // this keeps track of what version of mods have been loaded, this is to make sure that if a new version of a mod gets loaded the user get alerted that they have to restart

        private static readonly Dictionary<string, Texture2D> _cachedModImages = new Dictionary<string, Texture2D>();
        private static readonly List<string> _processingModImages = new List<string>();

        // private List<string> _TEMPModsBeforeSort, _TEMPModsAfterSort;

        /// <summary>
        /// Gets the mod folder path
        /// </summary>
        public string ModFolderPath => InternalUtils.GetSubdomain(Application.dataPath) + MOD_FOLDER_NAME + "/";

        /// <summary>
        /// The "pass on mod" that calls everything called on it on all loaded mods
        /// </summary>
        public PassOnToModsManager PassOnMod = new PassOnToModsManager();

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

        private static IEnumerator showModInvalidMessage(List<ModLoadError> errors)
        {
            for (int i = 0; i < errors.Count; i++)
            {
                // TODO: This is a very bad way of handling errors, completely deleting the mod file is *very* rarely going to be what you want to do, would be much better to give the option to disable the mod, or just continue.
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

        /// <summary>
        /// Reloads all loaded mods
        /// </summary>
        public void ReloadMods()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<ModLoadError> errors = new List<ModLoadError>();
            if (!reloadAllMods(ref errors))
                StartCoroutine(showModInvalidMessage(errors));

            stopwatch.Stop();
            debug.Log("(re)loaded " + _loadedMods.Count + " mods in " + stopwatch.Elapsed.TotalSeconds + " seconds");
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

        private bool reloadAllMods(ref List<ModLoadError> errors)
        {
            unloadAllLoadedMods();

            // First we take a pass to see if there any zip files we want to convert into folders
            string[] zipFiles = Directory.GetFiles(ModFolderPath, "*.zip");
            foreach (string zipFilePath in zipFiles)
            {
                string newDirectory = ModFolderPath + System.IO.Path.GetFileNameWithoutExtension(zipFilePath);
                Directory.CreateDirectory(newDirectory);
                FastZip fastZip = new FastZip();
                fastZip.ExtractZip(zipFilePath, newDirectory, null);
                File.Delete(zipFilePath);

                debug.Log("Unpacked " + System.IO.Path.GetFileName(zipFilePath) + "...");
            }

            // Get all mod infos
            string[] folders = Directory.GetDirectories(ModFolderPath);
            List<ModInfo> modsToLoad = new List<ModInfo>();
            Dictionary<string, ModInfo> modIdToInfo = new Dictionary<string, ModInfo>();
            foreach (string folderPath in folders)
            {
                if (!loadModInfo(folderPath, out ModInfo modInfo, out ModLoadError error))
                {
                    errors.Add(error);
                    continue;
                }

                if (modInfo == null)
                    continue;

                if (hasModAlreadyBeenLoaded(modInfo))
                {
                    errors.Add(new ModLoadError(modInfo, "Mod with the same ID has already been loaded"));
                    continue;
                }

                modsToLoad.Add(modInfo);
                modIdToInfo.Add(modInfo.UniqueID, modInfo);
            }

            // Check if there're any missing mods
            for (int i = modsToLoad.Count - 1; i >= 0; i--)
            {
                ModInfo modInfo = modsToLoad[i];
                string[] dependencies = modInfo.ModDependencies;

                if (dependencies == null || dependencies.Length == 0) continue;

                List<string> missingMods = new List<string>();

                foreach (string dependencyId in dependencies)
                {
                    if (!modIdToInfo.ContainsKey(dependencyId) || !modIdToInfo[dependencyId].IsModEnabled)
                        missingMods.Add(dependencyId);
                }

                if (missingMods.Count > 0)
                {
                    //TODO: get the mod name from the site
                    string errorMsg = modInfo.DisplayName + " requires other mods: \"";
                    errorMsg += string.Join("\", \"", missingMods) + "\"\n Make sure all mods are installed and enabled.";

                    errors.Add(new ModLoadError(modInfo, errorMsg));

                    modsToLoad.RemoveAt(i);
                    modIdToInfo.Remove(modInfo.UniqueID);
                }
            }

            /*List<string> debugModIds = new List<string>();
            for (int i = 0; i < modsToLoad.Count; i++)
                debugModIds.Add(modsToLoad[i].UniqueID);
            _TEMPModsBeforeSort = new List<string>(debugModIds);*/

            // Sort mods to load
            List<ModInfo> modsToLoadSorted = new List<ModInfo>();
            for (int i = 0; i < modsToLoad.Count; i++)
            {
                ModInfo modInfo = modsToLoad[i];
                addModDependenciesRecursive(modInfo, ref modsToLoad, ref modIdToInfo, ref modsToLoadSorted);
            }

            /*debugModIds.Clear();
            for (int i = 0; i < modsToLoadSorted.Count; i++)
                debugModIds.Add(modsToLoadSorted[i].UniqueID);
            _TEMPModsAfterSort = new List<string>(debugModIds);*/

            for (int i = 0; i < modsToLoadSorted.Count; i++)
            {
                ModInfo modInfo = modsToLoadSorted[i];
                if (!modInfo.IsModEnabled)
                {
                    _loadedMods.Add(new LoadedModInfo(null, modInfo));
                    continue;
                }

                if (!loadMod(modInfo, out ModLoadError error))
                    errors.Add(error);
            }
            RefreshAllLoadedActiveMods();

            return errors.Count == 0;
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
            string modInfoFilePath = folderPath + "/" + MOD_INFO_FILE_NAME;
            if (!File.Exists(modInfoFilePath))
            {
                //error = new ModLoadError(folderPath, "Could not find the \"" + MOD_INFO_FILE_NAME + "\" file");
                error = null; // if the folder doesnt have a mod info file we can just treat it as some random folder we don't care about.
                modInfo = null;
                return true;
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

            if (_firstLoadedVersionOfMod.ContainsKey(modInfo.UniqueID))
            {
                if (_firstLoadedVersionOfMod[modInfo.UniqueID] != modInfo.Version)
                {
                    Generic2ButtonDialogue generic2ButtonDialogue = new Generic2ButtonDialogue("Some other version of the " + modInfo.DisplayName + " mod already been loaded, you will need to restart the game for the new version to work. Do you want to restart now?",
                        "No, not right now.", delegate
                        {

                        },
                        "Yes, restart now", delegate
                        {
                            Application.Quit();
                        });

                    generic2ButtonDialogue.SetColorOfFirstButton(Color.red);
                    generic2ButtonDialogue.SetColorOfSecondButton(Color.green);
                }
            }
            else
            {
                _firstLoadedVersionOfMod.Add(modInfo.UniqueID, modInfo.Version);
            }


            error = null;
            return true;
        }

        internal void LoadMod(ModInfo modInfo)
        {
            if (!loadMod(modInfo, out ModLoadError error))
            {
                StartCoroutine(showModInvalidMessage(new List<ModLoadError>() { error }));
            }
        }

        private bool loadMod(ModInfo modInfo, out ModLoadError error)
        {
            string dllPath = modInfo.DLLPath;
            if (!File.Exists(dllPath))
            {
                error = new ModLoadError(modInfo, "The file \"" + modInfo.MainDLLFileName + "\" could not be found");
                return false;
            }
            Assembly loadedAssembly;
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
            Type mainType = null;
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

            Mod loadedMod = (Mod)Activator.CreateInstance(mainType);
            loadedMod.SourceAssembly = loadedAssembly;

            string relativePath = InternalUtils.GetRelativePathFromFullPath(modInfo.FolderPath);

            DataSaver.TryLoadDataFromFile(relativePath);

            LoadedModInfo loadedModInfo = null;
            foreach (LoadedModInfo existingModInfo in _loadedMods)
            {
                if (existingModInfo.OwnerModInfo.UniqueID == modInfo.UniqueID)
                {
                    existingModInfo.ModReference = loadedMod;
                    loadedModInfo = existingModInfo;
                }
            }

            if (loadedModInfo == null)
            {
                loadedModInfo = new LoadedModInfo(loadedMod, modInfo);
                _loadedMods.Add(loadedModInfo);
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

        private bool hasModAlreadyBeenLoaded(ModInfo modInfo)
        {
            foreach (LoadedModInfo loadedMod in _loadedMods)
            {
                if (loadedMod.OwnerModInfo.UniqueID == modInfo.UniqueID)
                    return true;
            }

            return false;
        }

        private void addModDependenciesRecursive(ModInfo modInfo, ref List<ModInfo> modsToLoad, ref Dictionary<string, ModInfo> modIdToInfo, ref List<ModInfo> sortedList)
        {
            if (sortedList.Contains(modInfo)) return;

            if (modInfo.ModDependencies != null && modInfo.ModDependencies.Length != 0)
                foreach (string dependencyId in modInfo.ModDependencies)
                {
                    ModInfo dependencyInfo = modIdToInfo[dependencyId];
                    addModDependenciesRecursive(dependencyInfo, ref modsToLoad, ref modIdToInfo, ref sortedList);
                }

            sortedList.Add(modInfo);
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

        private static IEnumerator callOnModRefreshedNextFrame(LoadedModInfo mod)
        {
            yield return 0;

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

        internal List<Mod> _allLoadedActiveMods = new List<Mod>();

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

        /// <summary>
        /// Gets all the currently loaded mods that are not disabled
        /// </summary>
        /// <returns></returns>
        public List<Mod> GetAllLoadedActiveMods()
        {
            return _allLoadedActiveMods;
        }

        /// <summary>
        /// Returns all the mods that are active Infos
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

        internal List<LoadedModInfo> GetAllMods()
        {
            return _loadedMods;
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

        public void GetModImage(ModInfo modInfo, Action<Texture2D> callback)
        {
            string uniqueId = modInfo.UniqueID;
            if (_cachedModImages.ContainsKey(uniqueId))
            {
                if (callback != null) callback(_cachedModImages[uniqueId]);
                return;
            }

            if (_processingModImages.Contains(modInfo.UniqueID))
            {
                StaticCoroutineRunner.StartStaticCoroutine(waitThenGetImageCoroutine(modInfo, callback));
                return;
            }
            StaticCoroutineRunner.StartStaticCoroutine(getImageCoroutine(modInfo, callback));
        }

        private IEnumerator getImageCoroutine(ModInfo modInfo, Action<Texture2D> callback)
        {
            _processingModImages.Add(modInfo.UniqueID);
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture($"file://{Path.Combine(modInfo.FolderPath, modInfo.ImageFileName)}"))
            {
                yield return webRequest.SendWebRequest();

                _processingModImages.Remove(modInfo.UniqueID);

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    if (callback != null) callback(null);
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                    _cachedModImages.Add(modInfo.UniqueID, texture);
                    if (callback != null) callback(texture);
                }
            }
            yield break;
        }

        private IEnumerator waitThenGetImageCoroutine(ModInfo modInfo, Action<Texture2D> callback)
        {
            while (_processingModImages.Contains(modInfo.UniqueID))
                yield return null;

            if (_cachedModImages.ContainsKey(modInfo.UniqueID))
            {
                if (callback != null) callback(_cachedModImages[modInfo.UniqueID]);
            }
            else
            {
                if (callback != null) callback(null);
            }
            yield break;
        }
    }
}
