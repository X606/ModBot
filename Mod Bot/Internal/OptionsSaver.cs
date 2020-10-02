using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json;
using ModLibrary;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to save mod options
    /// </summary>
    public static class OptionsSaver
    {
        static Dictionary<string, object> _savedSettingsDictionary = new Dictionary<string, object>();

        static readonly string _settingsFilePath = Application.persistentDataPath + "/SavedModSettings.json";

        static readonly string[] _oldSaveFormatTypeStrings = new string[] { "_str_", "_int_", "_flt_", "_bol_" };

        static OptionsSaver()
        {
            if (File.Exists(_settingsFilePath))
            {
                populateSettingDictionary();
            }
        }

        static void populateSettingDictionary()
        {
            string json = File.ReadAllText(_settingsFilePath);

            object deserializedObject = JsonConvert.DeserializeObject(json);

            if (deserializedObject is Dictionary<string, object> dictionary)
            {
                _savedSettingsDictionary = dictionary;
            }
            else if (deserializedObject is List<KeyValuePair<string, object>> list) // If this is the case we are migrating from the old format to the new
            {
                _savedSettingsDictionary = new Dictionary<string, object>();
                foreach (KeyValuePair<string, object> keyValuePair in list)
                {
                    if (_savedSettingsDictionary.ContainsKey(keyValuePair.Key))
                        continue;

                    _savedSettingsDictionary.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
            else if (deserializedObject is JArray jArray)
            {
                _savedSettingsDictionary = new Dictionary<string, object>();

                foreach (JObject jObject in jArray.Children<JObject>())
                {
                    string currentKey = "";
                    foreach (JProperty jProperty in jObject.Properties())
                    {
                        string key = jProperty.Name;
                        JToken value = jProperty.Value;

                        if (key == "Key") // This might seem very weird, but we are dealing with a serialized KeyValuePair, the keys in this context are the field names, so "Key", and "Value"
                        {
                            currentKey = (string)value; // We dont want the local variable 'key' to be the key here, the actual value of the key property is what we want to use as the key
                            continue;
                        }

                        if (key == "Value")
                        {
                            object settingValue = convertJTokenToSettingsValue(value);

                            if (settingValue != null)
                                _savedSettingsDictionary.Add(currentKey, settingValue);
                        }
                    }
                }
            }
            else if (deserializedObject is JObject jObject)
            {
                _savedSettingsDictionary = new Dictionary<string, object>();
                foreach (KeyValuePair<string, JToken> pair in jObject)
                {
                    object value = convertJTokenToSettingsValue(pair.Value);

                    if (value != null)
                        _savedSettingsDictionary.Add(pair.Key, value);
                }
            }
            else if (deserializedObject == null) // This happens if the settings file is empty
            {
                _savedSettingsDictionary = new Dictionary<string, object>();
            }
            else
            {
                throw new Exception("Cannot migrate settings file to dictionary type: Unsupported type: " + deserializedObject.GetType().FullName);
            }
        }

        static object convertJTokenToSettingsValue(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Boolean:
                    return token.ToObject<bool>();
                case JTokenType.Float:
                    return token.ToObject<float>();
                case JTokenType.Integer:
                    return token.ToObject<int>();
                case JTokenType.String:
                    return token.ToObject<string>();
                default:
                    debug.Log("Unsupported JToken type: " + token.Type, Color.red);
                    return null;
            }
        }

        static string getSaveIDForSetting(Mod owner, string providedSaveID)
        {
            return owner.GetUniqueID() + providedSaveID;
            // return owner.ModInfo.UniqueID + providedSaveID;
        }

        internal static void SaveToFile()
        {
            File.WriteAllText(_settingsFilePath, JsonConvert.SerializeObject(_savedSettingsDictionary));
        }

        internal static void SetSetting(Mod owner, string providedSaveID, object value, bool writeToFile)
        {
            string saveID = getSaveIDForSetting(owner, providedSaveID);

            _savedSettingsDictionary[saveID] = value;

            if (writeToFile)
                SaveToFile();
        }

        internal static object LoadSetting(Mod owner, string providedSaveID)
        {
            string saveID = getSaveIDForSetting(owner, providedSaveID);

            if (_savedSettingsDictionary.TryGetValue(saveID, out object value))
                return value;

            foreach (string typeString in _oldSaveFormatTypeStrings) // If the setting can't be found, look for it with the old save format instead, if a match is found with the old format, it is converted to the new
            {
                string oldSaveID = owner.GetUniqueID() + typeString + providedSaveID;
                // string oldSaveID = owner.ModInfo.UniqueID + typeString + providedSaveID;

                if (_savedSettingsDictionary.TryGetValue(oldSaveID, out object valueOldKey))
                {
                    _savedSettingsDictionary.Remove(oldSaveID);
                    _savedSettingsDictionary.Add(saveID, valueOldKey);

                    return valueOldKey;
                }
            }

            return null;
        }

        internal static bool HasSettingSaved(Mod owner, string providedSaveID)
        {
            return _savedSettingsDictionary.ContainsKey(getSaveIDForSetting(owner, providedSaveID));
        }
    }
}
