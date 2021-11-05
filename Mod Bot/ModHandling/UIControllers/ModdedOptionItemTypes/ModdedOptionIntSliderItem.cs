﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalModBot
{
    /// <summary>
    /// Used to reprecent Int slider items on a modded options page
    /// </summary>
    public class ModdedOptionIntSliderItem : ModdedOptionSliderItem
    {
        /// <summary>
        /// Called when the value of the slider is changed
        /// </summary>
        public Action<int> OnChange;

        /// <summary>
        /// If this slider should be limited to whole numbers
        /// </summary>
        protected override bool wholeNumbers => true;

        /// <summary>
        /// Invokes the OnChang callback
        /// </summary>
        /// <param name="newValue"></param>
        protected override void onValueChanged(float newValue)
        {
            OnChange?.Invoke((int)newValue);
        }
    }
}
