using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip;

namespace InternalModBot
{
    internal static class ModDownloadManager
    {
        // A list of mod infos with download precentage
        private static ModDownloadInfo _currentlyDownloadingMod;

        /// <summary>
        /// Get mod download information
        /// </summary>
        /// <param name="modInfo"></param>
        /// <returns></returns>
        internal static ModDownloadInfo GetDownloadingModInfo()
        {
            return _currentlyDownloadingMod;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static bool IsDownloadingAMod()
        {
            return !_currentlyDownloadingMod.IsNull && _currentlyDownloadingMod.ModInformation != null;
        }

        /// <summary>
        /// Download a mod using its info
        /// </summary>
        /// <param name="modInfo"></param>
        /// <param name="onComplete"></param>
        internal static void DownloadMod(ModInfo modInfo, Action onComplete)
        {
            StaticCoroutineRunner.StartStaticCoroutine(downloadModFile(modInfo, onComplete));
        }

        /// <summary>
        /// Cancel mod download
        /// </summary>
        public static void EndDownload(ModDownloadCancelReason reason)
        {
            if(reason == ModDownloadCancelReason.AlreadyDownloading)
            {
                return;
            }
            _currentlyDownloadingMod.IsDone = true;
            _currentlyDownloadingMod.ModInformation = null;
            _currentlyDownloadingMod.DownloadProgress = 0;
            _currentlyDownloadingMod.IsNull = true;
            StaticCoroutineRunner.StopStaticCoroutine(downloadModFile(null, null));
        }


        static IEnumerator downloadModFile(ModInfo modInfo, Action onComplete)
        {
            if (!_currentlyDownloadingMod.IsNull)
            {
                EndDownload(ModDownloadCancelReason.AlreadyDownloading);
            }
            _currentlyDownloadingMod.ModInformation = modInfo;
            _currentlyDownloadingMod.IsDone = false;
            _currentlyDownloadingMod.IsNull = false;
            _currentlyDownloadingMod.DownloadProgress = 0;

            // If mod is already loaded, just cancel the download instead of throwing an exception
            if (ModsManager.Instance.GetLoadedModWithID(modInfo.UniqueID) != null)
            {
                onComplete?.Invoke();
                EndDownload(ModDownloadCancelReason.Error);
                yield break;
            }

            string folderName = modInfo.DisplayName;
            foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
            {
                folderName = folderName.Replace(invalidCharacter, '_');
            }

            string targetDirectory = ModsManager.Instance.ModFolderPath + folderName;
            if (Directory.Exists(targetDirectory))
            {
                onComplete?.Invoke();
                EndDownload(ModDownloadCancelReason.Error);
                yield break;
            }

            byte[] bytes;
            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://modbot.org/api?operation=downloadMod&id=" + modInfo.UniqueID))
            {
                UnityWebRequestAsyncOperation request = webRequest.SendWebRequest();

                while (!request.isDone)
                {
                    if (webRequest.isHttpError || webRequest.isNetworkError)
                    {
                        debug.Log($"Network error while downloading mod {modInfo.DisplayName} ({modInfo.UniqueID}): {webRequest.error}");

                        EndDownload(ModDownloadCancelReason.Error);

                        onComplete?.Invoke();
                        yield break;
                    }

                    _currentlyDownloadingMod.DownloadProgress = request.progress;
                    yield return 0;
                }

                bytes = webRequest.downloadHandler.data;
            }

            string tempFilePath = Path.GetTempFileName();

            yield return new WaitForTask(Task.Run(async () =>
            {
                using (FileStream tempFile = new FileStream(tempFilePath, FileMode.Create))
                {
                    tempFile.Seek(0, SeekOrigin.Begin);
                    await tempFile.WriteAsync(bytes, 0, bytes.Length);
                }
            }));

            Directory.CreateDirectory(targetDirectory);
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(tempFilePath, targetDirectory, null);

            ModsManager.Instance.ReloadMods();

            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }

            onComplete?.Invoke();

            EndDownload(ModDownloadCancelReason.SuccessDownload);
        }

        internal struct ModDownloadInfo
        {
            public ModInfo ModInformation;
            public float DownloadProgress;
            public bool IsNull;

            public bool IsDone;
        }

        public enum ModDownloadCancelReason
        {
            Unknown,
            Disconnect,
            Manual,
            Error,
            SuccessDownload,
            AlreadyDownloading
        }
    }
}
