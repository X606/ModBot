using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using ModLibrary;

namespace InternalModBot
{
    /// <summary>
    /// Gets and caches images of installed mods
    /// </summary>
    public class ModImagesManager : Singleton<ModImagesManager>
    {
        private static readonly Dictionary<string, Texture2D> _cachedModImages = new Dictionary<string, Texture2D>();
        private static readonly List<string> _processingModImages = new List<string>();

        /// <summary>
        /// Gets image of specified mod
        /// </summary>
        /// <param name="modInfo"></param>
        /// <param name="callback"></param>
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