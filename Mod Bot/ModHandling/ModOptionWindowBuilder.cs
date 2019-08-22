using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ModLibrary
{
    public class ModOptionsWindowBuilder
    {
        private GameObject Content;
        private GameObject SpawnedBase;
        private Button XButton;

        internal ModOptionsWindowBuilder()
        {
            GameObject modsWindowPrefab = AssetLoader.GetObjectFromFile("modswindow", "ModOptionsCanvas", "Clone Drone in the Danger Zone_Data/");
            SpawnedBase = GameObject.Instantiate(modsWindowPrefab);
            ModdedObject modObject = SpawnedBase.GetComponent<ModdedObject>();
            Content = modObject.GetObject<GameObject>(0);
            XButton = modObject.GetObject<Button>(1);
            XButton.onClick.AddListener(Destory);
        }
        
        public void AddSlider(float min, float max, float defaultValue, string name)
        {
            GameObject SliderPrefab = AssetLoader.GetObjectFromFile("modswindow", "Slider", "Clone Drone in the Danger Zone_Data/");
            ModdedObject moddedObject = GameObject.Instantiate(SliderPrefab).GetComponent<ModdedObject>();
            moddedObject.transform.parent = Content.transform;
            moddedObject.GetObject<Text>(0).text = name;
            Slider slider = moddedObject.GetObject<Slider>(1);
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = defaultValue;
        }
        public void AddIntSlider(int min, int max, int defaultValue, string name)
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
        }
        public void AddCheckbox(bool defaultValue, string name)
        {
            GameObject CheckBoxPrefab = AssetLoader.GetObjectFromFile("modswindow", "Checkbox", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedObject = GameObject.Instantiate(CheckBoxPrefab);
            spawnedObject.transform.parent = Content.transform;
            ModdedObject moddedObject = spawnedObject.GetComponent<ModdedObject>();
            moddedObject.GetObject<Toggle>(0).isOn = defaultValue;
            moddedObject.GetObject<GameObject>(1).GetComponent<Text>().text = name;
            
        }
        public void AddInputField(string defaultValue, string name)
        {
        }
        public void AddDropdown(string[] options, int defaultIndex, string name)
        {
        }

        public void AddButton(string text, Action callback)
        {
        }

        public void AddLabel(string text)
        {
        }


        public void Destory()
        {
            GameObject.Destroy(SpawnedBase);
        }
    }
}
