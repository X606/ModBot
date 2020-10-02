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
        /// Places the page item in the page
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="owner"></param>
        public override void CreatePageItem(GameObject holder, Mod owner)
        {
            GameObject spawnedPrefab = InternalAssetBundleReferences.ModsWindow.InstantiateObject("InputField");
            spawnedPrefab.transform.parent = holder.transform;
            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            spawnedModdedObject.GetObject<Text>(0).text = DisplayName;
            InputField inputField = spawnedModdedObject.GetObject<InputField>(1);
            inputField.text = DefaultValue;

            object loadedValue = OptionsSaver.LoadSetting(owner, SaveID);
            if (loadedValue != null && loadedValue is string stringValue)
            {
                inputField.text = stringValue;
                _oldValue = stringValue;

                if (OnChange != null)
                    OnChange(inputField.text);
            }
            else
            {
                _oldValue = DefaultValue;
            }

            inputField.onEndEdit.AddListener(delegate (string value)
            {
                if (!Verify(value))
                {
                    inputField.text = _oldValue;
                    return;
                }

                _oldValue = value;

                OptionsSaver.SetSetting(owner, SaveID, value, true);

                if(OnChange != null)
                    OnChange(value);
            });

            applyCustomRect(spawnedPrefab);

            if (OnCreate != null)
                OnCreate(inputField);
        }

    }
}
