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
    /// Used by Mod-Bot to reprecent a dropdown item in a modded options page
    /// </summary>
    public class ModdedOptionDropDownItem : ModdedOptionPageItem
    {
        /// <summary>
        /// The dropdowns options
        /// </summary>
        public string[] Options;
        /// <summary>
        /// The default value of the dropdown
        /// </summary>
        public int DefaultValue;
        
        /// <summary>
        /// Called when the dropdown is created
        /// </summary>
        public Action<Dropdown> OnCreate;
        /// <summary>
        /// Called when the value of the dropdown is changed
        /// </summary>
        public Action<int> OnChange;

        /// <summary>
        /// Places the page item in the page
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="owner"></param>
        public override void CreatePageItem(GameObject holder, Mod owner)
        {
            if (Options.Length <= DefaultValue || DefaultValue < 0)
                throw new ArgumentOutOfRangeException(nameof(DefaultValue) + " must be in the bounds of the passed options");

            GameObject spawnedPrefab = InternalAssetBundleReferences.ModBot.InstantiateObject("DropDown");
            spawnedPrefab.transform.parent = holder.transform;
            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            spawnedModdedObject.GetObject<Text>(0).text = DisplayName;

            Dropdown dropdown = spawnedModdedObject.GetObject<Dropdown>(1);
            dropdown.options.Clear();

            foreach(string option in Options)
            {
                Dropdown.OptionData data = new Dropdown.OptionData(option);
                dropdown.options.Add(data);
            }
            dropdown.value = DefaultValue;
            dropdown.RefreshShownValue();

            object loadedValue = OptionsSaver.LoadSetting(owner, SaveID);
            if(loadedValue != null && loadedValue is int intValue)
            {
                dropdown.value = intValue;
                dropdown.RefreshShownValue();
            }

            if(OnChange != null)
                OnChange(dropdown.value);

            dropdown.onValueChanged.AddListener(delegate (int value)
            {
                OptionsSaver.SetSetting(owner, SaveID, value, true);

                if(OnChange != null)
                    OnChange(value);
            });

            applyCustomRect(spawnedPrefab);

            if(OnCreate != null)
                OnCreate(dropdown);
        }

    }
}
