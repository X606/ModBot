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

        /// <summary>
        /// Places the page item in the page
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="owner"></param>
        public override void CreatePageItem(GameObject holder, Mod owner)
        {
            GameObject spawnedPrefab = InternalAssetBundleCache.ModsWindow.InstantiateObject("Button");
            spawnedPrefab.transform.parent = holder.transform;

            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            Button button = spawnedModdedObject.GetObject<Button>(0);
            button.onClick.AddListener(delegate { OnClick(); });
            spawnedModdedObject.GetObject<Text>(1).text = DisplayName;

            applyCustomRect(spawnedPrefab);

            if(OnCreate != null)
                OnCreate(button);
        }

    }
}
