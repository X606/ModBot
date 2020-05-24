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

        /// <summary>
        /// Places the page item in the page
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="owner"></param>
        public override void CreatePageItem(GameObject holder, Mod owner)
        {
            KeyCodeInput keyCodeInput = InternalAssetBundleCache.ModsWindow.InstantiateObject("CustomKeyCodeInput").AddComponent<KeyCodeInput>();
            keyCodeInput.transform.parent = holder.transform;
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

            applyCustomRect(keyCodeInput.gameObject);

            if(OnCreate != null)
                OnCreate(keyCodeInput);
        }

    }
}
