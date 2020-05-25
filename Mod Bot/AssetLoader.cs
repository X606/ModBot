using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace ModLibrary
{
    /// <summary>
    /// Used to load assets from assetbundles (normally placed in the mods folder)
    /// </summary>
    public static class AssetLoader
    {
        /// <summary>
        /// The name of the folder where mods are stored
        /// </summary>
        public const string MODS_FOLDER_NAME = "mods/";

        /// <summary>
        /// Returns the full directory to the mods folder directory where we expect most of the assetbundles to be
        /// </summary>
        /// <returns></returns>
        public static string GetModsFolderDirectory()
        {
            return GetSubdomain(Application.dataPath) + MODS_FOLDER_NAME;
        }

        static AssetBundleInfo getAssetBundle(string name, string pathFromDataPath = MODS_FOLDER_NAME)
        {
            return AssetBundleInfo.CreateOrGetCached(pathFromDataPath + name);
        }

        /// <summary>
        /// Gets the <see cref="AssetBundleInfo"/> from the asset bundle with the specfied name at the given custom path
        /// </summary>
        /// <param name="name">The name of the asset bundle file</param>
        /// <param name="customPath">The custom path to the asset bundle, relative to the game root folder</param>
        /// <returns></returns>
        internal static AssetBundleInfo GetAssetBundle(string name, string customPath)
        {
            if (!customPath.EndsWith("/") && !customPath.EndsWith("\\"))
                customPath += "/";

            return getAssetBundle(name, customPath);
        }

        static T getObjectFromFileInternal<T>(string _assetBundleName, string _objectName, string _customPathFromDataPath = MODS_FOLDER_NAME) where T : UnityEngine.Object
        {
            return getAssetBundle(_assetBundleName, _customPathFromDataPath).GetObject<T>(_objectName);
        }

        /// <summary>
        /// Gets a <see cref="GameObject"/> from an asset bundle
        /// </summary>
        /// <param name="assetBundleName">The name of the asset bundle file (Must be located in the 'mods' folder for this method)</param>
        /// <param name="objectName">The name of the object you want to get from the asset bundle</param>
        /// <returns></returns>
        public static GameObject GetObjectFromFile(string assetBundleName, string objectName)
        {
            return getObjectFromFileInternal<GameObject>(assetBundleName, objectName);
        }

        /// <summary>
        /// Gets a <see cref="GameObject"/> from an asset bundle
        /// </summary>
        /// <param name="assetBundleName">The name of the asset bundle file</param>
        /// <param name="objectName">The name of the object you want to get from the asset bundle</param>
        /// <param name="customPath">The custom path of the asset bundle, starts from the 'Clone Drone in the Danger Zone' folder</param>
        /// <returns></returns>
        public static GameObject GetObjectFromFile(string assetBundleName, string objectName, string customPath)
        {
            return getObjectFromFileInternal<GameObject>(assetBundleName, objectName, customPath);
        }

        /// <summary>
        /// Gets an Object of type <typeparamref name="T"/> from an asset bundle
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="assetBundleName">The name of the asset bundle file</param>
        /// <param name="objectName">The name of the object you want to get from the asset bundle</param>
        /// <returns></returns>
        public static T GetObjectFromFile<T>(string assetBundleName, string objectName) where T : UnityEngine.Object
        {
            return getObjectFromFileInternal<T>(assetBundleName, objectName);
        }

        /// <summary>
        /// Gets an Object of type <typeparamref name="T"/> from an assetbundle
        /// </summary>
        /// <typeparam name="T">The type of the object in the assetbundle</typeparam>
        /// <param name="assetBundleName">The name of the assetbundle file</param>
        /// <param name="objectName">The name of the object you want to get from the assetbundle</param>
        /// <param name="customPath">The custom path where the assetbundle is located (goes from <seealso cref="Application.dataPath"/>)</param>
        /// <returns></returns>
        public static T GetObjectFromFile<T>(string assetBundleName, string objectName, string customPath) where T : UnityEngine.Object
        {
            return getObjectFromFileInternal<T>(assetBundleName, objectName, customPath);
        }

        /// <summary>Tries to save the file from the specified directory, (will not save file if one with the same already exists)</summary>
        /// <param name="url">The URL to download the file from.</param>
        /// <param name="name">The name of the file that will be created.</param>
        public static void TrySaveFileToMods(string url, string name)
        {
            string path = GetSubdomain(Application.dataPath) + "mods/" + name;

            if (File.Exists(path))
                return;

            SaveFileToMods(url, name);
        }

        /// <param name="url">The URL to download the file from.</param>
        /// <param name="name">The name of the file that will be created.</param>
        public static void SaveFileToMods(string url, string name)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(myRemoteCertificateValidationCallback);
            WebClient webClient = new WebClient
            {
                Headers =
                {
                    "User-Agent: Other"
                }
            };

            byte[] fileData = webClient.DownloadData(url);
            webClient.Dispose();

            string path = GetSubdomain(Application.dataPath) + "mods/";

            FileStream file = File.Create(path + name);
            file.Close();

            File.WriteAllBytes(path + name, fileData);
        }

        /// <summary>
        /// Saves a file into the mods folder, but does so asynchronously, returns a <see cref="AsyncDownload"/> that can be awaited in a corutine with yield return
        /// </summary>
        /// <param name="url">The url to download.</param>
        /// <param name="fileName">The name you want to give the file once you have downloaded it.</param>
        /// <returns>A <see cref="AsyncDownload"/> that can be awaited in corutines with yield return</returns>
        public static AsyncDownload SaveFileToModsAsync(string url, string fileName)
        {
            AsyncDownload async = new AsyncDownload();
            StaticCoroutineRunner.StartStaticCoroutine(saveFileToModsAsyncCorutine(url, fileName, async));
            return async;
        }

        static IEnumerator saveFileToModsAsyncCorutine(string url, string fileName, AsyncDownload asyncDownload)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);

            yield return webRequest.SendWebRequest();
            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                asyncDownload.IsDone = true;
                asyncDownload.isError = true;
                yield break;
            }

            string fullPath = GetModsFolderDirectory() + fileName;
            File.WriteAllBytes(fullPath, webRequest.downloadHandler.data);
            asyncDownload.IsDone = true;
        }

        static bool myRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool result = true;
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;

                        if (!chain.Build((X509Certificate2)certificate))
                            result = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Clears the cache for loaded assets
        /// </summary>
        public static void ClearCache()
        {
            AssetBundleInfo.ClearCache();
        }

        /// <summary>
        /// Gets the directory 1 step under a spesific directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetSubdomain(string path)
        {
            string[] subDomainsArray = path.Split(new char[] { '/' });

            List<string> subDomainsList = new List<string>(subDomainsArray);
            subDomainsList.RemoveAt(subDomainsList.Count - 1);
            
            return subDomainsList.Join("/") + "/";
        }

        /// <summary>
        /// A download handeler than can be awaited in corutines with yield return
        /// </summary>
        public class AsyncDownload : CustomYieldInstruction
        {
            /// <summary>
            /// If this return true, the download is not done yet
            /// </summary>
            public override bool keepWaiting => !IsDone;
            /// <summary>
            /// If this return true, there was an error during downloading
            /// </summary>
            public bool IsError => isError;

            internal bool isError = false;
            internal bool IsDone = false;
        }
    }
}
