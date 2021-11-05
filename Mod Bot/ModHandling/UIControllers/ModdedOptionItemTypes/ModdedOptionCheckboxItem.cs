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
    /// Used by Mod-Bot to reprecent checkbox items
    /// </summary>
    public class ModdedOptionCheckboxItem : ModdedOptionPageItem
    {
        /// <summary>
        /// The value of the checkbox by default
        /// </summary>
        public bool DefaultValue;

        /// <summary>
        /// Called when the toggle is spawned
        /// </summary>
        public Action<Toggle> OnCreate;
        /// <summary>
        /// called when the value of the toggle is changed
        /// </summary>
        public Action<bool> OnChange;

        static PooledPrefab _checkboxPool;
        /// <summary>
        /// The prefab pool for the UI element instantiated for this instance
        /// </summary>
        public override PooledPrefab ItemPool
        {
            get
            {
                if (_checkboxPool == null || _checkboxPool.Prefab == null)
                {
                    _checkboxPool = new PooledPrefab
                    {
                        AddPoolReference = false,
                        MaxCount = -1,
                        UnparentOnDisable = true,
                        Prefab = InternalAssetBundleReferences.ModBot.GetObject("Checkbox").transform
                    };
                }

                return _checkboxPool;
            }
        }

        /// <summary>
        /// Places the page item in the page
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="owner"></param>
        public override void CreatePageItem(GameObject holder, Mod owner)
        {
            Transform spawnedObject = ItemPool.InstantiateNewObject();
            spawnedObject.SetParent(holder.transform, false);
            ModdedObject moddedObject = spawnedObject.GetComponent<ModdedObject>();
            Toggle toggle = moddedObject.GetObject<Toggle>(0);
            toggle.isOn = DefaultValue;
            moddedObject.GetObject<GameObject>(1).GetComponent<Text>().text = DisplayName;

            object loadedBool = OptionsSaver.LoadSetting(owner, SaveID);
            if (loadedBool != null && loadedBool is bool boolValue)
                toggle.isOn = boolValue;

            if (OnChange != null)
                OnChange(toggle.isOn);

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                OptionsSaver.SetSetting(owner, SaveID, value, true);

                if (OnChange != null)
                    OnChange(value);
            });

            applyCustomRect(spawnedObject);

            if (OnCreate != null)
                OnCreate(toggle);
        }

    }
}
