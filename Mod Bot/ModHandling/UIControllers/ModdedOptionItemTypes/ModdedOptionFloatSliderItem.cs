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
    public class ModdedOptionFloatSliderItem : ModdedOptionSliderItem
    {
        /// <summary>
        /// Called when the value of the slider is changed
        /// </summary>
        public Action<float> OnChange;

        /// <summary>
        /// If this slider should be limited to whole numbers
        /// </summary>
        protected override bool wholeNumbers => false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newValue"></param>
        protected override void onValueChanged(float newValue)
        {
            OnChange?.Invoke(newValue);
        }
    }
}
