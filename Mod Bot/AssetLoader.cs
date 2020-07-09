using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using InternalModBot;
using System.Diagnostics;

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
            return InternalUtils.GetSubdomain(Application.dataPath) + MODS_FOLDER_NAME;
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
			StackFrame frame = new StackFrame(1);
			System.Reflection.MethodBase method = frame.GetMethod();
			Type type = method.DeclaringType;
			
			string path = InternalUtils.GetModFolderRootFromAssemblyPath(type.Assembly.Location);

			return getObjectFromFileInternal<GameObject>(assetBundleName, objectName, path);
        }

        /// <summary>
        /// Gets a <see cref="GameObject"/> from an asset bundle
        /// </summary>
        /// <param name="assetBundleName">The name of the asset bundle file</param>
        /// <param name="objectName">The name of the object you want to get from the asset bundle</param>
        /// <param name="customPath">The custom path of the asset bundle, starts from your mods root folder</param>
        /// <returns></returns>
        public static GameObject GetObjectFromFile(string assetBundleName, string objectName, string customPath)
        {
			StackFrame frame = new StackFrame(1);
			System.Reflection.MethodBase method = frame.GetMethod();
			Type type = method.DeclaringType;

			string path = InternalUtils.GetModFolderRootFromAssemblyPath(type.Assembly.Location);

			return getObjectFromFileInternal<GameObject>(assetBundleName, objectName, path + customPath);
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
			StackFrame frame = new StackFrame(1);
			System.Reflection.MethodBase method = frame.GetMethod();
			Type type = method.DeclaringType;

			string path = InternalUtils.GetModFolderRootFromAssemblyPath(type.Assembly.Location);

			return getObjectFromFileInternal<T>(assetBundleName, objectName, path);
        }

        /// <summary>
        /// Gets an Object of type <typeparamref name="T"/> from an assetbundle
        /// </summary>
        /// <typeparam name="T">The type of the object in the assetbundle</typeparam>
        /// <param name="assetBundleName">The name of the assetbundle file</param>
        /// <param name="objectName">The name of the object you want to get from the assetbundle</param>
        /// <param name="customPath">The custom path where the assetbundle is located, from your mods root folder
        /// <returns></returns>
        public static T GetObjectFromFile<T>(string assetBundleName, string objectName, string customPath) where T : UnityEngine.Object
        {
			StackFrame frame = new StackFrame(1);
			System.Reflection.MethodBase method = frame.GetMethod();
			Type type = method.DeclaringType;

			string path = InternalUtils.GetModFolderRootFromAssemblyPath(type.Assembly.Location);

			return getObjectFromFileInternal<T>(assetBundleName, objectName, path + customPath);
        }
		
        /// <summary>
        /// Clears the cache for loaded assets
        /// </summary>
        public static void ClearCache()
        {
            AssetBundleInfo.ClearCache();
        }
    }
}
