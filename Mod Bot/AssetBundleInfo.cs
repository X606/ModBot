using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModLibrary
{
    /// <summary>
    /// Contains information about an asset bundle with all the assets asynchronously cached
    /// </summary>
    public class AssetBundleInfo
    {
        static Dictionary<string, AssetBundleInfo> _cachedAssetBundleInfos = new Dictionary<string, AssetBundleInfo>();

        AssetBundle _assetBundle;
        Dictionary<string, UnityEngine.Object> _cachedObjects;

        /// <summary>
        /// Unloads the AssetBundle when this object is destroyed
        /// </summary>
        ~AssetBundleInfo()
        {
            _assetBundle.Unload(false);
        }

        private AssetBundleInfo(string path)
        {
            _cachedObjects = new Dictionary<string, UnityEngine.Object>();
            _assetBundle = AssetBundle.LoadFromFile(path);

            StaticCoroutineRunner.StartStaticCoroutine(cacheAllAssets());
        }

        internal static AssetBundleInfo CreateOrGetCached(string key)
        {
            if (_cachedAssetBundleInfos.ContainsKey(key))
                return _cachedAssetBundleInfos[key];

            string filePath = AssetLoader.GetSubdomain(Application.dataPath) + key;

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Could not find asset bundle file", filePath);

            AssetBundleInfo assetBundleInfo = new AssetBundleInfo(filePath);
            _cachedAssetBundleInfos.Add(key, assetBundleInfo);

            return assetBundleInfo;
        }

        IEnumerator cacheAllAssets()
        {
            string[] assets = _assetBundle.GetAllAssetNames();
            foreach (string assetName in assets)
            {
                AssetBundleRequest request = _assetBundle.LoadAssetAsync(assetName);
                yield return request;

                if (!_cachedObjects.ContainsKey(assetName))
                    _cachedObjects.Add(assetName, request.asset);
            }
        }

        /// <summary>
        /// Loads an object from the <see cref="AssetBundle"/>. Note: This will most likely be a prefab, if you want to instantiate the object you might want to use <see cref="InstantiateObject{T}(string)"/> instead
        /// </summary>
        /// <typeparam name="T">The type of object to load</typeparam>
        /// <param name="objectName">The name of the object to load</param>
        /// <returns>The object loaded from the asset bundle</returns>
        public T GetObject<T>(string objectName) where T : UnityEngine.Object
        {
            T result;

            if (_cachedObjects.TryGetValue(objectName, out UnityEngine.Object obj))
            {
                result = obj as T;
            }
            else
            {
                result = _assetBundle.LoadAsset<T>(objectName);

                if (result != null)
                    _cachedObjects.Add(objectName, result);
            }

            if (result == null)
                throw new Exception("Object \"" + objectName + "\" of type \"" + typeof(T) + "\" was not found in asset bundle \"" + _assetBundle.name + "\"");

            return result;
        }

        /// <summary>
        /// Loads a <see cref="GameObject"/> from the <see cref="AssetBundle"/>. Note: This will most likely be a prefab, if you want to instantiate the <see cref="GameObject"/> you might want to use <see cref="InstantiateObject(string)"/> instead
        /// </summary>
        /// <param name="objectName">The name of the <see cref="GameObject"/> to load</param>
        /// <returns>The <see cref="GameObject"/> loaded from the <see cref="AssetBundle"/></returns>
        public GameObject GetObject(string objectName)
        {
            return GetObject<GameObject>(objectName);
        }

        /// <summary>
        /// Instantiates an object of type <typeparamref name="T"/> from the <see cref="AssetBundle"/>
        /// </summary>
        /// <typeparam name="T">The type of the object to instantiate</typeparam>
        /// <param name="objectName">The name of the object to instantiate</param>
        /// <returns>The instantiated object</returns>
        public T InstantiateObject<T>(string objectName) where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(GetObject<T>(objectName));
        }

        /// <summary>
        /// Instantiates a <see cref="GameObject"/> from the <see cref="AssetBundle"/>
        /// </summary>
        /// <param name="objectName">The name of the <see cref="GameObject"/> to instantiate</param>
        /// <returns>The instantiated <see cref="GameObject"/></returns>
        public GameObject InstantiateObject(string objectName)
        {
            return InstantiateObject<GameObject>(objectName);
        }
    }
}
