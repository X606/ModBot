using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace ModLibrary
{
    /// <summary>
    /// Used to load assets from assetbundles (normally placed in the mods folder)
    /// </summary>
    public static class AssetLoader
    {
        private static Dictionary<string, UnityEngine.Object> cachedObjects = new Dictionary<string, UnityEngine.Object>();

        /// <summary>
        /// The name of the folder where mods are stored
        /// </summary>
        public const string ModsFolderName = "mods/";

        /// <summary>
        /// Returns the full directory to the mods folder directory where we expect most of the assetbundles to be
        /// </summary>
        /// <returns></returns>
        public static string GetModsFolderDirectory()
        {
            return GetSubdomain(Application.dataPath) + ModsFolderName;
        }

        private static T GetObjectFromFileInternal<T>(string _assetBundleName, string _objectName, string _customPathFromDataPath = ModsFolderName) where T : UnityEngine.Object
        {
            string key = _assetBundleName + ":" + _objectName;

            if (cachedObjects.ContainsKey(key))
            {
                return cachedObjects[key] as T;
            }

            string assetBundleDirectory = GetSubdomain(Application.dataPath) + _customPathFromDataPath;
            string assetBundleFilePath = assetBundleDirectory + _assetBundleName;

            if (!Directory.Exists(assetBundleDirectory))
            {
                throw new DirectoryNotFoundException("Could not find directory " + assetBundleDirectory);
            }
            if (!File.Exists(assetBundleFilePath))
            {
                throw new FileNotFoundException("Could not find AssetBundle file", _assetBundleName);
            }

            WWW www = WWW.LoadFromCacheOrDownload("file:///" + assetBundleFilePath, 1);
            AssetBundle assetBundle = www.assetBundle;
            T result = assetBundle.LoadAssetAsync<T>(_objectName).asset as T;
            www.Dispose();
            assetBundle.Unload(false);

            cachedObjects[key] = result;

            return result;
        }

        /// <summary>
        /// Gets a <see cref="GameObject"/> from an asset bundle
        /// </summary>
        /// <param name="assetBundleName">The name of the asset bundle file (Must be located in the 'mods' folder for this method)</param>
        /// <param name="objectName">The name of the object you want to get from the asset bundle</param>
        /// <returns></returns>
        public static GameObject GetObjectFromFile(string assetBundleName, string objectName)
        {
            return GetObjectFromFileInternal<GameObject>(assetBundleName, objectName);
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
            return GetObjectFromFileInternal<GameObject>(assetBundleName, objectName, customPath);
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
            return GetObjectFromFileInternal<T>(assetBundleName, objectName);
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
            return GetObjectFromFileInternal<T>(assetBundleName, objectName, customPath);
        }

        /// <summary>Tries to save the file from the specified directory, (will not save file if one with the same already exists)</summary>
        /// <param name="url">The URL to download the file from.</param>
        /// <param name="name">The name of the file that will be created.</param>
        public static void TrySaveFileToMods(string url, string name)
        {
            string path = GetSubdomain(Application.dataPath) + "mods/" + name;

            if (File.Exists(path))
            {
                return;
            }

            SaveFileToMods(url, name);
        }

        /// <param name="url">The URL to download the file from.</param>
        /// <param name="name">The name of the file that will be created.</param>
        public static void SaveFileToMods(string url, string name)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(MyRemoteCertificateValidationCallback);
            byte[] fileData = new WebClient
            {
                Headers =
                {
                    "User-Agent: Other"
                }
            }.DownloadData(url);

            string path = GetSubdomain(Application.dataPath) + "mods/";

            FileStream file = File.Create(path + name);
            file.Close();

            File.WriteAllBytes(path + name, fileData);
        }

        private static bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
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
                        {
                            result = false;
                        }
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
            cachedObjects.Clear();
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
    }
}
