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

        /// <summary>
        /// Places the page item in the page
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="owner"></param>
        public override void CreatePageItem(GameObject holder, Mod owner)
        {
            GameObject CheckBoxPrefab = AssetLoader.GetObjectFromFile("modswindow", "Checkbox", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedObject = GameObject.Instantiate(CheckBoxPrefab);
            spawnedObject.transform.parent = holder.transform;
            ModdedObject moddedObject = spawnedObject.GetComponent<ModdedObject>();
            Toggle toggle = moddedObject.GetObject<Toggle>(0);
            toggle.isOn = DefaultValue;
            moddedObject.GetObject<GameObject>(1).GetComponent<Text>().text = DisplayName;

            bool? loadedBool = OptionsSaver.LoadBool(owner, SaveID);
            if(loadedBool.HasValue)
                toggle.isOn = loadedBool.Value;

            if(OnChange != null)
                OnChange(toggle.isOn);

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                OptionsSaver.SaveBool(owner, SaveID, value);

                if(OnChange != null)
                {
                    OnChange(value);
                }
            });

            applyCustomRect(moddedObject.gameObject);

            if (OnCreate != null)
                OnCreate(toggle);
        }

    }
}
