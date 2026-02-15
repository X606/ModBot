using ModLibrary;
using ModLibrary.YieldInstructions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace InternalModBot
{
    /// <summary>
    /// Downloads mod infos and their files from the server
    /// </summary>
    internal static class ModsDownloadManager
    {
        public const int LOAD_MODINFOS_TIMEOUT = 9;

        public const bool DEBUG_PLACEHOLDER_MOD_INFOS = false;

        private static ModDownloadInfo _currentlyDownloadingMod;

        /// <summary>
        /// Get mod download information
        /// </summary>
        /// <returns></returns>
        internal static ModDownloadInfo GetDownloadingModInfo()
        {
            return _currentlyDownloadingMod;
        }

        /// <summary>
        /// Check if there's a mod downloading right now
        /// </summary>
        /// <returns></returns>
        internal static bool IsDownloadingAMod()
        {
            return _currentlyDownloadingMod != null && _currentlyDownloadingMod.Info != null;
        }

        /// <summary>
        /// Download the mod from the server
        /// </summary>
        /// <param name="info"></param>
        /// <param name="update"></param>
        /// <param name="callback"></param>
        internal static void DownloadMod(ModGeneralInfo info, bool update, Action<DownloadModResult> callback)
        {
            StaticCoroutineRunner.StartStaticCoroutine(downloadModCoroutine(info, update, callback));
        }

        private static IEnumerator downloadModCoroutine(ModGeneralInfo info, bool update, Action<DownloadModResult> callback)
        {
            _currentlyDownloadingMod = new ModDownloadInfo
            {
                Info = info,
                DownloadProgress = 0f
            };

            // If mod is already loaded, just cancel the download instead of throwing an exception
            if (!update && ModsManager.Instance.GetLoadedModWithID(info.UniqueID) != null)
            {
                if (callback != null) callback(new DownloadModResult() { Info = info });
                endDownload();
                yield break;
            }

            string folderName = update ? $"{info.DisplayName}_Ver{info.Version}" : info.DisplayName;
            foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
            {
                folderName = folderName.Replace(invalidCharacter, '_');
            }

            string targetDirectory = Path.Combine(ModsManager.Instance.ModFolderPath, folderName);
            if (Directory.Exists(targetDirectory))
            {
                if (callback != null) callback(new DownloadModResult() { Info = info });
                endDownload();
                yield break;
            }

            byte[] bytes;
            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://modbot.org/api?operation=downloadMod&id=" + info.UniqueID))
            {
                UnityWebRequestAsyncOperation request = webRequest.SendWebRequest();

                while (!request.isDone)
                {
                    _currentlyDownloadingMod.DownloadProgress = request.progress;
                    yield return null;
                }

                if (webRequest.isHttpError || webRequest.isNetworkError)
                {
                    endDownload();

                    if (callback != null) callback(new DownloadModResult()
                    {
                        Info = info,
                        Error = webRequest.error
                    });
                    yield break;
                }

                bytes = webRequest.downloadHandler.data;
            }

            bool successful = false;
            string zipFile = $"{targetDirectory}.zip";
            yield return new WaitForTaskCompletion(Task.Run(async () =>
            {
                try
                {
                    using (FileStream tempFile = new FileStream(zipFile, FileMode.Create))
                    {
                        tempFile.Seek(0, SeekOrigin.Begin);
                        await tempFile.WriteAsync(bytes, 0, bytes.Length);
                    }
                    successful = true;
                }
                catch (Exception exc)
                {
                    if (callback != null) callback(new DownloadModResult() { Info = info, Error = exc.ToString() });
                }
            }));

            endDownload();
            if (successful)
            {
                if (!update) ModsManager.Instance.LoadNewMods(new List<string>() { zipFile });
                if (callback != null) callback(new DownloadModResult() { Info = info });
            }
        }

        private static void endDownload()
        {
            _currentlyDownloadingMod.Info = null;
            _currentlyDownloadingMod.DownloadProgress = 0;
            _currentlyDownloadingMod = null;
        }

        /// <summary>
        /// Download all mod infos from the server
        /// </summary>
        /// <param name="callback"></param>
        public static void GetModInfos(Action<GetModInfosResult> callback)
        {
            StaticCoroutineRunner.StartStaticCoroutine(getModInfosCoroutine(callback));
        }

        private static IEnumerator getModInfosCoroutine(Action<GetModInfosResult> callback)
        {
            if (DEBUG_PLACEHOLDER_MOD_INFOS)
            {
                float timeout = Time.unscaledTime + 2f;
                while (Time.unscaledTime < timeout)
                    yield return null;

                if (callback != null) callback(new GetModInfosResult()
                {
                    Holder = new ModsHolder()
                    {
                        Mods = new ModInfo[]
                        {
                            new ModInfo()
                            {
                                DisplayName = "cool mod",
                                UniqueID = "1234567890-1234567890",
                                Description = "descriptive description",
                                Version = 1111,
                                Author = "a cool person"
                            },

                            new ModInfo()
                            {
                                DisplayName = "cool mod 2",
                                UniqueID = "1234567890-123456789012121",
                                Description = "another cool mod",
                                Version = 22,
                                Author = "a cool person"
                            },

                            new ModInfo()
                            {
                                DisplayName = "level editor plus update real",
                                UniqueID = "vQYm07Oxu0VnydSFsjsl",
                                Description = "b",
                                Version = 10,
                                Author = "Gorakh"
                            }
                        }
                    }
                });

                yield break;
            }

            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://modbot.org/api?operation=getAllModInfos"))
            {
                webRequest.timeout = LOAD_MODINFOS_TIMEOUT;

                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    if (callback != null) callback(new GetModInfosResult()
                    {
                        Error = webRequest.error + "\n(" + "Network: " + webRequest.isNetworkError + " HTTP: " + webRequest.isHttpError + ")"
                    });
                    yield break;
                }

                if (callback != null) callback(new GetModInfosResult()
                {
                    Holder = JsonConvert.DeserializeObject<ModsHolder>(webRequest.downloadHandler.text)
                });
            }
        }

        /// <summary>
        /// Find updates for mods from the list
        /// </summary>
        /// <param name="installedMods"></param>
        /// <param name="callback"></param>
        internal static void CheckForUpdates(List<LoadedModInfo> installedMods, Action<CheckForModUpdatesResult> callback)
        {
            StaticCoroutineRunner.StartStaticCoroutine(checkForUpdatesCoroutine(installedMods, callback));
        }

        private static IEnumerator checkForUpdatesCoroutine(List<LoadedModInfo> installedMods, Action<CheckForModUpdatesResult> callback)
        {
            GetModInfosResult modInfosResult = null;
            GetModInfos(delegate (GetModInfosResult result)
            {
                modInfosResult = result;
            });

            while (modInfosResult == null) yield return null;

            if (modInfosResult.HasFailed())
            {
                if (callback != null) callback(new CheckForModUpdatesResult()
                {
                    Error = modInfosResult.Error
                });
                yield break;
            }

            List<ModPair> pairs = new List<ModPair>();
            foreach (ModInfo remoteModInfo in modInfosResult.Holder.Value.Mods)
            {
                foreach (LoadedModInfo localMod in installedMods)
                {
                    ModInfo localModInfo = localMod.OwnerModInfo;
                    if (localModInfo == null) continue;

                    if (remoteModInfo.UniqueID == localModInfo.UniqueID && remoteModInfo.Version > localModInfo.Version)
                    {
                        pairs.Add(new ModPair()
                        {
                            Old = localModInfo,
                            New = remoteModInfo
                        });
                    }
                }
            }

            if (callback != null) callback(new CheckForModUpdatesResult()
            {
                Updates = pairs
            });
        }

        /// <summary>
        /// Update mods from the list
        /// </summary>
        /// <param name="mods"></param>
        /// <param name="callback"></param>
        internal static void UpdateMods(List<ModPair> mods, Action<UpdateModsCallback> callback)
        {
            StaticCoroutineRunner.StartStaticCoroutine(updateModsCoroutine(mods, callback));
        }

        internal static IEnumerator updateModsCoroutine(List<ModPair> mods, Action<UpdateModsCallback> callback)
        {
            UpdateModsCallback updateModsCallback = new UpdateModsCallback();

            foreach (ModPair pair in mods)
            {
                ModInfo newModInfo = pair.New;
                DownloadModResult downloadModResult = null;
                updateModsCallback.ProcessingModInfo = newModInfo;
                updateModsCallback.Progress = 0f;
                updateModsCallback.Error = null;

                DownloadMod(new ModGeneralInfo()
                {
                    DisplayName = newModInfo.DisplayName,
                    UniqueID = newModInfo.UniqueID,
                    Version = newModInfo.Version,
                }, true, delegate (DownloadModResult result)
                {
                    if (result.HasFailed())
                    {
                        updateModsCallback.Progress = 1f;
                        updateModsCallback.Error = result.Error;
                        if (callback != null) callback(updateModsCallback);
                    }
                });

                while (downloadModResult == null)
                {
                    updateModsCallback.Progress = _currentlyDownloadingMod == null ? 0f : _currentlyDownloadingMod.DownloadProgress;
                    if (callback != null) callback(updateModsCallback);
                    yield return null;
                }
            }

            updateModsCallback.ProcessingModInfo = null;
            updateModsCallback.Progress = 1f;
            updateModsCallback.Error = null;
            updateModsCallback.HasUpdatedAllMods = true;
            if (callback != null) callback(updateModsCallback);

            yield break;
        }

        /// <summary>
        /// Most important information about mod
        /// </summary>
        public class ModGeneralInfo
        {
            public string DisplayName;
            public string UniqueID;
            public uint Version;
        }

        /// <summary>
        /// Mod download information
        /// </summary>
        public class ModDownloadInfo
        {
            public ModGeneralInfo Info;
            public float DownloadProgress;
        }

        /// <summary>
        /// A pair of 2 mod infos
        /// </summary>
        public class ModPair
        {
            public ModInfo Old, New;
        }

        /// <summary>
        /// Used by <see cref="GetModInfos(Action{GetModInfosResult})"/>
        /// </summary>
        public class GetModInfosResult
        {
            public ModsHolder? Holder;
            public string Error;

            public bool HasFailed() => !string.IsNullOrEmpty(Error);
        }

        /// <summary>
        /// Used by <see cref="DownloadMod(ModGeneralInfo, bool, Action{DownloadModResult})"/>
        /// </summary>
        public class DownloadModResult
        {
            public ModGeneralInfo Info;
            public string Error;

            public bool HasFailed() => !string.IsNullOrEmpty(Error);
        }

        /// <summary>
        /// Used by <see cref="CheckForUpdates(List{LoadedModInfo}, Action{CheckForModUpdatesResult})"/>
        /// </summary>
        public class CheckForModUpdatesResult
        {
            public List<ModPair> Updates;
            public string Error;

            public bool HasFailed() => !string.IsNullOrEmpty(Error);
        }

        /// <summary>
        /// Used by <see cref="UpdateMods(List{ModPair}, Action{UpdateModsCallback})"/>
        /// </summary>
        public class UpdateModsCallback
        {
            public ModInfo ProcessingModInfo;
            public float Progress;
            public string Error;

            public bool HasUpdatedAllMods;

            public bool HasModDownloadFailed() => !string.IsNullOrEmpty(Error);
        }
    }
}