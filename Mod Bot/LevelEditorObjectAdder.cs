using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InternalModBot;

namespace ModLibrary
{
    /// <summary>
    /// Can be used to add your own custom things to the editor!
    /// </summary>
    public static class LevelEditorObjectAdder
    {
        const string LEVEL_OBJECT_PREFIX = "Prefabs/LevelObjects/";
        const string TEXTURE_PREFIX = "Images/modded/";

        static readonly Dictionary<string, Transform> _moddedLevelObjectTransforms = new Dictionary<string, Transform>();
        static readonly Dictionary<string, Texture2D> _moddedLevelObjectTextures = new Dictionary<string, Texture2D>();

        static readonly List<DoubleValueHolder<string, string>> _moddedLevelObjects = new List<DoubleValueHolder<string, string>>();

        /// <summary>
        /// Adds an object to the editor
        /// </summary>
        /// <param name="folderName">The name of the folder you want to add item to, if the folder doesnt already exist a new one will be created.</param>
        /// <param name="itemName">The name of the new item in the assets list.</param>
        /// <param name="transform">The prefab you want to add. PLEASE always make sure this is NOT a object in the scene but a real prefab.</param>
        /// <param name="texture">The texture to display in the assets list.</param>
        public static void AddObject(string folderName, string itemName, Transform transform, Texture2D texture)
        {
            string fullPath = LEVEL_OBJECT_PREFIX + folderName + "/" + itemName;
            string texturePath = TEXTURE_PREFIX + folderName + "/" + itemName;
            
            if (!_moddedLevelObjectTextures.ContainsKey(texturePath))
                _moddedLevelObjectTextures.Add(texturePath, texture);

            if (!_moddedLevelObjectTransforms.ContainsKey(fullPath))
                _moddedLevelObjectTransforms.Add(fullPath, transform);

            DoubleValueHolder<string, string> doubleValueHolder = new DoubleValueHolder<string, string>(fullPath, texturePath);

            if (!_moddedLevelObjects.Contains(doubleValueHolder))
                _moddedLevelObjects.Add(doubleValueHolder);

        }

        internal static Object GetObjectData(string path)
        {
            if (_moddedLevelObjectTransforms.ContainsKey(path))
                return _moddedLevelObjectTransforms[path];

            if (_moddedLevelObjectTextures.ContainsKey(path))
                return _moddedLevelObjectTextures[path];

            return null;
        }

        internal static void OnLevelEditorStarted()
        {
            List<LevelObjectEntry> levelObjects = LevelObjectsLibraryManager.Instance.GetLevelObjectsInLibrary();
            List<LevelObjectEntry> visibleLevelObjects = LevelObjectsLibraryManager.Instance.GetVisibleLevelEditorObjects();

            foreach(DoubleValueHolder<string, string> item in _moddedLevelObjects)
            {
                LevelObjectEntry entry = new LevelObjectEntry();
                string[] subStrings = item.FirstValue.Split("/".ToArray());
                entry.DisplayName = subStrings[subStrings.Length-1];
                entry.PathUnderResources = item.FirstValue;
                entry.PreviewPathUnderResources = item.SecondValue;

                levelObjects.Add(entry);
                visibleLevelObjects.Add(entry);
            }
            
            GameUIRoot.Instance.LevelEditorUI.LibraryUI.Populate();
        }
    }
}
