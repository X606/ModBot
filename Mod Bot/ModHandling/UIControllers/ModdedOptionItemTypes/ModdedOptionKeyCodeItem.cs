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
    /// Used by Mod-Bot to reprecent a KeyCode item in a modded options window
    /// </summary>
    public class ModdedOptionKeyCodeItem : ModdedOptionPageItem
    {
        /// <summary>
        /// The default keycode
        /// </summary>
        public KeyCode DefaultValue;

        /// <summary>
        /// called when the KeyCodeInput item is created
        /// </summary>
        public Action<KeyCodeInput> OnCreate;
        /// <summary>
        /// Called when the keycode is changed
        /// </summary>
        public Action<KeyCode> OnChange;

        static PooledPrefab _keyCodeInputPool;
        /// <summary>
        /// The prefab pool for the UI element instantiated for this instance
        /// </summary>
        public override PooledPrefab ItemPool
        {
            get
            {
                if (_keyCodeInputPool == null || _keyCodeInputPool.Prefab == null)
                {
                    _keyCodeInputPool = new PooledPrefab
                    {
                        AddPoolReference = false,
                        MaxCount = -1,
                        UnparentOnDisable = true,
                        Prefab = InternalAssetBundleReferences.ModBot.GetObject("CustomKeyCodeInput").transform
                    };
                }

                return _keyCodeInputPool;
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

            KeyCodeInput keyCodeInput = spawnedPrefab.GetComponent<KeyCodeInput>();
            if (keyCodeInput == null)
                keyCodeInput = spawnedPrefab.gameObject.AddComponent<KeyCodeInput>();

            keyCodeInput.Init(DefaultValue, delegate (KeyCode keyCode)
            {
                OptionsSaver.SetSetting(owner, SaveID, (int)keyCode, true);

                if (OnChange != null)
                    OnChange(keyCode);
            });

            object loadedValue = OptionsSaver.LoadSetting(owner, SaveID);
            if(loadedValue != null && loadedValue is int intValue && intValue != (int)DefaultValue)
                keyCodeInput.SelectedKey = (KeyCode)intValue;

            keyCodeInput.GetComponent<ModdedObject>().GetObject<Text>(2).text = DisplayName;

            applyCustomRect(spawnedPrefab);

            if(OnCreate != null)
                OnCreate(keyCodeInput);
        }

    }
}
