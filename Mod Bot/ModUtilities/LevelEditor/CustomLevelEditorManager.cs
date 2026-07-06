using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ModLibrary.LevelEditor
{
    /// <summary>
    /// Adds custom objects to level editor
    /// </summary>
    public static class CustomLevelEditorManager
    {
        /// <summary>
        /// Path to custom objects folder under resources folder
        /// </summary>
        public const string CUSTOM_OBJECTS_FOLDER_PATH = "Prefabs/LevelObjects/Mods";

        private static readonly Dictionary<string, Texture2D> s_textures = new Dictionary<string, Texture2D>();
        private static readonly Dictionary<string, Transform> s_objectTransforms = new Dictionary<string, Transform>();

        private static readonly Dictionary<string, string> s_pathOverrides = new Dictionary<string, string>();

        private static List<LevelObjectEntry> s_customObjects = new List<LevelObjectEntry>();

        private static Transform s_objectContainer;

        private static Transform s_missingObjectPrefab;

        private static bool s_hasInitialized;

        /// <summary>
        /// Sets up all custom level editor things
        /// </summary>
        internal static void Initialize()
        {
            if (s_hasInitialized) return;

            AddObjectAndTexture(new LevelObjectPath(null, "TempObject"), new GameObject("CustomLevelEditorObject").transform, null, null); // todo: replace it with something better

            s_hasInitialized = true;
        }

        /// <summary>
        /// Adds object to level editor registry
        /// </summary>
        /// <param name="objectPath">Path the the object</param>
        /// <param name="prefab">An object to instantiate. Can be an actual prefab or an already instantiated object</param>
        /// <param name="textureFilePath">Path to preview image on disk</param>
        /// <param name="components">Components to add</param>
        public static void AddObject(LevelObjectPath objectPath, Transform prefab, string textureFilePath = null, Type[] components = null)
        {
            addObject(objectPath, prefab, components, out string textureLoadPath);
            if (!string.IsNullOrEmpty(textureFilePath)) LoadTexture(textureFilePath, textureLoadPath);
        }

        /// <summary>
        /// Adds object to level editor registry
        /// </summary>
        /// <param name="objectPath">Path the the object</param>
        /// <param name="prefab">An object to instantiate. Can be an actual prefab or an already instantiated object</param>
        /// <param name="texture">Preview image</param>
        /// <param name="components">Components to add</param>
        public static void AddObjectAndTexture(LevelObjectPath objectPath, Transform prefab, Texture2D texture = null, Type[] components = null)
        {
            addObject(objectPath, prefab, components, out string textureLoadPath);
            if (texture) addTextureToList(texture, textureLoadPath);
        }

        private static void addObject(LevelObjectPath objectPath, Transform transform, Type[] componentTypes, out string previewTextureResourcePath)
        {
            string objectResourcePath = GetFullPath(false, objectPath);
            previewTextureResourcePath = GetFullPath(true, objectPath);

            ensureContainerIsPresent();

            if (transform.gameObject.scene.name != null) transform.SetParent(s_objectContainer); // reparent if the "prefab" is not an actual prefab

            transform.name = objectPath.ObjectName;
            if (componentTypes != null)
                foreach (Type type in componentTypes)
                    transform.gameObject.AddComponent(type);

            s_objectTransforms.Add(objectResourcePath, transform);
            s_customObjects.Add(new LevelObjectEntry()
            {
                PathUnderResources = objectResourcePath,
                PreviewPathUnderResources = previewTextureResourcePath,
                DisplayName = objectPath.ObjectName
            });
        }

        internal static Transform GetTransform(string path)
        {
            string actualPath;
            if (HasPathOverride(path))
            {
                actualPath = GetObjectPathOverride(path);
            }
            else
            {
                actualPath = path;
            }
            return s_objectTransforms.ContainsKey(actualPath) ? s_objectTransforms[actualPath] : null;
        }

        internal static void LoadTexture(string pathToFileOnDisk, string resourcePath)
        {
            if (!s_textures.ContainsKey(resourcePath))
            {
                s_textures.Add(resourcePath, null);
                _ = StaticCoroutineRunner.StartStaticCoroutine(loadTextureCoroutine(pathToFileOnDisk, resourcePath));
            }
        }

        internal static IEnumerator loadTextureCoroutine(string path, string resourcePath)
        {
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture("file://" + path))
            {
                webRequest.timeout = 10;
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.Success && webRequest.downloadHandler is DownloadHandlerTexture download)
                {
                    addTextureToList(download.texture, resourcePath);
                }
                else
                {
                    Texture2D texture = new Texture2D(2, 2);
                    addTextureToList(texture, resourcePath);
                }
            }
            yield break;
        }

        private static void addTextureToList(Texture2D texture2D, string resourcePath)
        {
            if (s_textures.ContainsKey(resourcePath))
            {
                Texture2D oldTexture = s_textures[resourcePath];
                if (oldTexture) UnityEngine.Object.Destroy(oldTexture);

                s_textures[resourcePath] = texture2D;
                return;
            }
            s_textures.Add(resourcePath, texture2D);
        }

        internal static Texture2D GetTexture(string path) => s_textures.ContainsKey(path) ? s_textures[path] : null;

        /// <summary>
        /// Binds the old path to the new one
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        public static void AddPathOverride(LevelObjectPath oldPath, LevelObjectPath newPath)
        {
            addPathOverride(GetFullPath(false, oldPath), GetFullPath(false, newPath));
        }

        private static void addPathOverride(string oldFullPath, string newFullPath) => s_pathOverrides[oldFullPath] = newFullPath;

        internal static bool HasPathOverride(string path) => s_pathOverrides.ContainsKey(path);

        internal static string GetObjectPathOverride(string path) => getObjectPathOverrideRecursive(path);

        private static string getObjectPathOverrideRecursive(string path)
        {
            if (HasPathOverride(path))
            {
                return getObjectPathOverrideRecursive(s_pathOverrides[path]);
            }
            return path;
        }

        internal static string GetFullPath(bool isImagesFolder, LevelObjectPath objectPath)
        {
            return $"{GetRootFolder(isImagesFolder)}{objectPath}";
        }

        internal static string GetRootFolder(bool isImagesFolder)
        {
            string prefix = isImagesFolder ? "Images" : "Prefabs";
            return prefix + "/LevelObjects/Mods/";
        }

        internal static Transform GetMissingObjectTransform()
        {
            if (s_missingObjectPrefab) return s_missingObjectPrefab;

            ensureContainerIsPresent();

            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.name = "MissingObject";
            gameObject.transform.SetParent(s_objectContainer);

            gameObject.AddComponent<LevelEditorMissingObject>();
            gameObject.AddComponent<LevelEditorDisableRendererAndCollision>();

            Transform transform = gameObject.transform;
            s_missingObjectPrefab = transform;
            return transform;
        }

        internal static bool IsPathToCustomObject(string path) => path.StartsWith(CUSTOM_OBJECTS_FOLDER_PATH);

        internal static List<LevelObjectEntry> GetLevelObjectEntries() => s_customObjects;

        internal static void AddMissingEntries(List<LevelObjectEntry> entriesToAdd, List<LevelObjectEntry> allEntries)
        {
            foreach (LevelObjectEntry entry in entriesToAdd)
            {
                bool isPresent = false;

                for (int i = allEntries.Count - 1; i >= 0; i--) // check entries from the end, where custom entries are located
                {
                    LevelObjectEntry existingEntry = allEntries[i];
                    if (entry.PathUnderResources == existingEntry.PathUnderResources)
                    {
                        isPresent = true;
                        break;
                    }
                }

                if (!isPresent)
                {
                    allEntries.Add(entry);
                }
            }
        }

        private static void ensureContainerIsPresent()
        {
            if (s_objectContainer) return;

            GameObject gameObject = new GameObject("Level Editor Custom Objects Container");
            gameObject.SetActive(false);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            s_objectContainer = gameObject.transform;
        }
    }
}