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
            GameObject sliderPrefab = AssetLoader.GetObjectFromFile("modswindow", "Slider", "Clone Drone in the Danger Zone_Data/");
            ModdedObject moddedObject = GameObject.Instantiate(sliderPrefab).GetComponent<ModdedObject>();
            moddedObject.transform.parent = holder.transform;
            moddedObject.GetObject<Text>(0).text = DisplayName;
            Slider slider = moddedObject.GetObject<Slider>(1);
            slider.minValue = Min;
            slider.maxValue = Max;
            slider.value = DefaultValue;
            slider.wholeNumbers = true;
            Text numberDisplay = moddedObject.GetObject<Text>(2);

            int? loadedInt = OptionsSaver.LoadInt(owner, SaveID);
            if(loadedInt.HasValue)
            {
                slider.value = loadedInt.Value;
            }

            if(OnChange != null)
            {
                OnChange((int)slider.value);
            }

            numberDisplay.text = slider.value.ToString();

            slider.onValueChanged.AddListener(delegate (float value)
            {
                int intValue = (int)value;
                OptionsSaver.SaveInt(owner, SaveID, intValue);

                if(OnChange != null)
                {
                    OnChange(intValue);
                }

                numberDisplay.text = value.ToString();
            });

            applyCustomRect(moddedObject.gameObject);

            if (OnCreate != null)
                OnCreate(slider);
        }

    }
}
