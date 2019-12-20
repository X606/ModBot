using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json;
using ModLibrary;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to save mod options
    /// </summary>
    public static class OptionsSaver
    {
        static List<KeyValuePair<string, object>> _loadedkeys = new List<KeyValuePair<string, object>>();

        [JsonIgnore]
        readonly static string _path = Application.persistentDataPath + "/SavedModSettings.json";

        static OptionsSaver()
        {
            if (System.IO.File.Exists(_path))
            {
                string json = System.IO.File.ReadAllText(_path);
                Load(json);
            }
        }

        /// <summary>
        /// Sets the loaded options from an input json string
        /// </summary>
        /// <param name="json"></param>
        public static void Load(string json)
        {
            _loadedkeys = JsonConvert.DeserializeObject<List<KeyValuePair<string, object>>>(json);

            if (_loadedkeys == null)
            {
                _loadedkeys = new List<KeyValuePair<string, object>>();
            }
        }

        /// <summary>
        /// Saves the current loaded options to a file
        /// </summary>
        static void save()
        {
            string json = JsonConvert.SerializeObject(_loadedkeys);
            System.IO.File.WriteAllText(_path, json);
        }

        /// <summary>
        /// Used to make sure that mods always get saved in the same format
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="saveFormat"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        static string generateSaveFormatString(Mod mod, SaveFormats saveFormat, string name)
        {
            string generatedString = mod.GetUniqueID();
            switch (saveFormat)
            {
                case SaveFormats.String:
                    generatedString += "_str_";
                    break;
                case SaveFormats.Int:
                    generatedString += "_int_";
                    break;
                case SaveFormats.Float:
                    generatedString += "_flt_";
                    break;
                case SaveFormats.Bool:
                    generatedString += "_bol_";
                    break;
            }
            generatedString += name;
            return generatedString;
        }

        static int? getIndexOfKey(string key)
        {
            for (int i = 0; i < _loadedkeys.Count; i++)
            {
                if (_loadedkeys[i].Key == key)
                {
                    return i;
                }
            }

            return null;
        }

        /// <summary>
        /// Saves a string to the save file and loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="id"></param>
        /// <param name="_string"></param>
        public static void SaveString(Mod mod, string id, string _string)
        {
            string key = generateSaveFormatString(mod, SaveFormats.String, id);
            int? index = getIndexOfKey(key);

            if (index.HasValue)
            {
                _loadedkeys[index.Value] = new KeyValuePair<string, object>(key, _string);
            }
            else
            {
                _loadedkeys.Add(new KeyValuePair<string, object>(key, _string));
            }

            save();
        }

        /// <summary>
        /// Loads a string from the loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string LoadString(Mod mod, string id)
        {
            if (_loadedkeys == null)
            {
                return null;
            }
            
            foreach (KeyValuePair<string, object> value in _loadedkeys)
            {
                if (value.Key == generateSaveFormatString(mod, SaveFormats.String, id))
                {
                    return value.Value as string;
                }
            }

            return null;
        }

        /// <summary>
        /// Saves a int to the save file and loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="id"></param>
        /// <param name="_int"></param>
        public static void SaveInt(Mod mod, string id, int _int)
        {
            string key = generateSaveFormatString(mod, SaveFormats.Int, id);
            int? index = getIndexOfKey(key);

            if (index.HasValue)
            {
                _loadedkeys[index.Value] = new KeyValuePair<string, object>(key, _int);
            }
            else
            {
                _loadedkeys.Add(new KeyValuePair<string, object>(key, _int));
            }
            
            save();
        }

        /// <summary>
        /// loads an int from the loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int? LoadInt(Mod mod, string id)
        {
            if (_loadedkeys == null)
            {
                return null;
            }
            
            foreach (KeyValuePair<string, object> value in _loadedkeys)
            {
                if (value.Key == generateSaveFormatString(mod, SaveFormats.Int, id))
                {
                    if (value.Value is int)
                    {
                        return (int)value.Value;
                    }
                    else if (value.Value is long)
                    {
                        return (int)(long)value.Value;
                    }
                    else
                    {
                        throw new Exception("Unrecognized Type \"" + value.Value.GetType().FullName + "\"");
                    }
                    
                }

            }
            
            return null;
        }

        /// <summary>
        /// Saves a float to the save file and loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="id"></param>
        /// <param name="_float"></param>
        public static void SaveFloat(Mod mod, string id, float _float)
        {
            string key = generateSaveFormatString(mod, SaveFormats.Float, id);
            int? index = getIndexOfKey(key);

            if (index.HasValue)
            {
                _loadedkeys[index.Value] = new KeyValuePair<string, object>(key, _float);
            }
            else
            {
                _loadedkeys.Add(new KeyValuePair<string, object>(key, _float));
            }

            save();
        }

        /// <summary>
        /// loads a float from the loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static float? LoadFloat(Mod mod, string id)
        {
            if(_loadedkeys == null)
            {
                return null;
            }

            foreach(KeyValuePair<string, object> value in _loadedkeys)
            {
                string generated = generateSaveFormatString(mod, SaveFormats.Float, id);

                if(value.Key == generated)
                {
                    if(value.Value is float)
                    {
                        return (float)value.Value;
                    }
                    else if(value.Value is double)
                    {
                        return (float)(double)value.Value;
                    }
                    else
                    {
                        throw new Exception("Unrecognized Type \"" + value.Value.GetType().FullName + "\"");
                    }

                }
            }

            return null;
        }

        /// <summary>
        /// Saves a bool to the save file and loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="id"></param>
        /// <param name="_bool"></param>
        public static void SaveBool(Mod mod, string id, bool _bool)
        {
            string key = generateSaveFormatString(mod, SaveFormats.Bool, id);
            int? index = getIndexOfKey(key);

            if (index.HasValue)
            {
                _loadedkeys[index.Value] = new KeyValuePair<string, object>(key, _bool);
            }
            else
            {
                _loadedkeys.Add(new KeyValuePair<string, object>(key, _bool));
            }

            save();
        }

        /// <summary>
        /// loads a bool from the loaded options
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool? LoadBool(Mod mod, string id)
        {
            if (_loadedkeys == null)
            {
                return null;
            }

            bool? outputBool = null;
            foreach (KeyValuePair<string, object> value in _loadedkeys)
            {
                if (value.Key == generateSaveFormatString(mod, SaveFormats.Bool, id))
                {
                    outputBool = value.Value as bool?;
                    break;
                }
            }

            return outputBool;
        }

        /// <summary>
        /// The diffrent types of save formats supported
        /// </summary>
        enum SaveFormats
        {
            String,
            Int,
            Float,
            Bool
        }
    }
}
