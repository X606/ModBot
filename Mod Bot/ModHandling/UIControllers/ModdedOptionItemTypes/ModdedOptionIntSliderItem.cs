using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Used to reprecent Int slider items on a modded options page
    /// </summary>
    public class ModdedOptionIntSliderItem : ModdedOptionPageItem
    {
        /// <summary>
        /// The max value of the slider
        /// </summary>
        public int Max;
        /// <summary>
        /// The min value of the slider
        /// </summary>
        public int Min;
        /// <summary>
        /// The default value of the slider
        /// </summary>
        public int DefaultValue;

        /// <summary>
        /// Called when the slider is created
        /// </summary>
        public Action<Slider> OnCreate;
        /// <summary>
        /// Called when the value of the slider is changed
        /// </summary>
        public Action<int> OnChange;

        /// <summary>
        /// Places the page item in the page
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="owner"></param>
        public override void CreatePageItem(GameObject holder, Mod owner)
        {
            GameObject spawnedPrefab = InternalAssetBundleReferences.ModsWindow.InstantiateObject("Slider");
            spawnedPrefab.transform.parent = holder.transform;
            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            spawnedModdedObject.GetObject<Text>(0).text = DisplayName;
            Slider slider = spawnedModdedObject.GetObject<Slider>(1);
            slider.minValue = Min;
            slider.maxValue = Max;
            slider.value = DefaultValue;
            slider.wholeNumbers = true;
            Text numberDisplay = spawnedModdedObject.GetObject<Text>(2);

            object loadedValue = OptionsSaver.LoadSetting(owner, SaveID);
            if(loadedValue != null && loadedValue is int intValue)
                slider.value = intValue;

            if(OnChange != null)
                OnChange((int)slider.value);

            numberDisplay.text = slider.value.ToString();

            slider.onValueChanged.AddListener(delegate (float value)
            {
                OptionsSaver.SetSetting(owner, SaveID, (int)value, true);

                if(OnChange != null)
                    OnChange((int)value);

                numberDisplay.text = value.ToString();
            });

            applyCustomRect(spawnedModdedObject.gameObject);

            if (OnCreate != null)
                OnCreate(slider);
        }

    }
}
