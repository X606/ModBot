using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace ModLibrary
{
    public static class AssetLoader
    {
        private static Dictionary<string, UnityEngine.Object> cachedObjects = new Dictionary<string, UnityEngine.Object>();

        #region Obsolete methods
        /// <summary>
        /// Gets a GameObject from a file.
        /// </summary>
        /// <param name="file">The asset file (loaded from the mods folder)</param>
        /// <param name="name">Name of the GamObject to get</param>
        /// <returns</returns>
        [Obsolete("Use GetObjectFromFile instead")]
        public static GameObject getObjectFromFile(string file, string name)
        {
            string key = file + ":" + name;
            if (cachedObjects.ContainsKey(key))
            {
                return (GameObject)cachedObjects[key];
            }

            string path = getSubdomain(Application.dataPath) + "mods/";
            if (!Directory.Exists(path))
            {
                Debug.LogError("getObjectFromFile: This should never, ever, ever happen. If it does something is terribly wrong (there is no mods directory, but you are running a mod)" + path);
                return null;
            }
            WWW www = WWW.LoadFromCacheOrDownload("file:///" + path + file, 1);
            AssetBundle assetBundle = www.assetBundle;
            GameObject result = assetBundle.LoadAssetAsync<GameObject>(name).asset as GameObject;
            www.Dispose();
            assetBundle.Unload(false);
            cachedObjects[key] = result;

            return result;
        }

        [Obsolete("Use GetObjectFromFile instead")]
        public static GameObject getObjectFromFile(string file, string name, string _path)
        {
            string key = file + ":" + name;
            if (cachedObjects.ContainsKey(key))
            {
                return (GameObject)cachedObjects[key];
            }

            string path = getSubdomain(Application.dataPath) + _path;
            if (!Directory.Exists(path))
            {
                Debug.LogError("getObjectFromFile + This should never, ever, ever happen. If it does something is terribly wrong (there is no mods directory, but you are running a mod)" + path);
                return null;
            }
            WWW www = WWW.LoadFromCacheOrDownload("file:///" + path + file, 1);
            AssetBundle assetBundle = www.assetBundle;
            GameObject result = assetBundle.LoadAssetAsync<GameObject>(name).asset as GameObject;
            www.Dispose();
            assetBundle.Unload(false);
            cachedObjects[key] = result;

            return result;
        }

        [Obsolete("Use GetObjectFromFile instead")]
        public static T getObjectFromFile<T>(string file, string name) where T : UnityEngine.Object
        {
            string key = file + ":" + name;
            if (cachedObjects.ContainsKey(key))
            {
                return (T)cachedObjects[key];
            }

            string path = getSubdomain(Application.dataPath) + "mods/";
            if (!Directory.Exists(path))
            {
                Debug.LogError("getObjectFromFile<T> + This should never, ever, ever happen. If it does something is terribly wrong (there is no mods directory, but you are running a mod)" + path);
                return null;
            }
            WWW www = WWW.LoadFromCacheOrDownload("file:///" + path + file, 1);
            AssetBundle assetBundle = www.assetBundle;
            T result = assetBundle.LoadAssetAsync<T>(name).asset as T;
            www.Dispose();
            assetBundle.Unload(false);
            cachedObjects[key] = result;

            return result;
        }

        /// <summary>
        /// If it there is already a file named the same thing as name, this wont do anything
        /// </summary>
        [Obsolete("Use TrySaveFileToMods instead")]
        public static void trySaveFileToMods(string url, string name)
        {
            string path = getSubdomain(Application.dataPath) + "mods/" + name;
            if (File.Exists(path))
            {
                return;
            }
            saveFileToMods(url, name);
        }

        [Obsolete("Use SaveFileToMods instead")]
        public static void saveFileToMods(string url, string name)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(MyRemoteCertificateValidationCallback);
            byte[] file = new WebClient
            {
                Headers =
                {
                    "User-Agent: Other"
                }
            }.DownloadData(url);
            string path = getSubdomain(Application.dataPath) + "mods/";
            var a = File.Create(path + name);
            a.Close();
            File.WriteAllBytes(path + name, file);
        }

        [Obsolete("Use GetSubdomain instead")]
        public static string getSubdomain(string path)
        {
            string[] subDomainsArray = path.Split(new char[] { '/' });

            List<string> subDomainsList = new List<string>(subDomainsArray);
            subDomainsList.RemoveAt(subDomainsList.Count - 1);

            return subDomainsList.Join("/") + "/";
        }
        #endregion

        public const string ModsFolderName = "mods/";

        /// <returns>The full directory to the mods folder</returns>
        public static string GetModsFolderDirectory()
        {
            return GetSubdomain(Application.dataPath) + ModsFolderName;
        }

        private static GameObject GetObjectFromFileInternal(string _assetBundleName, string _objectName, string _customPathFromDataPath = ModsFolderName)
        {
            string key = _assetBundleName + ":" + _objectName;

            if (cachedObjects.ContainsKey(key))
            {
                return (GameObject)cachedObjects[key];
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
            GameObject result = assetBundle.LoadAssetAsync<GameObject>(_objectName).asset as GameObject;
            www.Dispose();
            assetBundle.Unload(false);

            cachedObjects[key] = result;

            return result;
        }

        /// <summary>
        /// Gets a GameObject from an asset bundle
        /// </summary>
        /// <param name="assetBundleName">The name of the asset bundle file (Must be located in the 'mods' folder for this method)</param>
        /// <param name="objectName">The name of the object you want to get from the asset bundle</param>
        /// <returns></returns>
        public static GameObject GetObjectFromFile(string assetBundleName, string objectName)
        {
            return GetObjectFromFileInternal(assetBundleName, objectName);
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
            return GetObjectFromFileInternal(assetBundleName, objectName, customPath);

            string assetPath = assetBundleName + ":" + objectName;
            string assetBundlePath = GetSubdomain(Application.dataPath) + customPath;

            if (cachedObjects.ContainsKey(assetPath))
            {
                return (GameObject)cachedObjects[assetPath];
            }

            if (!Directory.Exists(assetBundlePath))
            {
                Debug.LogError("GetObjectFromFile: This should never, ever, ever happen. If it does something is terribly wrong (there is no mods directory, but you are running a mod)" + assetBundlePath);
                return null;
            }

            WWW www = WWW.LoadFromCacheOrDownload("file:///" + assetBundlePath + assetBundleName, 1);
            AssetBundle assetBundle = www.assetBundle;
            GameObject result = assetBundle.LoadAssetAsync<GameObject>(objectName).asset as GameObject;
            www.Dispose();
            assetBundle.Unload(false);

            cachedObjects[assetPath] = result;

            return result;
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
            return GetObjectFromFileInternal(assetBundleName, objectName) as T;

            string key = assetBundleName + ":" + objectName;
            if (cachedObjects.ContainsKey(key))
            {
                return (T)cachedObjects[key];
            }

            string path = GetSubdomain(Application.dataPath) + "mods/";
            if (!Directory.Exists(path))
            {
                Debug.LogError("GetObjectFromFile<T>: This should never, ever, ever happen. If it does something is terribly wrong (there is no mods directory, but you are running a mod)" + path);
                return null;
            }
            WWW www = WWW.LoadFromCacheOrDownload("file:///" + path + assetBundleName, 1);
            AssetBundle assetBundle = www.assetBundle;
            T result = assetBundle.LoadAssetAsync<T>(objectName).asset as T;
            www.Dispose();
            assetBundle.Unload(false);
            cachedObjects[key] = result;

            return result;
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
            return GetObjectFromFileInternal(assetBundleName, objectName, customPath) as T;
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

        public static void ClearCache()
        {
            cachedObjects.Clear();
        }

        public static string GetSubdomain(string path)
        {
            string[] subDomainsArray = path.Split(new char[] { '/' });

            List<string> subDomainsList = new List<string>(subDomainsArray);
            subDomainsList.RemoveAt(subDomainsList.Count - 1);
            
            return subDomainsList.Join("/") + "/";
        }
    }
}
