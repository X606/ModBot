﻿using System;
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

        /// <summary>
        /// Places the page item in the page
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="owner"></param>
        public override void CreatePageItem(GameObject holder, Mod owner)
        {
            KeyCodeInput keyCodeInput = GameObject.Instantiate(AssetLoader.GetObjectFromFile("modswindow", "CustomKeyCodeInput", "Clone Drone in the Danger Zone_Data/")).AddComponent<KeyCodeInput>();
            keyCodeInput.transform.parent = holder.transform;
            keyCodeInput.Init(DefaultValue, delegate (KeyCode keyCode)
            {
                OptionsSaver.SaveInt(owner, SaveID, (int)keyCode);
                OnChange(keyCode);
            });
            int? loadedKey = OptionsSaver.LoadInt(owner, SaveID);
            if(loadedKey.HasValue && loadedKey.Value != (int)DefaultValue)
            {
                keyCodeInput.SelectedKey = (KeyCode)loadedKey;
            }

            keyCodeInput.GetComponent<ModdedObject>().GetObject<Text>(2).text = DisplayName;

            applyCustomRect(keyCodeInput.gameObject);

            if(OnCreate != null)
                OnCreate(keyCodeInput);
        }

    }
}