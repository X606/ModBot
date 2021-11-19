using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InternalModBot;
using System;

namespace ModLibrary
{
#if MODDED_LEVEL_OBJECTS
    /// <summary>
    /// Can be used to add your own custom things to the editor!
    /// </summary>
    public static class LevelEditorObjectAdder
    {
        const string LEVEL_OBJECT_PREFIX = "Prefabs/LevelObjects/";
        const string TEXTURE_PREFIX = "Images/modded/";

        static readonly Dictionary<string, Transform> _moddedLevelObjectTransforms = new Dictionary<string, Transform>();
        static readonly Dictionary<string, Texture2D> _moddedLevelObjectTextures = new Dictionary<string, Texture2D>();

        static readonly List<Tuple<string, string>> _moddedLevelObjects = new List<Tuple<string, string>>();

        /// <summary>
        /// Adds an object to the editor (temporarily disabled)
        /// </summary>
        /// <param name="folderName">The name of the folder you want to add item to, if the folder doesnt already exist a new one will be created.</param>
        /// <param name="itemName">The name of the new item in the assets list.</param>
        /// <param name="transform">The prefab you want to add. PLEASE always make sure this is NOT a object in the scene but a real prefab.</param>
        /// <param name="texture">The texture to display in the assets list.</param>
        public static void AddObject(string folderName, string itemName, Transform transform, Texture2D texture)
        {
            DelegateScheduler.Instance.Schedule(delegate
            {
                debug.Log("Adding custom level editor objects is temporarily disabled", Color.yellow);
            }, 1);
            // This will be re-added in mod-bot 2.1

            /*
            string fullPath = LEVEL_OBJECT_PREFIX + folderName + "/" + itemName;
            string texturePath = TEXTURE_PREFIX + folderName + "/" + itemName;
            
            if (!_moddedLevelObjectTextures.ContainsKey(texturePath))
                _moddedLevelObjectTextures.Add(texturePath, texture);

            if (!_moddedLevelObjectTransforms.ContainsKey(fullPath))
                _moddedLevelObjectTransforms.Add(fullPath, transform);

            Tuple<string, string> tuple = new Tuple<string, string>(fullPath, texturePath);

            if (!_moddedLevelObjects.Contains(tuple))
                _moddedLevelObjects.Add(tuple);
            */
        }

        internal static UnityEngine.Object GetObjectData(string path)
        {
            if (_moddedLevelObjectTransforms.ContainsKey(path))
                return _moddedLevelObjectTransforms[path];

            if (_moddedLevelObjectTextures.ContainsKey(path))
                return _moddedLevelObjectTextures[path];

            return null;
        }

        internal static void OnLevelEditorStarted()
        {
            // Temporarily disabled until mod-bot 2.1
            /*
            List<LevelObjectEntry> levelObjects = LevelObjectsLibraryManager.Instance.GetLevelObjectsInLibrary();
            List<LevelObjectEntry> visibleLevelObjects = LevelObjectsLibraryManager.Instance.GetVisibleLevelEditorObjects();

            foreach(Tuple<string, string> tuple in _moddedLevelObjects)
            {
                LevelObjectEntry entry = new LevelObjectEntry();
                string[] subStrings = tuple.Item1.Split("/".ToArray());
                entry.DisplayName = subStrings[subStrings.Length-1];
                entry.PathUnderResources = tuple.Item1;
                entry.PreviewPathUnderResources = tuple.Item2;

                levelObjects.Add(entry);
                visibleLevelObjects.Add(entry);
            }
            
            GameUIRoot.Instance.LevelEditorUI.LibraryUI.Populate();
            */
        }
    }
#endif
}
