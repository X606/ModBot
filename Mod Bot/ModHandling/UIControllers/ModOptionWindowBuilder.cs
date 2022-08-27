using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;
using InternalModBot;
using ModLibrary;

#pragma warning disable IDE1005 // Delegate invocation can be simplefied

namespace ModLibrary
{
    /// <summary>
    /// Used to place all of the options in the options window
    /// </summary>
    public partial class ModOptionsWindowBuilder
    {
        readonly GameObject _parentWindow;
        readonly Mod _ownerMod;
        List<Page> _pages = new List<Page>();

        internal ModOptionsWindowBuilder(GameObject parentWindow, Mod ownerMod)
        {
            parentWindow.SetActive(false);
            _parentWindow = parentWindow;
            _ownerMod = ownerMod;
            //_spawnedBase = InternalAssetBundleReferences.ModsWindow.InstantiateObject("ModOptionsCanvas");
            ModBotUIRoot.Instance.ModOptionsWindow.WindowObject.SetActive(true);
            ModBotUIRoot.Instance.ModOptionsWindow.WindowObject.AddComponent<CloseModOptionsWindowOnEscapeKey>().Init(this); // used to make sure we can close the window with escape
            ModBotUIRoot.Instance.ModOptionsWindow.XButton.onClick = new Button.ButtonClickedEvent();
            ModBotUIRoot.Instance.ModOptionsWindow.XButton.onClick.AddListener(CloseWindow);

            DelegateScheduler.Instance.Schedule(delegate
            {
                PopulatePages();
                if(_pages.Count >= 1)
                {
                    SetPage(_pages[0]);
                }
            }, -1f); // We do -1f here because the game might be paused and cause this to be triggered right after it is unpaused
        }
        
        /// <summary>
        /// Removes all of the page buttons and spawns in new ones
        /// </summary>
        public void PopulatePages()
        {
            GameUIRoot.Instance.SetEscMenuDisabled(true);
            RegisterShouldCursorBeEnabledDelegate.Register(shouldCurorBeEnabled);
            GameUIRoot.Instance.RefreshCursorEnabled();

            TransformUtils.DestroyAllChildren(ModBotUIRoot.Instance.ModOptionsWindow.PageButtonsHolder.transform);
            GameObject buttonPrefab = InternalAssetBundleReferences.ModBot.GetObject("PageButton");

            foreach(Page page in _pages)
            {
                GameObject spawnedButton = GameObject.Instantiate(buttonPrefab);
                spawnedButton.transform.SetParent(ModBotUIRoot.Instance.ModOptionsWindow.PageButtonsHolder.transform, false);
                ModdedObject moddedObject = spawnedButton.GetComponent<ModdedObject>();
                moddedObject.GetObject<Text>(0).text = page.Name;
                moddedObject.GetObject<Button>(1).onClick.AddListener(delegate { SetPage(page); });
            }
        }

        void SetPage(Page page)
        {
            TransformUtils.DestroyAllChildren(ModBotUIRoot.Instance.ModOptionsWindow.Content.transform);
            page.Populate(ModBotUIRoot.Instance.ModOptionsWindow.Content, _ownerMod);
        }

        /// <summary>
        /// Adds a new page, call methods on this page to add items to it. If a page with the same name already exists returns a reference to that page
        /// </summary>
        /// <param name="pageName">The name of the page to spawn</param>
        /// <param name="forcedHeight">If not null this will set the height of the page to this</param>
        /// <returns></returns>
        public Page AddPage(string pageName, float? forcedHeight = null)
        {
            foreach(Page item in _pages)
            {
                if(item.Name == pageName)
                {
                    return item;
                }
            }

            Page page = new Page(pageName, forcedHeight);

            _pages.Add(page);

            return page;
        }

        /// <summary>
        /// Closes the options window, this also opens its parent window (probably the mods window)
        /// </summary>
        public void CloseWindow()
        {
            RegisterShouldCursorBeEnabledDelegate.UnRegister(shouldCurorBeEnabled);

            ModBotUIRoot.Instance.ModOptionsWindow.WindowObject.SetActive(false);
            _parentWindow.SetActive(true);

            GameUIRoot.Instance.RefreshCursorEnabled();
        }

        /// <summary>
        /// Closes the options window, does NOT open the parent window
        /// </summary>
        public void ForceCloseWindow()
        {
            RegisterShouldCursorBeEnabledDelegate.UnRegister(shouldCurorBeEnabled);

            ModBotUIRoot.Instance.ModOptionsWindow.WindowObject.SetActive(false);
            GameUIRoot.Instance.SetEscMenuDisabled(false);

            GameUIRoot.Instance.RefreshCursorEnabled();
        }

        bool shouldCurorBeEnabled()
        {
            return true;
        }

        /// <summary>
        /// Represents a page in the mod options window
        /// </summary>
        public class Page
        {
            internal Page(string name, float? forcedHeight)
            {
                Name = name;
                ForcedHeight = forcedHeight;
                _items = new List<ModdedOptionPageItem>();
            }

            internal void Populate(GameObject container, Mod owner)
            {
                foreach(ModdedOptionPageItem item in _items)
                {
                    item.CreatePageItem(container, owner);
                }
            }

            /// <summary>The name of the page</summary>
            public readonly string Name;

            /// <summary>The forced height of the page, if null the height will be set automatically</summary>
            public readonly float? ForcedHeight;

            readonly List<ModdedOptionPageItem> _items;
            
            /// <summary>
            /// Adds a slider to the page with the passed arguements
            /// </summary>
            /// <param name="min">The minimum value of the slider</param>
            /// <param name="max">The maximum value of the slider</param>
            /// <param name="defaultValue">The value the slider should be set to by default</param>
            /// <param name="displayName">The text you want to display next to the slider</param>
            /// <param name="saveID">The Id used to get this value in the <see cref="ModdedSettings"/> class</param>
            /// <param name="onCreate">Called when the slider is created, use this to change properties of the slider</param>
            /// <param name="customRect">The custom rect of the slider, use this to change the position and scale of the slider</param>
            /// <param name="onChange">Called when the value of the slider is changed</param>
            public void AddSlider(float min, float max, float defaultValue, string displayName, string saveID, Action<Slider> onCreate = null, Rect? customRect = null, Action<float> onChange = null)
            {
                ModdedOptionSliderItem slider = new ModdedOptionSliderItem
                {
                    Min = min,
                    Max = max,
                    DefaultValue = defaultValue,
                    DisplayName = displayName,
                    SaveID = saveID,
                    OnCreate = onCreate,
                    CustomRect = customRect,
                    OnChange = onChange
                };

                _items.Add(slider);
            }

            /// <summary>
            /// Adds a slider with only whole numbers to the page with the passed arguements
            /// </summary>
            /// <param name="min">The minimum value of the slider</param>
            /// <param name="max">The maximum value of the slider</param>
            /// <param name="defaultValue">The value the slider should be set to by default</param>
            /// <param name="displayName">The text you want to display next to the slider</param>
            /// <param name="saveID">The Id used to get this value in the <see cref="ModdedSettings"/> class</param>
            /// <param name="onCreate">Called when the slider is created, use this to change properties of the slider</param>
            /// <param name="customRect">The custom rect of the slider, use this to change the position and scale of the slider</param>
            /// <param name="onChange">Called when the value of the slider is changed</param>
            public void AddIntSlider(int min, int max, int defaultValue, string displayName, string saveID, Action<Slider> onCreate = null, Rect? customRect = null, Action<int> onChange = null)
            {
                ModdedOptionIntSliderItem slider = new ModdedOptionIntSliderItem
                {
                    Min = min,
                    Max = max,
                    DefaultValue = defaultValue,
                    DisplayName = displayName,
                    SaveID = saveID,
                    OnCreate = onCreate,
                    CustomRect = customRect,
                    OnChange = onChange
                };

                _items.Add(slider);
            }

            /// <summary>
            /// Adds a <see cref="InputField"/> to the page with the passed arguements
            /// </summary>
            /// <param name="defaultValue">The value the <see cref="InputField"/> should be set to by default</param>
            /// <param name="displayName">The text you want to display next to the <see cref="InputField"/></param>
            /// <param name="saveID">The Id used to get this value in the <see cref="ModdedSettings"/> class</param>
            /// <param name="onCreate">Called when the <see cref="InputField"/> is created, use this to change properties of the <see cref="InputField"/></param>
            /// <param name="customRect">The custom rect of the <see cref="InputField"/>, use this to change the position and scale of the <see cref="InputField"/></param>
            /// <param name="onChange">Called when the value of the <see cref="InputField"/> is changed</param>
            public void AddInputField(string defaultValue, string displayName, string saveID, Action<InputField> onCreate = null, Rect? customRect = null, Action<string> onChange = null)
            {
                ModdedOptionInputFieldItem input = new ModdedOptionInputFieldItem
                {
                    DefaultValue = defaultValue,
                    DisplayName = displayName,
                    SaveID = saveID,
                    OnCreate = onCreate,
                    CustomRect = customRect,
                    OnChange = onChange
                };

                _items.Add(input);
            }

            /// <summary>
            /// Adds a verifying <see cref="InputField"/> to the page with the specified arguments
            /// </summary>
            /// <param name="defaultValue">The value the <see cref="InputField"/> should be set to by default (NOTE: This value will not be checked by the <see cref="ModdedOptionVerifyingInputFieldItem.Verify"/> predicate)</param>
            /// <param name="displayName">The text you want to display next to the <see cref="InputField"/></param>
            /// <param name="saveID">The Id used to get this value in the <see cref="ModdedSettings"/> class</param>
            /// <param name="verificationPredicate">The <see cref="Predicate{T}"/> to verify the contents of the <see cref="InputField"/> when it is changed</param>
            /// <param name="onCreate">Called when the <see cref="InputField"/> is created, use this to change properties of the <see cref="InputField"/></param>
            /// <param name="customRect">The custom rect of the <see cref="InputField"/>, use this to change the position and scale of the <see cref="InputField"/></param>
            /// <param name="onChange">Called when the value of the <see cref="InputField"/> is changed</param>
            public void AddVerifyingInputField(string defaultValue, string displayName, string saveID, Predicate<string> verificationPredicate, Action<InputField> onCreate = null, Rect? customRect = null, Action<string> onChange = null)
            {
                ModdedOptionVerifyingInputFieldItem verifyingInput = new ModdedOptionVerifyingInputFieldItem
                {
                    DefaultValue = defaultValue,
                    DisplayName = displayName,
                    SaveID = saveID,
                    Verify = verificationPredicate,
                    OnCreate = onCreate,
                    CustomRect = customRect,
                    OnChange = onChange
                };

                _items.Add(verifyingInput);
            }

            /// <summary>
            /// Adds a <see cref="Toggle"/> to the page with the passed arguements
            /// </summary>
            /// <param name="defaultValue">The value the <see cref="Toggle"/> should be set to by default</param>
            /// <param name="displayName">The text you want to display next to the <see cref="Toggle"/></param>
            /// <param name="saveID">The Id used to get this value in the <see cref="ModdedSettings"/> class</param>
            /// <param name="onCreate">Called when the <see cref="Toggle"/> is created, use this to change properties of the <see cref="Toggle"/></param>
            /// <param name="customRect">The custom rect of the <see cref="Toggle"/>, use this to change the position and scale of the <see cref="Toggle"/></param>
            /// <param name="onChange">Called when the value of the <see cref="Toggle"/> is changed</param>
            public void AddCheckbox(bool defaultValue, string displayName, string saveID, Action<Toggle> onCreate = null, Rect? customRect = null, Action<bool> onChange = null)
            {
                ModdedOptionCheckboxItem checkBox = new ModdedOptionCheckboxItem
                {
                    DefaultValue = defaultValue,
                    DisplayName = displayName,
                    SaveID = saveID,
                    OnCreate = onCreate,
                    CustomRect = customRect,
                    OnChange = onChange
                };

                _items.Add(checkBox);
            }

            /// <summary>
            /// Adds a <see cref="Dropdown"/> to the page with the passed arguements
            /// </summary>
            /// <param name="options">The options of the dropdown</param>
            /// <param name="defaultValue">The index of the options to set by default</param>
            /// <param name="displayName">The text you want to display next to the <see cref="Dropdown"/></param>
            /// <param name="saveID">The Id used to get this value in the <see cref="ModdedSettings"/> class</param>
            /// <param name="onCreate">Called when the <see cref="Dropdown"/> is created, use this to change properties of the <see cref="Dropdown"/></param>
            /// <param name="customRect">The custom rect of the <see cref="Dropdown"/>, use this to change the position and scale of the <see cref="Dropdown"/></param>
            /// <param name="onChange">Called when the value of the <see cref="Dropdown"/> is changed</param>
            public void AddDropdown(string[] options, int defaultValue, string displayName, string saveID, Action<Dropdown> onCreate = null, Rect? customRect = null, Action<int> onChange = null)
            {
                ModdedOptionDropDownItem dropdown = new ModdedOptionDropDownItem
                {
                    Options = options,
                    DefaultValue = defaultValue,
                    DisplayName = displayName,
                    SaveID = saveID,
                    OnCreate = onCreate,
                    CustomRect = customRect,
                    OnChange = onChange
                };

                _items.Add(dropdown);
            }
            /// <summary>
            /// Adds a <see cref="Dropdown"/> to the page with the options of the passed enum 
            /// </summary>
            /// <param name="defaultValue">The index of the enum to set by default</param>
            /// <param name="displayName">The text you want to display next to the <see cref="Dropdown"/></param>
            /// <param name="saveID">The Id used to get this value in the <see cref="ModdedSettings"/> class</param>
            /// <param name="onCreate">Called when the <see cref="Dropdown"/> is created, use this to change properties of the <see cref="Dropdown"/></param>
            /// <param name="customRect">The custom rect of the <see cref="Dropdown"/>, use this to change the position and scale of the <see cref="Dropdown"/></param>
            /// <param name="onChange">Called when the value of the <see cref="Dropdown"/> is changed</param>
            public void AddDropdown<T>(T defaultValue, string displayName, string saveID, Action<Dropdown> onCreate = null, Rect? customRect = null, Action<T> onChange = null)
            {
                if(!typeof(T).IsEnum)
                    throw new InvalidOperationException("Generic type must be an enum");

                string[] names = Enum.GetNames(typeof(T));
                AddDropdown(names, (int)(object)defaultValue, displayName, saveID, onCreate, customRect, delegate (int value)
                {
                    if (onChange != null)
                        onChange((T)(object)value);
                });
            }

            /// <summary>
            /// Adds a <see cref="KeyCodeInput"/> to the page with the passed arguements
            /// </summary>
            /// <param name="defaultValue">The value the <see cref="KeyCodeInput"/> should be set to by default</param>
            /// <param name="displayName">The text you want to display next to the <see cref="KeyCodeInput"/></param>
            /// <param name="saveID">The Id used to get this value in the <see cref="ModdedSettings"/> class</param>
            /// <param name="onCreate">Called when the <see cref="KeyCodeInput"/> is created, use this to change properties of the <see cref="KeyCodeInput"/></param>
            /// <param name="customRect">The custom rect of the <see cref="KeyCodeInput"/>, use this to change the position and scale of the <see cref="KeyCodeInput"/></param>
            /// <param name="onChange">Called when the value of the <see cref="KeyCodeInput"/> is changed</param>
            public void AddKeyCodeInput(KeyCode defaultValue, string displayName, string saveID, Action<KeyCodeInput> onCreate = null, Rect? customRect = null, Action<KeyCode> onChange = null)
            {
                ModdedOptionKeyCodeItem keyCodeItem = new ModdedOptionKeyCodeItem
                {
                    DefaultValue = defaultValue,
                    DisplayName = displayName,
                    SaveID = saveID,
                    OnCreate = onCreate,
                    CustomRect = customRect,
                    OnChange = onChange
                };

                _items.Add(keyCodeItem);
            }

            /// <summary>
            /// Adds a <see cref="Button"/> to the page with the passed arguements
            /// </summary>
            /// <param name="displayName">The text you want to display next to the <see cref="Button"/></param>
            /// <param name="onClick">Called when the user clicks on the created <see cref="Button"/></param>
            /// <param name="customRect">The custom rect of the <see cref="Button"/>, use this to change the position and scale of the <see cref="Button"/></param>
            /// <param name="onCreate">Called when the <see cref="Button"/> is created, use this to change properties of the <see cref="Button"/></param>
            public void AddButton(string displayName, Action onClick, Rect? customRect = null, Action<Button> onCreate = null)
            {
                ModdedOptionButtonItem button = new ModdedOptionButtonItem
                {
                    DisplayName = displayName,
                    OnClick = onClick,
                    CustomRect = customRect,
                    OnCreate = onCreate
                };

                _items.Add(button);
            }

            /// <summary>
            /// Adds a label (a bit of text) to the page
            /// </summary>
            /// <param name="displayName">The page to display</param>
            /// <param name="onCreate">Called when the label is created, use this to change the properties of the <see cref="Text"/></param>
            public void AddLabel(string displayName, Action<Text> onCreate = null)
            {
                ModdedOptionLabelItem label = new ModdedOptionLabelItem
                {
                    DisplayName = displayName,
                    OnCreate = onCreate
                };

                _items.Add(label);
            }

            /// <summary>
            /// Adds a generic page item, use this to add your own item types! To create a new item type simply make a class that extends <see cref="ModdedOptionPageItem"/> and pass a instance of it to this class
            /// </summary>
            /// <param name="customItem">The generic <see cref="ModdedOptionPageItem"/> to add</param>
            public void AddCustom(ModdedOptionPageItem customItem)
            {
                _items.Add(customItem);
            }
        }
        /// <summary>
        /// Used to represent a position and scale of items in modded option window pages
        /// </summary>
        public struct Rect
        {
            /// <summary>
            /// The position of the item, if null keeps default values
            /// </summary>
            public Vector2? Position;
            /// <summary>
            /// The Scale of the item, if null keeps defualt values
            /// </summary>
            public Vector2? Scale;

        }
    }

    /// <summary>
    /// A base class for modded option page items
    /// </summary>
    public abstract class ModdedOptionPageItem
    {
        /// <summary>
        /// The custom rect of the page, if null uses default values
        /// </summary>
        public ModOptionsWindowBuilder.Rect? CustomRect;
        /// <summary>
        /// The name that should be displayed on the option
        /// </summary>
        public string DisplayName;
        /// <summary>
        /// The Id of the option
        /// </summary>
        public string SaveID;

        /// <summary>
        /// Should create the object you want to spawn as a child of holder
        /// </summary>
        /// <param name="holder">The object that the spawned object should be a child of</param>
        /// <param name="owner">The mod who spawned the option</param>
        public abstract void CreatePageItem(GameObject holder, Mod owner);

        /// <summary>
        /// Applies the <see cref="CustomRect"/> to the passed <see cref="GameObject"/>, if <see cref="CustomRect"/> is not <see langword="null"/>.
        /// </summary>
        /// <param name="spawnedObject"></param>
        protected void applyCustomRect(GameObject spawnedObject)
        {
            if(!CustomRect.HasValue)
                return;

            LayoutElement element = spawnedObject.GetComponent<LayoutElement>();
            if(element == null)
                element = spawnedObject.AddComponent<LayoutElement>();

            element.ignoreLayout = true;

            RectTransform rectTransform = spawnedObject.GetComponent<RectTransform>();
            if (CustomRect.Value.Scale.HasValue)
                rectTransform.sizeDelta = CustomRect.Value.Scale.Value;
            if(CustomRect.Value.Position.HasValue)
                rectTransform.anchoredPosition = CustomRect.Value.Position.Value;
        }
    }

}