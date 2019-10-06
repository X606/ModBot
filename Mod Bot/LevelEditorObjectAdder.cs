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
        private const string LEVEL_OBJECT_PREFIX = "Prefabs/LevelObjects/";
        private const string TEXTURE_PREFIX = "Images/modded/";

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
            
            if (!ModdedLevelObjectTextures.ContainsKey(texturePath))
            {
                ModdedLevelObjectTextures.Add(texturePath, texture);
            }
            if(!ModdedLevelObjectTransforms.ContainsKey(fullPath))
            {
                ModdedLevelObjectTransforms.Add(fullPath, transform);
            }

            DoubleValueHolder<string, string> doubleValueHolder = new DoubleValueHolder<string, string>(fullPath, texturePath);
            if (!ModdedLevelObjects.Contains(doubleValueHolder))
            {
                ModdedLevelObjects.Add(doubleValueHolder);
            } else
            {
                //debug.Log("Tried to add the same object to the editor twice, this wont do anything but try to avoid it", Color.yellow);
            }

        }

        internal static Object GetObjectData(string path)
        {
            if(ModdedLevelObjectTransforms.ContainsKey(path))
            {
                return ModdedLevelObjectTransforms[path];
            }
            if(ModdedLevelObjectTextures.ContainsKey(path))
            {
                return ModdedLevelObjectTextures[path];
            }

            return null;
        }
        internal static void OnLevelEditorStarted()
        {
            List<LevelObjectEntry> levelObjects = LevelObjectsLibraryManager.Instance.GetLevelObjectsInLibrary();
            List<LevelObjectEntry> visibleLevelObjects = LevelObjectsLibraryManager.Instance.GetVisibleLevelEditorObjects();

            foreach(DoubleValueHolder<string, string> item in ModdedLevelObjects)
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

        private static readonly Dictionary<string, Transform> ModdedLevelObjectTransforms = new Dictionary<string, Transform>();
        private static readonly Dictionary<string, Texture2D> ModdedLevelObjectTextures = new Dictionary<string, Texture2D>();

        private static readonly List<DoubleValueHolder<string, string>> ModdedLevelObjects = new List<DoubleValueHolder<string, string>>();
    }
}
