using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace InternalModBot.LevelEditor
{
    /// <summary>
    /// Adds custom objects to level editor
    /// </summary>
    public static class CustomLevelEditorManager
    {
        private static readonly Dictionary<string, Texture2D> s_textures = new Dictionary<string, Texture2D>();
        private static readonly Dictionary<string, Transform> s_objectTransforms = new Dictionary<string, Transform>();

        private static readonly Dictionary<string, string> s_pathOverrides = new Dictionary<string, string>();

        private static List<LevelObjectEntry> s_customObjects = new List<LevelObjectEntry>();

        private static Transform s_objectContainer;

        private static Transform s_missingObjectPrefab;

        /// <summary>
        /// Sets up the all custom level editor things
        /// </summary>
        internal static void Initialize()
        {
            GameObject scriptableObjectPrefab = InternalAssetBundleReferences.ModBot.GetObject("ScriptableObject");
            AddObjectAndTexture(new LevelObjectPath(null, "ScriptableObject"), scriptableObjectPrefab.transform,
                InternalAssetBundleReferences.ModBot.GetObject<Texture2D>("script"),
                null);

            scriptableObjectPrefab.AddComponent<Scriptable>();
            scriptableObjectPrefab.AddComponent<LevelEditorToolRestriction>().DisallowedTools = new List<LevelEditorToolType>() { LevelEditorToolType.Rotate, LevelEditorToolType.Scale };
            scriptableObjectPrefab.AddComponent<LevelEditorComponentDescription>().Description = "Runs a bit of code when the level starts";
        }

        public static void AddObject(LevelObjectPath objectPath, Transform prefab, string textureFilePath = null, Type[] components = null)
        {
            addObject(objectPath, prefab, components, out string textureLoadPath);
            if (!string.IsNullOrEmpty(textureFilePath)) AddTexture(textureFilePath, textureLoadPath);
        }

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

            transform.SetParent(s_objectContainer);
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

        internal static bool HasTransform(string path)
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
            return s_objectTransforms.ContainsKey(actualPath);
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
            return s_objectTransforms[actualPath];
        }

        internal static void AddTexture(string path, string resourcePath)
        {
            if (!s_textures.ContainsKey(resourcePath))
            {
                s_textures.Add(resourcePath, null);
                _ = StaticCoroutineRunner.StartStaticCoroutine(addTextureCoroutine(path, resourcePath));
            }
        }

        internal static IEnumerator addTextureCoroutine(string path, string resourcePath)
        {
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture("file://" + path))
            {
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


        internal static bool HasTexture(string path) => s_textures.ContainsKey(path);

        internal static Texture2D GetTexture(string path) => s_textures[path];

        internal static UnityEngine.Object GetResourceObject(string path)
        {
            if (HasTexture(path))
            {
                return GetTexture(path);
            }
            else if (HasTransform(path))
            {
                return GetTransform(path);
            }
            return null;
        }

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
                return getObjectPathOverrideRecursive(GetObjectPathOverride(path));
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
            gameObject.AddComponent<LevelEditorDisableRendererAndCollission>();

            Transform transform = gameObject.transform;
            s_missingObjectPrefab = transform;
            return transform;
        }

        internal static bool IsPathToCustomObject(string path) => path.StartsWith("Prefabs/LevelObjects/Mods");

        internal static List<LevelObjectEntry> GetLevelObjectEntries() => s_customObjects;

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