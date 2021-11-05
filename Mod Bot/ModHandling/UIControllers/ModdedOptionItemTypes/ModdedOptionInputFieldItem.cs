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
    /// Used by Mod-Bot to reprecent a InputField in a modded options page
    /// </summary>
    public class ModdedOptionInputFieldItem : ModdedOptionPageItem
    {
        /// <summary>
        /// The default value of the input field
        /// </summary>
        public string DefaultValue;
        
        /// <summary>
        /// Called when the InputField is spawned
        /// </summary>
        public Action<InputField> OnCreate;
        /// <summary>
        /// Called when the content of the input field is changed
        /// </summary>
        public Action<string> OnChange;

        static PooledPrefab _inputFieldPool;
        /// <summary>
        /// The prefab pool for the UI element instantiated for this instance
        /// </summary>
        public override PooledPrefab ItemPool
        {
            get
            {
                if (_inputFieldPool == null || _inputFieldPool.Prefab == null)
                {
                    _inputFieldPool = new PooledPrefab
                    {
                        AddPoolReference = false,
                        MaxCount = -1,
                        UnparentOnDisable = true,
                        Prefab = InternalAssetBundleReferences.ModBot.GetObject("InputField").transform
                    };
                }

                return _inputFieldPool;
            }
        }

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
            InputField inputField = spawnedModdedObject.GetObject<InputField>(1);
            inputField.text = DefaultValue;

            object loadedValue = OptionsSaver.LoadSetting(owner, SaveID);
            if(loadedValue != null && loadedValue is string stringValue)
                inputField.text = stringValue;

            if(OnChange != null)
                OnChange(inputField.text);

            inputField.onValueChanged.AddListener(delegate (string value)
            {
                onChanged(value, inputField, owner);
            });

            applyCustomRect(spawnedPrefab);

            if (OnCreate != null)
                OnCreate(inputField);
        }

        /// <summary>
        /// Called when the input field is changed
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="inputField"></param>
        /// <param name="owner"></param>
        protected virtual void onChanged(string newValue, InputField inputField, Mod owner)
        {
            OptionsSaver.SetSetting(owner, SaveID, newValue, true);

            if (OnChange != null)
                OnChange(newValue);
        }
    }
}
