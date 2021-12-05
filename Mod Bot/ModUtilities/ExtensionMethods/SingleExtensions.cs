using InternalModBot;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine;
using Newtonsoft.Json;

namespace ModLibrary
{
    /// <summary>
    /// Dont call these methods directly from here
    /// </summary>
    public static class SingleMethodExtensions
    {
        /// <summary>
        /// Gets the <see cref="UnityEngine.Object"/> at the specified index and casts it to type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of the object at the index</typeparam>
        /// <param name="moddedObject"></param>
        /// <param name="index">The index of the <see cref="ModdedObject.objects"/> <see cref="List{T}"/></param>
        /// <returns>The <see cref="UnityEngine.Object"/> at the specified index, casted to type <typeparamref name="T"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="moddedObject"/> is <see langword="null"/></exception>
        /// <exception cref="IndexOutOfRangeException">If the given index is outside the range of <see cref="ModdedObject.objects"/></exception>
        /// <exception cref="InvalidCastException">If the <see cref="UnityEngine.Object"/> at index <paramref name="index"/> is not of type <typeparamref name="T"/></exception>
        public static T GetObject<T>(this ModdedObject moddedObject, int index) where T : UnityEngine.Object
        {
            if (moddedObject == null)
                throw new ArgumentNullException(nameof(moddedObject));

            if (index < 0 || index >= moddedObject.objects.Count)
                throw new IndexOutOfRangeException("Given index was not in the range of the objects list:\tMin: 0 " + "Max: " + (moddedObject.objects.Count - 1) + ", Recieved: " + index);

            if (moddedObject.objects[index] is T)
                return moddedObject.objects[index] as T;

            throw new InvalidCastException("Object at index " + index + " could not be casted to type " + typeof(T).ToString());
        }

        /// <summary>
        /// Checks if the given <see cref="Mod"/> is currently activated
        /// </summary>
        /// <param name="mod"></param>
        /// <returns><see langword="true"/> of the <see cref="Mod"/> is enabled, <see langword="false"/> if it's disabled</returns>
        public static bool IsModEnabled(this Mod mod)
        {
            return mod.ModInfo.IsModEnabled;
        }

        public static LevelEditorLevelData GetLevelEditorLevelData(this LevelDescription levelDescription)
        {
            if (levelDescription is null)
                throw new ArgumentNullException(nameof(levelDescription));

            LevelEditorLevelData levelEditorLevelData = null;
			if (levelDescription.LevelTags.Contains(LevelTags.LevelEditor))
			{
				string prefabPath = "Data/LevelEditorLevels/" + levelDescription.PrefabName;
                UnityEngine.Object levelJsonObject = Resources.Load(prefabPath);
                if (levelJsonObject == null)
				{
					UnityEngine.Debug.LogError("[GetLevelEditorLevelData] Could not load level with path " + prefabPath + " | levelID: " + levelDescription.LevelID);
					return null;
				}

                levelEditorLevelData = JsonConvert.DeserializeObject<LevelEditorLevelData>((levelJsonObject as TextAsset).text, DataRepository.Instance.GetSettings());
			}

			if (levelDescription.IsStreamedMultiplayerLevel)
			{
				string levelJSON = MultiplayerLevelStreamingManager.Instance.GetLevelJSON(levelDescription.LevelID);
				levelEditorLevelData = JsonConvert.DeserializeObject<LevelEditorLevelData>(levelJSON, DataRepository.Instance.GetSettings());
			}
			else if (levelDescription.IsPlayfabHostedLevel)
			{
				bool loadedLevelData;
				if (GameVersionManager.IsConsoleBuild() && !GameVersionManager.IsUnityEditor())
				{
					loadedLevelData = DataRepository.Instance.levelDataContainer.TryGetLevelEditorLevelData(levelDescription.LevelJSONPath, out levelEditorLevelData);
				}
				else
				{
					loadedLevelData = DataRepository.Instance.TryLoad(levelDescription.LevelJSONPath, out levelEditorLevelData, false);
				}

				if (!loadedLevelData)
				{
					UnityEngine.Debug.LogError("[LevelManager.SpawnCurrentLevel] Count not load level " + levelDescription.LevelID);
                    return null;
				}
			}
			else if (levelDescription.IsLevelEditorLevel() && !GameModeManager.UsesWorkshopChallengeLevels())
			{
				levelEditorLevelData = LevelManager.Instance.LoadLevelEditorLevelData(levelDescription.LevelJSONPath);
			}

			return levelEditorLevelData;
		}
    }
}
