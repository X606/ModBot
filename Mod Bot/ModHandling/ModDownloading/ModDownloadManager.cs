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
        public const int LOAD_MODINFOS_TIMEOUT = 9;

        private static ModDownloadInfo _currentlyDownloadingMod;

        private static UnityWebRequest _modInfoDownloadWebRequest;

        /// <summary>
        /// Get mod download information
        /// </summary>
        /// <returns></returns>
        internal static ModDownloadInfo GetDownloadingModInfo()
        {
            return _currentlyDownloadingMod;
        }

        internal static bool IsDownloadingAMod()
        {
            return _currentlyDownloadingMod != null && _currentlyDownloadingMod.Info != null;
        }

        internal static bool IsLoadingModInfos()
        {
            return _modInfoDownloadWebRequest != null;
        }

        /// <summary>
        /// Download a mod using its info
        /// </summary>
        /// <param name="info"></param>
        /// <param name="update"></param>
        /// <param name="callback"></param>
        internal static void DownloadMod(ModGeneralInfo info, bool update, Action<DownloadModResult> callback)
        {
            StaticCoroutineRunner.StartStaticCoroutine(downloadModFile(info, update, callback));
        }

        private static IEnumerator downloadModFile(ModGeneralInfo info, bool update, Action<DownloadModResult> callback)
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

            yield return new WaitForTask(Task.Run(async () =>
            {
                using (FileStream tempFile = new FileStream($"{targetDirectory}.zip", FileMode.Create))
                {
                    tempFile.Seek(0, SeekOrigin.Begin);
                    await tempFile.WriteAsync(bytes, 0, bytes.Length);
                }
            }));

            endDownload();
            if (!update) ModsManager.Instance.ReloadMods(true);
            if (callback != null) callback(new DownloadModResult() { Info = info });
        }

        private static void endDownload()
        {
            _currentlyDownloadingMod.Info = null;
            _currentlyDownloadingMod.DownloadProgress = 0;
            _currentlyDownloadingMod = null;
        }

        /// <summary>
        /// Download all mod infos from server
        /// </summary>
        /// <param name="callback"></param>
        public static void DownloadModsData(Action<GetModInfosResult> callback)
        {
            if (IsDownloadingAMod()) return;
            StaticCoroutineRunner.StartStaticCoroutine(downloadModData(callback));
        }

        private static IEnumerator downloadModData(Action<GetModInfosResult> callback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://modbot.org/api?operation=getAllModInfos"))
            {
                _modInfoDownloadWebRequest = webRequest;
                webRequest.timeout = LOAD_MODINFOS_TIMEOUT;

                yield return webRequest.SendWebRequest();

                _modInfoDownloadWebRequest = null;
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

        public class ModGeneralInfo
        {
            public string DisplayName;
            public string UniqueID;
            public uint Version;
        }

        public class ModDownloadInfo
        {
            public ModGeneralInfo Info;
            public float DownloadProgress;
        }

        public class GetModInfosResult
        {
            public ModsHolder? Holder;
            public string Error;

            public bool HasFailed() => !string.IsNullOrEmpty(Error);
        }

        public class DownloadModResult
        {
            public ModGeneralInfo Info;
            public string Error;

            public bool HasFailed() => !string.IsNullOrEmpty(Error);
        }
    }
}