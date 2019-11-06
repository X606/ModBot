using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using InternalModBot;

namespace ModLibrary
{
    public partial class ModOptionsWindowBuilder
    {
        /// <summary>
        /// The name used for the page if a mod uses the old methods
        /// </summary>
        public const string LEGACY_PAGE_NAME = "Legacy support page";

        /// <summary>
        /// Adds KeyCodeInput, note that the value of the <see cref="KeyCode"/> gets saved by Mod-Bot so you dont need to worry about it
        /// </summary>
        /// <param name="defaultValue">The value you want the key to be bound to be default</param>
        /// <param name="name">The name of the slider, this will both be displayed to the user and used in the mod to get the value (no 2 names should EVER be the same)</param>
        /// <param name="keyCodeInput">The spawned <see cref="KeyCodeInput"/></param>
        /// <param name="onChange">Called when the selected key is changed</param>
        public void AddKeyCodeInput(KeyCode defaultValue, string name, out KeyCodeInput keyCodeInput, Action<KeyCode> onChange = null)
        {
            Page page = AddPage(LEGACY_PAGE_NAME);
            page.AddKeyCodeInput(defaultValue, name, name, null, null, onChange);

            keyCodeInput = null;
        }

        /// <summary>
        /// Adds KeyCodeInput, note that the value of the <see cref="KeyCode"/> gets saved by Mod-Bot so you dont need to worry about it
        /// </summary>
        /// <param name="defaultValue">The value you want the key to be bound to be default</param>
        /// <param name="name">The name of the slider, this will both be displayed to the user and used in the mod to get the value (no 2 names should EVER be the same)</param>
        /// <param name="onChange">Called when the selected key is changed</param>
        public void AddKeyCodeInput(KeyCode defaultValue, string name, Action<KeyCode> onChange = null)
        {
            AddKeyCodeInput(defaultValue, name, out KeyCodeInput input, onChange);
        }

        /// <summary>
        /// Adds a slider, note that the value of the slider will be saved by Mod-Bot so you dont need to save it in a ny way
        /// </summary>
        /// <param name="min">The minimum value of the slider</param>
        /// <param name="max">The maximum value of the slider</param>
        /// <param name="defaultValue">The value the slider will be set to before it is changed by the user</param>
        /// <param name="name">The name of the slider, this will both be displayed to the user and used in the mod to get the value (no 2 names should EVER be the same)</param>
        /// <param name="slider">A reference that is set to the created slider</param>
        /// <param name="onChange">A callback that gets called when the slider gets changed, if null wont do anything</param>
        public void AddSlider(float min, float max, float defaultValue, string name, out Slider slider, Action<float> onChange = null)
        {
            Page page = AddPage(LEGACY_PAGE_NAME);
            page.AddSlider(min,max, defaultValue, name, name, null, null, onChange);

            slider = null;
        }

        /// <summary>
        /// Adds a slider, note that the value of the slider will be saved by Mod-Bot so you dont need to save it in a ny way
        /// </summary>
        /// <param name="min">The minimum value of the slider</param>
        /// <param name="max">The maximum value of the slider</param>
        /// <param name="defaultValue">The value the slider will be set to before it is changed by the user</param>
        /// <param name="name">The name of the slider, this will both be displayed to the user and used in the mod to get the value (no 2 names should EVER be the same)</param>
        /// <param name="onChange">A callback that gets called when the slider gets changed, if null wont do anything</param>
        public void AddSlider(float min, float max, float defaultValue, string name, Action<float> onChange = null)
        {
            AddSlider(min, max, defaultValue, name, out Slider slider, onChange);
        }

        /// <summary>
        /// Adds a slider to the options window that can only be whole numbers
        /// </summary>
        /// <param name="min">The minimum value of the slider</param>
        /// <param name="max">That maximum value of the slider</param>
        /// <param name="defaultValue">The value the slider will be set to before it is changed by the user</param>
        /// <param name="name">Both the display name in the list and used by you to get the value (no 2 names should EVER be the same)</param>
        /// <param name="slider">A reference that is set to the created slider</param>
        /// <param name="onChange">Called when the value is changed, if null does nothing</param>
        public void AddIntSlider(int min, int max, int defaultValue, string name, out Slider slider, Action<int> onChange = null)
        {
            Page page = AddPage(LEGACY_PAGE_NAME);
            page.AddIntSlider(min, max, defaultValue, name, name, null, null, onChange);

            slider = null;
        }

        /// <summary>
        /// Adds a slider to the options window that can only be whole numbers
        /// </summary>
        /// <param name="min">The minimum value of the slider</param>
        /// <param name="max">That maximum value of the slider</param>
        /// <param name="defaultValue">The value the slider will be set to before it is changed by the user</param>
        /// <param name="name">Both the display name in the list and used by you to get the value (no 2 names should EVER be the same)</param>
        /// <param name="onChange">Called when the value is changed, if null does nothing</param>
        public void AddIntSlider(int min, int max, int defaultValue, string name, Action<int> onChange = null)
        {
            AddIntSlider(min, max, defaultValue, name, out Slider slider, onChange);
        }

        /// <summary>
        /// Adds a checkbox to the mods window
        /// </summary>
        /// <param name="defaultValue">The value the checkbox will be set to before the user changes it</param>
        /// <param name="name">Both the display name of the checkbox and what you use to get the value of the checkbox (no 2 names should EVER be the same)</param>
        /// <param name="toggle">>A reference that is set to the created toggle</param>
        /// <param name="onChange">Called when the value of the checkbox is changed, if null does nothing</param>
        public void AddCheckbox(bool defaultValue, string name, out Toggle toggle, Action<bool> onChange = null)
        {
            Page page = AddPage(LEGACY_PAGE_NAME);
            page.AddCheckbox(defaultValue, name, name, null, null, onChange);

            toggle = null;
        }

        /// <summary>
        /// Adds a checkbox to the mods window
        /// </summary>
        /// <param name="defaultValue">The value the checkbox will be set to before the user changes it</param>
        /// <param name="name">Both the display name of the checkbox and what you use to get the value of the checkbox (no 2 names should EVER be the same)</param>
        /// <param name="onChange">Called when the value of the checkbox is changed, if null does nothing</param>
        public void AddCheckbox(bool defaultValue, string name, Action<bool> onChange = null)
        {
            AddCheckbox(defaultValue, name, out Toggle toggle, onChange);
        }

        /// <summary>
        /// Adds a input field to the mods window
        /// </summary>
        /// <param name="defaultValue">The defualt value before it is edited by the user</param>
        /// <param name="name">Name used both as a display name and as a key for you to get the value by later (no 2 names should EVER be the same)</param>
        /// <param name="inputField">A reference to the created InputField</param>
        /// <param name="onChange">Gets called when the value of the inputField gets changed, if null doesnt nothing</param>
        public void AddInputField(string defaultValue, string name, out InputField inputField, Action<string> onChange = null)
        {
            Page page = AddPage(LEGACY_PAGE_NAME);
            page.AddInputField(defaultValue, name, name, null, null, onChange);

            inputField = null;
        }

        /// <summary>
        /// Adds a input field to the mods window
        /// </summary>
        /// <param name="defaultValue">The defualt value before it is edited by the user</param>
        /// <param name="name">Name used both as a display name and as a key for you to get the value by later (no 2 names should EVER be the same)</param>
        /// <param name="onChange">Gets called when the value of the inputField gets changed, if null doesnt nothing</param>
        public void AddInputField(string defaultValue, string name, Action<string> onChange = null)
        {
            AddInputField(defaultValue, name, out InputField inputField, onChange);
        }
        
        /// <summary>
        /// Adds a dropdown to the mods window
        /// </summary>
        /// <param name="options">The diffrent options that should be selectable</param>
        /// <param name="defaultIndex">what index in the previus array will be selected before the user edits it</param>
        /// <param name="name">Display name and key for you later (no 2 names should EVER be the same)</param>
        /// <param name="dropdown">a reference to the dropdown created, null if defaultIndex is not in the bounds of options</param>
        /// <param name="onChange">Gets called when the value of the dropdown is changed, if null does nothing</param>
        public void AddDropdown(string[] options, int defaultIndex, string name, out Dropdown dropdown, Action<int> onChange = null)
        {
            Page page = AddPage(LEGACY_PAGE_NAME);
            page.AddDropDown(options, defaultIndex, name, name, null, null, onChange);

            dropdown = null;
        }

        /// <summary>
        /// Adds a dropdown to the mods window
        /// </summary>
        /// <param name="options">The diffrent options that should be selectable</param>
        /// <param name="defaultIndex">what index in the previus array will be selected before the user edits it</param>
        /// <param name="name">Display name and key for you later (no 2 names should EVER be the same)</param>
        /// <param name="onChange">Gets called when the value of the dropdown is changed, if null does nothing</param>
        public void AddDropdown(string[] options, int defaultIndex, string name, Action<int> onChange = null)
        {
            AddDropdown(options, defaultIndex, name, out Dropdown dropdown, onChange);
        }

        /// <summary>
        /// Adds a dropdown to the options window
        /// </summary>
        /// <typeparam name="T">Must be an enum type, the options of this enum will be displayed as the options of the dropdown</typeparam>
        /// <param name="defaultIndex">The index in the enum that will be selected before the user edits it</param>
        /// <param name="name">Display name and key to get value (no 2 names should EVER be the same)</param>
        /// <param name="dropdown">a refrence to the dropdown created</param>
        /// <param name="onChange"></param>
        public void AddDropDown<T>(int defaultIndex, string name, out Dropdown dropdown, Action<int> onChange = null) where T : IComparable, IFormattable, IConvertible
        {
            Page page = AddPage(LEGACY_PAGE_NAME);
            page.AddDropdown<T>((T)((object)defaultIndex), name, name, null, null, delegate(T val) { onChange((int)((object)val)); });

            dropdown = null;
        }
        
        /// <summary>
        /// Adds a dropdown to the options window
        /// </summary>
        /// <typeparam name="T">Must be an enum type, the options of this enum will be displayed as the options of the dropdown</typeparam>
        /// <param name="defaultIndex">The index in the enum that will be selected before the user edits it</param>
        /// <param name="name">Display name and key to get value (no 2 names should EVER be the same)</param>
        /// <param name="onChange"></param>
        public void AddDropDown<T>(int defaultIndex, string name, Action<int> onChange = null) where T : IComparable, IFormattable, IConvertible
        {
            AddDropDown<T>(defaultIndex, name, out Dropdown dropdown, onChange);
        }

        /// <summary>
        /// Adds a button to the options window
        /// </summary>
        /// <param name="text">The text displayed on the button</param>
        /// <param name="button">a refrence to the created button</param>
        /// <param name="callback">Called when the user clicks the button</param>
        public void AddButton(string text, out Button button, UnityEngine.Events.UnityAction callback)
        {
            Page page = AddPage(LEGACY_PAGE_NAME);
            page.AddButton(text, delegate { callback(); }, null);

            button = null;
        }

        /// <summary>
        /// Adds a button to the options window
        /// </summary>
        /// <param name="text">The text displayed on the button</param>
        /// <param name="callback">Called when the user clicks the button</param>
        public void AddButton(string text, UnityEngine.Events.UnityAction callback)
        {
            AddButton(text, out Button button, callback);
        }

        /// <summary>
        /// Adds a plain text to the options window
        /// </summary>
        /// <param name="text">string that will be displayed</param>
        /// <param name="_text">a refrence to the created text</param>
        public void AddLabel(string text, out Text _text)
        {
            Page page = AddPage(LEGACY_PAGE_NAME);
            page.AddLabel(text);

            _text = null;
        }

        /// <summary>
        /// Adds a plain text to the options window
        /// </summary>
        /// <param name="text">string that will be displayed</param>
        public void AddLabel(string text)
        {
            AddLabel(text, out _);
        }
    }
}
