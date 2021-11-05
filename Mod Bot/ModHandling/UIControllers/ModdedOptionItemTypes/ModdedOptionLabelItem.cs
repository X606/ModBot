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
    /// Used by Mod-Bot to reprecent label items on modded option pages
    /// </summary>
    public class ModdedOptionLabelItem : ModdedOptionPageItem
    {
        /// <summary>
        /// Called when the label is created
        /// </summary>
        public Action<Text> OnCreate;

        static PooledPrefab _labelPool;
        /// <summary>
        /// The prefab pool for the UI element instantiated for this instance
        /// </summary>
        public override PooledPrefab ItemPool
        {
            get
            {
                if (_labelPool == null || _labelPool.Prefab == null)
                {
                    _labelPool = new PooledPrefab
                    {
                        AddPoolReference = false,
                        MaxCount = -1,
                        UnparentOnDisable = true,
                        Prefab = InternalAssetBundleReferences.ModBot.GetObject("Label").transform
                    };
                }

                return _labelPool;
            }
        }

        /// <summary>
        /// Places the page item in the page
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="owner"></param>
        public override void CreatePageItem(GameObject holder, Mod owner)
        {
            Transform spawnedPrefab = ItemPool.InstantiateNewObject();;
            spawnedPrefab.SetParent(holder.transform, false);

            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            Text text = spawnedModdedObject.GetObject<Text>(0);
            text.text = DisplayName;

            applyCustomRect(spawnedPrefab);

            if(OnCreate != null)
                OnCreate(text);
        }

    }
}
