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
    /// Used by Mod-Bot to reprecent Slider items on modded option pages
    /// </summary>
    public class ModdedOptionSliderItem : ModdedOptionPageItem
    {
        /// <summary>
        /// The max value of the slider
        /// </summary>
        public float Max;
        /// <summary>
        /// The min value of the slider
        /// </summary>
        public float Min;
        /// <summary>
        /// The default value of the slider
        /// </summary>
        public float DefaultValue;

        /// <summary>
        /// Called when the slider is created
        /// </summary>
        public Action<Slider> OnCreate;
        /// <summary>
        /// Called when the value of the slider is changed
        /// </summary>
        public Action<float> OnChange;

        /// <summary>
        /// Places the page item in the page
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="owner"></param>
        public override void CreatePageItem(GameObject holder, Mod owner)
        {
            GameObject sliderPrefab = AssetLoader.GetObjectFromFile("modswindow", "Slider", "Clone Drone in the Danger Zone_Data/");
            ModdedObject moddedObject = GameObject.Instantiate(sliderPrefab).GetComponent<ModdedObject>();
            moddedObject.transform.parent = holder.transform;
            moddedObject.GetObject<Text>(0).text = DisplayName;
            Slider slider = moddedObject.GetObject<Slider>(1);
            slider.minValue = Min;
            slider.maxValue = Max;
            slider.value = DefaultValue;
            Text numberDisplay = moddedObject.GetObject<Text>(2);

            float? loadedFloat = OptionsSaver.LoadFloat(owner, SaveID);
            if(loadedFloat.HasValue)
            {
                slider.value = loadedFloat.Value;
            }

            if(OnChange != null)
            {
                OnChange(slider.value);
            }

            numberDisplay.text = slider.value.ToString();

            slider.onValueChanged.AddListener(delegate (float value)
            {
                OptionsSaver.SaveFloat(owner, SaveID, value);

                if(OnChange != null)
                {
                    OnChange(value);
                }

                numberDisplay.text = value.ToString();
            });

            applyCustomRect(moddedObject.gameObject);

            if (OnCreate != null)
                OnCreate(slider);
        }

    }
}
