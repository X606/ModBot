using System;
using ModLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Used to represent slider items on a modded options page
    /// </summary>
    public abstract class ModdedOptionSliderItem : ModdedOptionPageItem
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

        static PooledPrefab _sliderPool;
        /// <summary>
        /// The prefab pool for the UI element instantiated for this instance
        /// </summary>
        public override PooledPrefab ItemPool
        {
            get
            {
                if (_sliderPool == null || _sliderPool.Prefab == null)
                {
                    _sliderPool = new PooledPrefab
                    {
                        AddPoolReference = false,
                        MaxCount = -1,
                        UnparentOnDisable = true,
                        Prefab = InternalAssetBundleReferences.ModBot.GetObject("Slider").transform
                    };
                }

                return _sliderPool;
            }
        }

        /// <summary>
        /// If this slider should be limited to whole numbers
        /// </summary>
        protected abstract bool wholeNumbers { get; }

        /// <summary>
        /// Places the page item in the page
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="owner"></param>
        public override void CreatePageItem(GameObject holder, Mod owner)
        {
            Transform spawnedPrefab = ItemPool.InstantiateNewObject();
            spawnedPrefab.SetParent(holder.transform, false);
            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            spawnedModdedObject.GetObject<Text>(0).text = DisplayName;
            Slider slider = spawnedModdedObject.GetObject<Slider>(1);
            slider.minValue = Min;
            slider.maxValue = Max;
            slider.value = DefaultValue;
            slider.wholeNumbers = wholeNumbers;
            Text numberDisplay = spawnedModdedObject.GetObject<Text>(2);

            object loadedValue = OptionsSaver.LoadSetting(owner, SaveID);
            if (loadedValue != null)
            {
                if (loadedValue is float floatValue)
                {
                    slider.value = floatValue;
                }
                else if (loadedValue is int intValue)
                {
                    slider.value = intValue;
                }
            }

            onValueChanged(slider.value);

            numberDisplay.text = slider.value.ToString();

            slider.onValueChanged.AddListener(delegate (float value)
            {
                OptionsSaver.SetSetting(owner, SaveID, (int)value, true);

                onValueChanged(value);

                numberDisplay.text = value.ToString();
            });

            applyCustomRect(spawnedPrefab);

            if (OnCreate != null)
                OnCreate(slider);
        }

        /// <summary>
        /// Invokes the OnChange callback
        /// </summary>
        /// <param name="newValue"></param>
        protected abstract void onValueChanged(float newValue);
    }
}
