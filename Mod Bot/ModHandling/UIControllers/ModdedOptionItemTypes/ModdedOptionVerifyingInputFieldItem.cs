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
    /// Used by Mod-Bot to represent an InputField that verifies input in a modded options page
    /// </summary>
    public class ModdedOptionVerifyingInputFieldItem : ModdedOptionInputFieldItem
    {
        /// <summary>
        /// Used to verify the input of the input field, if this returns <see langword="false"/>, the value will revert back to the previous value, and apply it normally otherwise.
        /// </summary>
        public Predicate<string> Verify;

        string _oldValue = string.Empty;

        /// <summary>
        /// Called when the input field is changed
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="inputField"></param>
        /// <param name="owner"></param>
        protected override void onChanged(string newValue, InputField inputField, Mod owner)
        {
            if (!Verify(newValue))
            {
                inputField.text = _oldValue;
                return;
            }

            _oldValue = newValue;
            base.onChanged(newValue, inputField, owner);
        }
    }
}
