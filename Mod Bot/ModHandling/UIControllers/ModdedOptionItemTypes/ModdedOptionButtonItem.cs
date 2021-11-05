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
    /// Used by Mod-Bot to reprecent a button in a modded options page
    /// </summary>
    public class ModdedOptionButtonItem : ModdedOptionPageItem
    {
        /// <summary>
        /// If is not null, is called when the user clicks the button
        /// </summary>
        public Action OnClick;
        /// <summary>
        /// Is called when the button is spawned
        /// </summary>
        public Action<Button> OnCreate;

        static PooledPrefab _buttonPool;
        /// <summary>
        /// The prefab pool for the UI element instantiated for this instance
        /// </summary>
        public override PooledPrefab ItemPool
        {
            get
            {
                if (_buttonPool == null || _buttonPool.Prefab == null)
                {
                    _buttonPool = new PooledPrefab
                    {
                        AddPoolReference = false,
                        MaxCount = -1,
                        UnparentOnDisable = true,
                        Prefab = InternalAssetBundleReferences.ModBot.GetObject("Button").transform
                    };
                }

                return _buttonPool;
            }
        }

        /// <summary>
        /// Places the page item in the page
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="owner"></param>
        public override void CreatePageItem(GameObject holder, Mod owner)
        {
            Transform buttonTransform = ItemPool.InstantiateNewObject();
            buttonTransform.SetParent(holder.transform, false);

            ModdedObject spawnedModdedObject = buttonTransform.GetComponent<ModdedObject>();
            Button button = spawnedModdedObject.GetObject<Button>(0);
            button.onClick.AddListener(delegate { OnClick(); });
            spawnedModdedObject.GetObject<Text>(1).text = DisplayName;

            applyCustomRect(buttonTransform);

            if (OnCreate != null)
                OnCreate(button);
        }

    }
}
