using ModLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// The window that helps get started at making a mod
    /// </summary>
    internal class ModCreationWindow : MonoBehaviour
    {
        /// <summary>
        /// Made this in case we don't need it
        /// </summary>
        public const bool IsImplemented = false;

        /// <summary>
        /// Getting started
        /// </summary>
        public const string GuideLink = "https://github.com/X606/ModBot/blob/master/Documentation/Getting%20Started.md";

        /// <summary>
        /// Tags that may be assigned to mod info
        /// </summary>
        public static readonly string[] Tags = new string[]
        {
            "New upgrades",
            "Level editor",
            "Weapons",
            "QoL",
            "Visual changes",
            "Cheats",
            "Other"
        };

        private readonly List<string> _selectedTags = new List<string>();

        /// <summary>
        /// Don't let player create mods while playing LBS, only on title screen!
        /// </summary>
        public static bool CanBeShown => IsImplemented && GameModeManager.IsOnTitleScreen();

        private bool _hasMadeProgress;
        public bool HasUserMadeProgress { get => _hasMadeProgress; set { _hasMadeProgress = value; RefreshCreateButton(); } }

        public GameObject TheGameObject;
        public InputField ModNameField;
        public InputField ModDescriptionField;
        public InputField ModIDField;
        public Button CreateButton;
        public Transform TagsContainer;
        public ModdedObject TagPrefab;

        /// <summary>
        /// Set the window up
        /// </summary>
        /// <param name="moddedObject"></param>
        public void Init(ModdedObject moddedObject)
        {
            TheGameObject = moddedObject.gameObject;
            ModNameField = moddedObject.GetObject_Alt<InputField>(0);
            ModDescriptionField = moddedObject.GetObject_Alt<InputField>(1);
            ModIDField = moddedObject.GetObject_Alt<InputField>(3);
            TagsContainer = moddedObject.GetObject_Alt<Transform>(12);
            TagPrefab = moddedObject.GetObject_Alt<ModdedObject>(11);
            TagPrefab.gameObject.SetActive(false);
            CreateButton = moddedObject.GetObject_Alt<Button>(4);
            CreateButton.onClick.AddListener(createMod);

            WebLinkButton w = moddedObject.GetObject_Alt<Button>(7).gameObject.AddComponent<WebLinkButton>();
            w.URL = GuideLink;
            moddedObject.GetObject_Alt<Button>(7).onClick.AddListener(w.OnButtonClicked);

            ModBotUIRoot.Instance.ModsWindow.CreateModButton.onClick.AddListener(TryShow);
            moddedObject.GetObject_Alt<Button>(8).onClick.AddListener(TryHide);

            TryHide();
        }

        /// <summary>
        /// Reset all input fields
        /// </summary>
        public void ResetWindow()
        {
            ModNameField.onValueChanged.RemoveAllListeners();
            HasUserMadeProgress = false;

            ModNameField.text = string.Empty;
            ModDescriptionField.text = string.Empty;
        }

        /// <summary>
        /// Select mod name input field
        /// </summary>
        public void FocusOnModNameField()
        {
            ModNameField.Select();
            ModNameField.onValueChanged.AddListener(onValueChange);
            ModDescriptionField.onValueChanged.AddListener(onValueChange);
            GenerateNewGUID();
            populateTags();
        }

        /// <summary>
        /// Generate new id for mod
        /// </summary>
        public void GenerateNewGUID()
        {
            ModIDField.text = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Select or deselect mod tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns><b>True</b> if the tag was added to selected ones, <b>False</b> if was removed</returns>
        public bool TrySelectTag(string tag)
        {
            HasUserMadeProgress = true;
            if (_selectedTags.Contains(tag))
            {
                _selectedTags.Remove(tag);
                return false;
            }
            _selectedTags.Add(tag);
            return true;
        }

        /// <summary>
        /// Check if we have selected tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool HasSelectedTag(string tag)
        {
            return _selectedTags.Contains(tag);
        }

        public bool CanFinishCreatingMod()
        {
            return HasUserMadeProgress && /*ModBotSignInUI.HasSignedIn &&*/ !string.IsNullOrEmpty(ModNameField.text) && !string.IsNullOrEmpty(ModDescriptionField.text);
        }

        public void RefreshCreateButton()
        {
            CreateButton.interactable = CanFinishCreatingMod();
        }

        /// <summary>
        /// Show the window if possible
        /// </summary>
        public void TryShow()
        {
            if (!CanBeShown)
            {
                return;
            }

            TheGameObject.SetActive(true);
            DelegateScheduler.Instance.Schedule(FocusOnModNameField, Time.deltaTime * 2);
        }

        /// <summary>
        /// Hide the window or ask user about quiting
        /// </summary>
        public void TryHide()
        {
            if (HasUserMadeProgress)
            {
                new Generic2ButtonDialogue("Do you want to continue?", "No, let me exit", delegate { HasUserMadeProgress = false; TryHide(); }, "Yes, continue", null, Generic2ButtonDialogeUI.ModDeletionSizeDelta);
                return;
            }

            ResetWindow();
            TheGameObject.SetActive(false);
        }

        private void populateTags()
        {
            _selectedTags.Clear();
            TransformUtils.DestroyAllChildren(TagsContainer);
            foreach (string tag in Tags)
            {
                ModdedObject m = Instantiate(TagPrefab, TagsContainer);
                m.gameObject.SetActive(true);
                m.GetObject_Alt<Text>(0).text = tag;
                m.gameObject.AddComponent<ModTagButton>().Init(tag);
            }
        }

        private void onValueChange(string anystring)
        {
            HasUserMadeProgress = true;
        }

        private void createMod()
        {
            string tagsString = string.Empty;
            int index = 0;
            if (_selectedTags.Count != 0)
                foreach (string tag in _selectedTags)
                {
                    if (index == _selectedTags.Count - 1)
                    {
                        tagsString += "\"" + tag + "\"";
                    }
                    else
                    {
                        tagsString += "\"" + tag + "\", ";
                    }
                    index++;
                }

            string finalString = "{\n\"DisplayName\": \"" + ModNameField.text + "\",\n\"UniqueID\": \"" + ModIDField.text + "\",\n\"MainDLLFileName\": \"\",\n\"Author\": \"" + ModBotSignInUI.CurrentUserName + "\",\n\"Version\": 1,\n\"ImageFileName\": \"\",\n\"Description\": \"" +
                ModDescriptionField.text + "\",\n\"ModDependencies\": [],\n\"Tags\": [ " + tagsString + " ]\n }";

            Debug.Log(finalString);

            // Create directory
            string path = ModsManager.Instance.ModFolderPath + ModNameField.text;
            Directory.CreateDirectory(path);

            // Create mod info file
            StreamWriter write = File.CreateText(path + "/ModInfo.json");
            write.WriteLine(finalString);
            write.Close();
        }
    }
}