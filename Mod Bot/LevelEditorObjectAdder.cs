using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModLibrary
{
    /// <summary>
    /// Can be used to add your own custom things to the editor!
    /// </summary>
    public static class LevelEditorObjectAdder
    {
        public const string LEVEL_OBJECT_PREFIX = "Prefabs/LevelObjects/";
        public const string TEXTURE_PREFIX = "Images/modded/";

        static Dictionary<string, Transform> _moddedLevelObjectTransforms = new Dictionary<string, Transform>();
        static Dictionary<string, Texture2D> _moddedLevelObjectTextures = new Dictionary<string, Texture2D>();

        static List<Tuple<string, string>> _moddedLevelObjects = new List<Tuple<string, string>>();

        /// <summary>
        /// Adds an object to the editor
        /// </summary>
        /// <param name="folderName">The name of the folder you want to add item to, if the folder doesnt already exist a new one will be created.</param>
        /// <param name="itemName">The name of the new item in the assets list.</param>
        /// <param name="transform">The prefab you want to add. It <b>must</b> be a prefab.</param>
        /// <param name="image">The texture to display in the assets list.</param>
        public static void AddObject(string folderName, string itemName, Transform transform, Texture2D image)
        {
            if (!IsPrefab(transform))
            {
                throw new ArgumentException("The transform you want to add is not a prefab, item name: " + itemName);
            }

            string fullPath = LEVEL_OBJECT_PREFIX + folderName + "/" + itemName;
            string texturePath = TEXTURE_PREFIX + folderName + "/" + itemName;

            if (!_moddedLevelObjectTextures.ContainsKey(texturePath))
                _moddedLevelObjectTextures.Add(texturePath, image);

            if (!_moddedLevelObjectTransforms.ContainsKey(fullPath))
                _moddedLevelObjectTransforms.Add(fullPath, transform);

            Tuple<string, string> tuple = new Tuple<string, string>(fullPath, texturePath);

            if (!_moddedLevelObjects.Contains(tuple))
                _moddedLevelObjects.Add(tuple);
        }

        internal static UnityEngine.Object GetObjectData(string path)
        {
            UnityEngine.Object obj = GetGameObject(path);
            if (obj == null)
            {
                obj = GetPreviewTexture(path);
            }
            return obj;
        }

        internal static void OnLevelEditorStarted()
        {
            List<LevelObjectEntry> levelObjects = LevelObjectsLibraryManager.Instance.GetLevelObjectsInLibrary();
            List<LevelObjectEntry> visibleLevelObjects = LevelObjectsLibraryManager.Instance.GetVisibleLevelEditorObjects();

            foreach (Tuple<string, string> tuple in _moddedLevelObjects)
            {
                LevelObjectEntry entry = new LevelObjectEntry();
                string[] subStrings = tuple.Item1.Split("/".ToArray());
                entry.DisplayName = subStrings[subStrings.Length - 1];
                entry.PathUnderResources = tuple.Item1;
                entry.PreviewPathUnderResources = tuple.Item2;

                levelObjects.Add(entry);
                visibleLevelObjects.Add(entry);
            }

            GameUIRoot.Instance.LevelEditorUI.LibraryUI.Populate();
        }

        /// <summary>
        /// Get the preview image
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Texture2D GetPreviewTexture(string path)
        {
            if (HasLoadedTexture(path))
            {
                return _moddedLevelObjectTextures[path];
            }
            return null;
        }

        /// <summary>
        /// Get the preview image
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Returns GameObject with its components</returns>
        public static Transform GetGameObject(string path)
        {
            Transform transform = null;
            if (HasLoadedTransform(path))
            {
                transform = _moddedLevelObjectTransforms[path];
            }
            return transform;
        }

        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Returns <b>true</b> if object Transform was found, otherwise <b>false</b></returns>
        public static bool HasLoadedTransform(string path)
        {
            return _moddedLevelObjectTransforms.ContainsKey(path);
        }

        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Returns <b>true</b> if Texture2D was found, otherwise <b>false</b></returns>
        public static bool HasLoadedTexture(string path)
        {
            return _moddedLevelObjectTextures.ContainsKey(path);
        }

        /// <summary>
        /// Probably not the best solution for chekcing if the transform is a prefab
        /// </summary>
        /// <param name="transform"></param>
        /// <returns>Returns <b>true</b> if Transform is not in any visible scenes, otherwise <b>false</b></returns>
        private static bool IsPrefab(Transform transform)
        {
            string sceneName = transform.gameObject.scene.name;
            return sceneName != "Gameplay" && sceneName != "SplashScreen" && sceneName != "DontDestroyOnLoad";
        }
    }
}
