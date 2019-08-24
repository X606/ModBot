using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using ModLibrary;
using InternalModBot;

namespace ModLibrary
{
    public class ModOptionsWindowBuilder
    {
        private readonly GameObject Content;
        private readonly GameObject SpawnedBase;
        private readonly Button XButton;
        private readonly GameObject Owner;
        private readonly Mod OwnerMod;

        internal ModOptionsWindowBuilder(GameObject owner, Mod ownerMod)
        {
            owner.SetActive(false);
            Owner = owner;
            OwnerMod = ownerMod;
            GameObject modsWindowPrefab = AssetLoader.GetObjectFromFile("modswindow", "ModOptionsCanvas", "Clone Drone in the Danger Zone_Data/");
            SpawnedBase = GameObject.Instantiate(modsWindowPrefab);
            ModdedObject modObject = SpawnedBase.GetComponent<ModdedObject>();
            Content = modObject.GetObject<GameObject>(0);
            XButton = modObject.GetObject<Button>(1);
            XButton.onClick.AddListener(CloseWindow);


            // Debug stuff
            // int RandomNumber = UnityEngine.Random.Range(0, 40);

            // OptionsSaver.SaveString(OwnerMod, "test" + RandomNumber, DEBUG_RandomString(10) );
            // debug.Log( OptionsSaver.LoadString(OwnerMod, "testStringHaHa") );
        }

        public void AddSlider(float min, float max, float defaultValue, string name, Action<float> onChange = null)
        {
            GameObject SliderPrefab = AssetLoader.GetObjectFromFile("modswindow", "Slider", "Clone Drone in the Danger Zone_Data/");
            ModdedObject moddedObject = GameObject.Instantiate(SliderPrefab).GetComponent<ModdedObject>();
            moddedObject.transform.parent = Content.transform;
            moddedObject.GetObject<Text>(0).text = name;
            Slider slider = moddedObject.GetObject<Slider>(1);
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = defaultValue;

            float? loadedFloat = OptionsSaver.LoadFloat(OwnerMod, name);
            if (loadedFloat.HasValue)
            {
                slider.value = loadedFloat.Value;
            }
            onChange?.Invoke(slider.value);
            slider.onValueChanged.AddListener(delegate (float value) { OptionsSaver.SaveFloat(OwnerMod, name, value); onChange?.Invoke(value); });
        }
        public void AddIntSlider(int min, int max, int defaultValue, string name, Action<int> onChange = null)
        {
            GameObject SliderPrefab = AssetLoader.GetObjectFromFile("modswindow", "Slider", "Clone Drone in the Danger Zone_Data/");
            ModdedObject moddedObject = GameObject.Instantiate(SliderPrefab).GetComponent<ModdedObject>();
            moddedObject.transform.parent = Content.transform;
            moddedObject.GetObject<Text>(0).text = name;
            Slider slider = moddedObject.GetObject<Slider>(1);
            slider.minValue = min;
            slider.maxValue = max;
            slider.wholeNumbers = true;
            slider.value = defaultValue;

            int? loadedInt = OptionsSaver.LoadInt(OwnerMod, name);
            if (loadedInt.HasValue)
            {
                slider.value = loadedInt.Value;
            }
            onChange?.Invoke((int)slider.value);
            slider.onValueChanged.AddListener(delegate (float value) { OptionsSaver.SaveInt(OwnerMod, name, (int)value); onChange?.Invoke((int)value); });
        }
        public void AddCheckbox(bool defaultValue, string name, Action<bool> onChange = null)
        {
            GameObject CheckBoxPrefab = AssetLoader.GetObjectFromFile("modswindow", "Checkbox", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedObject = GameObject.Instantiate(CheckBoxPrefab);
            spawnedObject.transform.parent = Content.transform;
            ModdedObject moddedObject = spawnedObject.GetComponent<ModdedObject>();
            Toggle toggle = moddedObject.GetObject<Toggle>(0);
            toggle.isOn = defaultValue;
            moddedObject.GetObject<GameObject>(1).GetComponent<Text>().text = name;

            bool? loadedBool = OptionsSaver.LoadBool(OwnerMod, name);
            if (loadedBool.HasValue)
            {
                toggle.isOn = loadedBool.Value;
            }
            onChange?.Invoke(toggle.isOn);
            toggle.onValueChanged.AddListener(delegate (bool value) { OptionsSaver.SaveBool(OwnerMod, name, value); onChange?.Invoke(value); });
        }
        public void AddInputField(string defaultValue, string name, Action<string> onChange = null)
        {
            GameObject InputFieldPrefab = AssetLoader.GetObjectFromFile("modswindow", "InputField", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedPrefab = GameObject.Instantiate(InputFieldPrefab);
            spawnedPrefab.transform.parent = Content.transform;
            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            spawnedModdedObject.GetObject<Text>(0).text = name;
            InputField field = spawnedModdedObject.GetObject<InputField>(1);
            field.text = defaultValue;

            string loadedString = OptionsSaver.LoadString(OwnerMod, name);
            if (loadedString != null)
            {
                field.text = loadedString;
            }
            onChange?.Invoke(field.text);
            field.onValueChanged.AddListener(delegate (string value) { OptionsSaver.SaveString(OwnerMod, name, value); onChange?.Invoke(value); });
        }
        public void AddDropdown(string[] options, int defaultIndex, string name, Action<int> onChange = null)
        {
            if (options.Length <= defaultIndex || defaultIndex < 0)
                return;

            GameObject dropdownPrefab = AssetLoader.GetObjectFromFile("modswindow", "DropDown", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedPrefab = GameObject.Instantiate(dropdownPrefab);
            spawnedPrefab.transform.parent = Content.transform;
            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            spawnedModdedObject.GetObject<Text>(0).text = name;

            Dropdown dropdown = spawnedModdedObject.GetObject<Dropdown>(1);
            dropdown.options.Clear();
            
            foreach (string option in options)
            {
                Dropdown.OptionData data = new Dropdown.OptionData(option);
                dropdown.options.Add(data);
            }
            dropdown.value = defaultIndex;
            dropdown.RefreshShownValue();

            int? loadedInt = OptionsSaver.LoadInt(OwnerMod, name);
            if (loadedInt.HasValue)
            {
                dropdown.value = loadedInt.Value;
                dropdown.RefreshShownValue();
            }
            onChange?.Invoke(dropdown.value);
            dropdown.onValueChanged.AddListener(delegate (int value) { OptionsSaver.SaveInt(OwnerMod, name, value); onChange?.Invoke(value); });
        }
        public void AddDropDown<T>(int defaultIndex, string name, Action<int> onChange = null) where T : IComparable, IFormattable, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("The generic type T must be an enum type");
            }
            List<string> enums = ModTools.EnumTools.GetNames<T>();
            AddDropdown(enums.ToArray(), defaultIndex, name, onChange);

        }

        public void AddButton(string text, UnityEngine.Events.UnityAction callback)
        {
            GameObject buttonPrefab = AssetLoader.GetObjectFromFile("modswindow", "Button", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedPrefab = GameObject.Instantiate(buttonPrefab);
            spawnedPrefab.transform.parent = Content.transform;
            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            spawnedModdedObject.GetObject<Button>(0).onClick.AddListener(callback);
            spawnedModdedObject.GetObject<Text>(1).text = text;
        }
        public void AddLabel(string text)
        {
            GameObject labelPrefab = AssetLoader.GetObjectFromFile("modswindow", "Label", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedPrefab = GameObject.Instantiate(labelPrefab);
            spawnedPrefab.transform.parent = Content.transform;
            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            spawnedModdedObject.GetObject<Text>(0).text = text;
        }


        public void CloseWindow()
        {
            GameObject.Destroy(SpawnedBase);
            Owner.SetActive(true);
        }


    }
}
namespace InternalModBot {

    public static class OptionsSaver
    {
        private static List<KeyValuePair<string, object>> Loadedkeys = new List<KeyValuePair<string, object>>();

        [JsonIgnore]
        private readonly static string Path;

        static OptionsSaver()
        {
            Path = Application.persistentDataPath + "/SavedModSettings.json";
            if (System.IO.File.Exists(Path))
            {
                string json = System.IO.File.ReadAllText(Path);
                Load(json);
            }
        }
        public static void Load(string json)
        {
            Loadedkeys = JsonConvert.DeserializeObject<List<KeyValuePair<string, object>>>(json);
            if (Loadedkeys == null)
            {
                Loadedkeys = new List<KeyValuePair<string, object>>();
            }
        }
        private static void Save()
        {
            string json = JsonConvert.SerializeObject(Loadedkeys);
            System.IO.File.WriteAllText(Path, json);
        }

        private static string GenerateSaveFormatString(Mod mod, SaveFormats saveFormat, string name)
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

        private enum SaveFormats
        {
            String,
            Int,
            Float,
            Bool
        }

        private static int? GetIndexOfKey(string key)
        {
            for (int i = 0; i < Loadedkeys.Count; i++)
            {
                if (Loadedkeys[i].Key == key)
                {
                    return i;
                }
            }

            return null;
        }

        public static void SaveString(Mod mod, string name, string _string)
        {
            string key = GenerateSaveFormatString(mod, SaveFormats.String, name);
            int? index = GetIndexOfKey(key);
            if (index != null)
            {
                Loadedkeys[index.Value] = new KeyValuePair<string, object>(key, _string);
            } else
            {
                Loadedkeys.Add(new KeyValuePair<string, object>(key, _string));
            }

            Save();
        }
        public static string LoadString(Mod mod, string name)
        {
            if (Loadedkeys == null)
                return null;

            string outputString = null;
            foreach (KeyValuePair<string, object> value in Loadedkeys)
            {
                if (value.Key == GenerateSaveFormatString(mod, SaveFormats.String, name))
                {
                    outputString = value.Value as string;
                    debug.Log("setting string stuff a \"" + outputString + "\"");
                    break;
                }
            }

            return outputString;
        }

        public static void SaveInt(Mod mod, string name, int _int)
        {
            string key = GenerateSaveFormatString(mod, SaveFormats.Int, name);
            int? index = GetIndexOfKey(key);
            if (index != null)
            {
                Loadedkeys[index.Value] = new KeyValuePair<string, object>(key, _int);
            }
            else
            {
                Loadedkeys.Add(new KeyValuePair<string, object>(key, _int));
            }
            
            Save();
        }
        public static int? LoadInt(Mod mod, string name)
        {
            if (Loadedkeys == null)
                return null;

            int? outputInt = null;
            foreach (KeyValuePair<string, object> value in Loadedkeys)
            {
                if (value.Key == GenerateSaveFormatString(mod, SaveFormats.Int, name))
                {
                    outputInt = value.Value as int?;
                    break;
                }
            }
            
            return outputInt;
        }

        public static void SaveFloat(Mod mod, string name, float _float)
        {
            string key = GenerateSaveFormatString(mod, SaveFormats.Float, name);
            int? index = GetIndexOfKey(key);
            if (index != null)
            {
                Loadedkeys[index.Value] = new KeyValuePair<string, object>(key, _float);
            }
            else
            {
                Loadedkeys.Add(new KeyValuePair<string, object>(key, _float));
            }

            Save();
        }
        public static float? LoadFloat(Mod mod, string name)
        {
            if (Loadedkeys == null)
                return null;

            float? outputFloat = null;
            foreach (KeyValuePair<string, object> value in Loadedkeys)
            {
                if (value.Key == GenerateSaveFormatString(mod, SaveFormats.Float, name))
                {
                    outputFloat = value.Value as float?;
                    break;
                }
            }

            return outputFloat;
        }

        public static void SaveBool(Mod mod, string name, bool _bool)
        {
            string key = GenerateSaveFormatString(mod, SaveFormats.Bool, name);
            int? index = GetIndexOfKey(key);
            if (index != null)
            {
                Loadedkeys[index.Value] = new KeyValuePair<string, object>(key, _bool);
            }
            else
            {
                Loadedkeys.Add(new KeyValuePair<string, object>(key, _bool));
            }

            Save();
        }
        public static bool? LoadBool(Mod mod, string name)
        {
            if (Loadedkeys == null)
                return null;

            bool? outputBool = null;
            foreach (KeyValuePair<string, object> value in Loadedkeys)
            {
                if (value.Key == GenerateSaveFormatString(mod, SaveFormats.Bool, name))
                {
                    outputBool = value.Value as bool?;
                    break;
                }
            }

            return outputBool;
        }
    }

}
