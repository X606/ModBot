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

        /// <summary>
        /// Places the page item in the page
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="owner"></param>
        public override void CreatePageItem(GameObject holder, Mod owner)
        {
            GameObject InputFieldPrefab = AssetLoader.GetObjectFromFile("modswindow", "InputField", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedPrefab = GameObject.Instantiate(InputFieldPrefab);
            spawnedPrefab.transform.parent = holder.transform;
            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            spawnedModdedObject.GetObject<Text>(0).text = DisplayName;
            InputField inputField = spawnedModdedObject.GetObject<InputField>(1);
            inputField.text = DefaultValue;

            string loadedString = OptionsSaver.LoadString(owner, SaveID);
            if(loadedString != null)
            {
                inputField.text = loadedString;
            }

            if(OnChange != null)
            {
                OnChange(inputField.text);
            }

            inputField.onValueChanged.AddListener(delegate (string value)
            {
                OptionsSaver.SaveString(owner, SaveID, value);

                if(OnChange != null)
                {
                    OnChange(value);
                }
            });

            applyCustomRect(spawnedPrefab);

            if (OnCreate != null)
                OnCreate(inputField);
        }

    }
}
