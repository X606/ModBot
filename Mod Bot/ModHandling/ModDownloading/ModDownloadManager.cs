using ICSharpCode.SharpZipLib.Zip;
using ModLibrary;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace InternalModBot
{
    internal static class ModsDownloadManager
    {
        public const int LOAD_MODINFOS_TIMEOUT = 9; // Is it enough?

        // A list of mod infos with download precentage
        private static ModDownloadInfo _currentlyDownloadingMod;

        private static UnityWebRequest _modInfoDownloadWebRequest;

        /// <summary>
        /// Get mod download information
        /// </summary>
        /// <param name="modInfo"></param>
        /// <returns></returns>
        internal static ModDownloadInfo GetDownloadingModInfo()
        {
            return _currentlyDownloadingMod;
        }

        internal static bool IsDownloadingAMod()
        {
            return _currentlyDownloadingMod != null && _currentlyDownloadingMod.ModInformation != null;
        }

        internal static bool IsLoadingModInfos()
        {
            return _modInfoDownloadWebRequest != null;
        }

        internal static UnityWebRequest GetModInfosWebRequest()
        {
            return _modInfoDownloadWebRequest;
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
            if (reason == ModDownloadCancelReason.AlreadyDownloading)
            {
                return;
            }
            _currentlyDownloadingMod.IsDone = true;
            _currentlyDownloadingMod.ModInformation = null;
            _currentlyDownloadingMod.DownloadProgress = 0;
            _currentlyDownloadingMod = null;
            StaticCoroutineRunner.StopStaticCoroutine(downloadModFile(null, null));
        }

        private static IEnumerator downloadModFile(ModInfo modInfo, Action onComplete)
        {
            if (_currentlyDownloadingMod != null)
            {
                EndDownload(ModDownloadCancelReason.AlreadyDownloading);
            }
            _currentlyDownloadingMod = new ModDownloadInfo
            {
                ModInformation = modInfo,
                IsDone = false,
                DownloadProgress = 0
            };

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
                        webRequest.Dispose();
                        yield break;
                    }

                    _currentlyDownloadingMod.DownloadProgress = request.progress;
                    yield return 0;
                }

                bytes = webRequest.downloadHandler.data;
                webRequest.Dispose();
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

        /// <summary>
        /// Download all mod infos from server
        /// </summary>
        /// <param name="onFinishDownload"></param>
        /// <param name="onGotError"></param>
        public static void DownloadModsData(Action<ModsHolder?> onFinishDownload, Action<string> onGotError = null)
        {
            if (IsDownloadingAMod()) return;
            StaticCoroutineRunner.StartStaticCoroutine(downloadModData(onFinishDownload, onGotError));
        }

        private static IEnumerator downloadModData(Action<ModsHolder?> onFinishDownload, Action<string> onGotError = null)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://modbot.org/api?operation=getAllModInfos"))
            {
                _modInfoDownloadWebRequest = webRequest;
                webRequest.timeout = LOAD_MODINFOS_TIMEOUT;

                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    _modInfoDownloadWebRequest = null;
                    string error = webRequest.error + "\n(" + "Network: " + webRequest.isNetworkError + " HTTP: " + webRequest.isHttpError + ")";
                    if (onGotError != null) onGotError(error);
                    if (onGotError != null) onFinishDownload(null);
                    webRequest.Dispose();

                    yield break;
                }
                _modInfoDownloadWebRequest = null;

                ModsHolder? modsHolder = JsonConvert.DeserializeObject<ModsHolder>(webRequest.downloadHandler.text);
                if (onGotError != null) onFinishDownload(modsHolder);

                webRequest.Dispose();
            }
        }



        internal class ModDownloadInfo
        {
            public ModInfo ModInformation;
            public float DownloadProgress;

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
