using InternalModBot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLibrary
{
    /// <summary>
    /// Handles reading/writing modded level metadata in the level editor
    /// </summary>
    public static class LevelEditorModdedMetadataManager
    {
        /// <summary>
        /// Returns if the level editor is active, and a level is loaded
        /// </summary>
        /// <returns></returns>
        public static bool IsCurrentlyEditingLevel()
        {
            return GameModeManager.IsInLevelEditor() && LevelEditorDataManager.Instance != null && LevelEditorDataManager.Instance.GetCurrentLevelData() != null;
        }

        /// <summary>
        /// Attempts to set the modded metadata of the current level open in the level editor
        /// </summary>
        /// <remarks>
        /// Modded metadata is stored in the <see cref="LevelEditorLevelData"/> class as a <see cref="Dictionary{TKey, TValue}"/>(<see cref="string"/>, <see cref="string"/>)
        /// <br/>
        /// This method uses the caller's ModID as the key in this <see cref="Dictionary{TKey, TValue}"/>, and a json serialized <see cref="Dictionary{TKey, TValue}"/>(<see cref="string"/>, <see cref="string"/>) as its value, the <paramref name="key"/> and <paramref name="value"/> arguments are used as the key and value in this serialized <see cref="Dictionary{TKey, TValue}"/>
        /// </remarks>
        /// <param name="key">The key to store the <paramref name="value"/> with, must not be <see langword="null"/>, empty, or whitespace</param>
        /// <param name="value">The value to store with the <paramref name="key"/>, can be any string value, including <see langword="null"/></param>
        /// <returns>If the metadata of the current level was successfully set</returns>
        /// <exception cref="ArgumentException"><paramref name="key"/> is <see langword="null"/>, empty, or whitespace</exception>
        public static bool TrySetMetadata(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));

            if (!IsCurrentlyEditingLevel())
                return false;

            Mod metadataOwner = InternalUtils.GetCallerModInstance();
            if (metadataOwner == null)
            {
                debug.Log("[LevelEditorModdedMetadataManager.TrySetMetadata] Unable to find caller mod instance, stack trace: " + new StackTrace().ToString());
                return false;
            }

            LevelEditorLevelData currentLevelData = LevelEditorDataManager.Instance.GetCurrentLevelData();
            if (currentLevelData.ModdedMetadata == null)
                currentLevelData.ModdedMetadata = new Dictionary<string, string>();

            string modID = metadataOwner.ModInfo.UniqueID;

            Dictionary<string, string> metadataForMod;
            if (currentLevelData.ModdedMetadata.TryGetValue(modID, out string serializedDictionary))
            {
                metadataForMod = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedDictionary);
            }
            else
            {
                metadataForMod = new Dictionary<string, string>();
            }

            metadataForMod[key] = value;

            currentLevelData.ModdedMetadata[modID] = JsonConvert.SerializeObject(metadataForMod);

            GlobalEventManager.Instance.Dispatch(GlobalEvents.LevelEditorLevelChanged);

            return true;
        }

        /// <summary>
        /// Attempts to read the modded metadata of the current level that is either open in the level editor or currently being played
        /// </summary>
        /// <remarks>
        /// Modded metadata is stored in the <see cref="LevelEditorLevelData"/> class as a <see cref="Dictionary{TKey, TValue}"/>(<see cref="string"/>, <see cref="string"/>)
        /// <br/>
        /// This method uses the caller's ModID as the key in this <see cref="Dictionary{TKey, TValue}"/>, and a json serialized <see cref="Dictionary{TKey, TValue}"/>(<see cref="string"/>, <see cref="string"/>) as its value, the <paramref name="key"/> and <paramref name="value"/> arguments are used as the key and value in this serialized <see cref="Dictionary{TKey, TValue}"/>
        /// </remarks>
        /// <param name="key">The key to read the stored value of, must not be <see langword="null"/>, empty, or whitespace</param>
        /// <param name="value">The stored metadata value, or <see langword="null"/> if the operation was unsuccessful.</param>
        /// <returns><see langword="true"/> if a value was successfully read from the level metadata, <see langword="false"/> if not</returns>
        /// <exception cref="ArgumentException"><paramref name="key"/> is <see langword="null"/>, empty, or whitespace</exception>
        public static bool TryGetMetadata(string key, out string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));

            Mod metadataOwner = InternalUtils.GetCallerModInstance();
            if (metadataOwner == null)
            {
                debug.Log("[LevelEditorModdedMetadataManager.TryGetMetadata] Unable to find caller mod instance, stack trace: " + new StackTrace().ToString());
                value = null;
                return false;
            }

            if (IsCurrentlyEditingLevel())
            {
                return tryGetMetadata(LevelEditorDataManager.Instance.GetCurrentLevelData(), metadataOwner, key, out value);
            }
            else
            {
                LevelDescription currentLevel = LevelManager.Instance.GetCurrentLevelDescription();
                if (currentLevel != null)
                {
                    return tryGetMetadata(currentLevel, metadataOwner, key, out value);
                }
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Attempts to read a value of the modded metadata of a <see cref="LevelDescription"/>
        /// </summary>
        /// <param name="levelDescription">The level to read the metadata of</param>
        /// <param name="key">The key to use when retrieving the metadata value</param>
        /// <param name="value">The value stored in the metadata with the specified key</param>
        /// <returns><see langword="true"/> if <paramref name="key"/> exists in the metadata of <paramref name="levelDescription"/>, and <see langword="false"/> if not, or the level data couldn't be loaded</returns>
        public static bool TryGetMetadata(this LevelDescription levelDescription, string key, out string value)
        {
            Mod metadataOwner = InternalUtils.GetCallerModInstance();
            if (metadataOwner == null)
            {
                debug.Log("[LevelEditorModdedMetadataManager.TryGetMetadata] Unable to find caller mod instance, stack trace: " + new StackTrace().ToString());
                value = null;
                return false;
            }

            return tryGetMetadata(levelDescription, metadataOwner, key, out value);
        }

        /// <summary>
        /// Attempts to read a value of the modded metadata of a <see cref="LevelEditorLevelData"/>
        /// </summary>
        /// <param name="levelData">The level data to read the metadata of</param>
        /// <param name="key">The key to use when retrieving the metadata value</param>
        /// <param name="value">The value stored in the metadata with the specified key</param>
        /// <returns><see langword="true"/> if <paramref name="key"/> exists in the metadata of <paramref name="levelData"/>, and <see langword="false"/> if not</returns>
        public static bool TryGetMetadata(this LevelEditorLevelData levelData, string key, out string value)
        {
            Mod metadataOwner = InternalUtils.GetCallerModInstance();
            if (metadataOwner == null)
            {
                debug.Log("[LevelEditorModdedMetadataManager.TryGetMetadata] Unable to find caller mod instance, stack trace: " + new StackTrace().ToString());
                value = null;
                return false;
            }

            return tryGetMetadata(levelData, metadataOwner, key, out value);
        }

        static bool tryGetMetadata(LevelDescription levelDescription, Mod owner, string key, out string value)
        {
            return tryGetMetadata(levelDescription.GetLevelEditorLevelData(), owner, key, out value);
        }

        static bool tryGetMetadata(LevelEditorLevelData levelData, Mod owner, string key, out string value)
        {
            if (levelData == null)
            {
                debug.Log($"[LevelEditorModdedMetadataManager.tryGetMetadata] {nameof(levelData)} is null!");
                value = null;
                return false;
            }

            string modID = owner.ModInfo.UniqueID;

            if (levelData.ModdedMetadata != null && levelData.ModdedMetadata.TryGetValue(modID, out string serializedDictionary) && !string.IsNullOrEmpty(serializedDictionary))
            {
                Dictionary<string, string> metadataForMod;
                try
                {
                    metadataForMod = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedDictionary);
                }
                catch (JsonException jsonError)
                {
                    throw new Exception($"JsonConvert.DeserializeObject failed with argument \"{serializedDictionary}\", {nameof(levelData)}: {levelData.GeneratedUniqueID}, {nameof(owner)}: {owner.ModInfo.MainDLLFileName}, {nameof(key)}: \"{key}\"", jsonError);
                }

                if (metadataForMod != null && metadataForMod.TryGetValue(key, out string storedValue))
                {
                    value = storedValue;
                    return true;
                }
            }

            value = null;
            return false;
        }
    }
}
