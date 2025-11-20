using ModLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to control most of the UI in Mod-Bot, this has control over the mod buttons and mods window. Note that all functions and fields on this class are private since they more or less work on their own.
    /// </summary>
    public class ModsPanelManager : Singleton<ModsPanelManager>
    {
        /// <summary>
        /// The color used for disabled mods
        /// </summary>
        public readonly Color DisabledModColor = new Color32(123, 14, 14, 255);

        private Action _actionOnModsPanelClose = null;

        private void Start()
        {
            Vector3 pauseScreenButtonOffset = new Vector3(0f, 1.2f, 0f);

            GameObject titleScreenContainer = TransformUtils.FindChildRecursive(GameUIRoot.Instance.TitleScreenUI.RootButtonsContainer, "BottomButtons").gameObject; // Gets the lower buttons container

            // Copy the options button to make into the Mods button
            GameObject modsButtonPrefab = TransformUtils.FindChildRecursive(titleScreenContainer.transform, "OptionsButton").gameObject; // Gets the options button (we copy it and replace its organs and face)
            GameObject mainMenuModsButton = Instantiate(modsButtonPrefab, titleScreenContainer.transform);

            mainMenuModsButton.GetComponentInChildren<LocalizedTextField>().LocalizationID = "modsbutton"; // Set LocalizationID
            mainMenuModsButton.transform.SetSiblingIndex(1);

            GameObject pauseScreenModsButton = Instantiate(GameUIRoot.Instance.EscMenu.SettingsButton.transform.gameObject, GameUIRoot.Instance.EscMenu.SettingsButton.transform.parent); // All of these lines edit the buttons on the pause menu
            GameUIRoot.Instance.EscMenu.ReturnToGameButton.transform.position += pauseScreenButtonOffset;
            GameUIRoot.Instance.EscMenu.SettingsButton.transform.position += pauseScreenButtonOffset;
            GameUIRoot.Instance.EscMenu.AchievementsButton.transform.position -= pauseScreenButtonOffset;
            GameUIRoot.Instance.EscMenu.ExitButton.transform.position -= pauseScreenButtonOffset;
            GameUIRoot.Instance.EscMenu.ExitConfirmUI.transform.position -= pauseScreenButtonOffset;
            GameUIRoot.Instance.EscMenu.MainMenuButton.transform.position -= pauseScreenButtonOffset;
            GameUIRoot.Instance.EscMenu.MainMenuConfirmUI.transform.position -= pauseScreenButtonOffset;

            pauseScreenModsButton.transform.position -= pauseScreenButtonOffset;
            pauseScreenModsButton.GetComponentInChildren<LocalizedTextField>().LocalizationID = "modsbutton";

            mainMenuModsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent(); // This is used to remove the persistent listeners that the options button has
            mainMenuModsButton.GetComponent<Button>().onClick.AddListener(openModsMenu); // Add open menu callback
            pauseScreenModsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent(); // This is used to remove the persistent listeners that the options button has
            pauseScreenModsButton.GetComponent<Button>().onClick.AddListener(openModsMenu); // Add open menu callback

            Transform image = Instantiate(GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform.GetChild(1), GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform);
            image.gameObject.SetActive(true);
            image.GetComponent<Image>().sprite = InternalAssetBundleReferences.ModBot.GetObject<Sprite>("modbot");
            image.GetComponent<RectTransform>().localScale = new Vector3(image.GetComponent<RectTransform>().localScale.x * 1.5f, image.GetComponent<RectTransform>().localScale.y * 0.375f, 1f);
            image.GetComponent<RectTransform>().position -= new Vector3(7f, 0f);

            Transform spawnedObject = Instantiate(GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform.GetChild(4), GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform);
            spawnedObject.gameObject.SetActive(true);
            spawnedObject.gameObject.AddComponent<LocalizedTextField>().LocalizationID = "mod_bot_credits_developers_list";
            spawnedObject.GetComponent<RectTransform>().position -= new Vector3(7f, -2f);

            Transform upperTitle = Instantiate(GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform.GetChild(3), GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform);
            upperTitle.gameObject.SetActive(true);
            upperTitle.GetComponent<LocalizedTextField>().LocalizationID = "mod_bot_credits_by";
            upperTitle.GetComponent<Text>().color = new Color32(255, 165, 0, 255);
            upperTitle.GetComponent<RectTransform>().position -= new Vector3(7f, -2f);

            GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform.GetChild(1).GetComponent<RectTransform>().position += new Vector3(7f, 0f);
            GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform.GetChild(3).GetComponent<RectTransform>().position += new Vector3(7f, 0f);
            GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform.GetChild(4).GetComponent<RectTransform>().position += new Vector3(7f, 0f);

            Transform settingsButtonHolder = TransformUtils.FindChildRecursive(GameUIRoot.Instance.SettingsMenu.RootContainer.transform, "TabHolder");

            int buttonCount = settingsButtonHolder.childCount;

            for (int i = 0; i < buttonCount; i++)
            {
                RectTransform button = settingsButtonHolder.GetChild(i).GetComponent<RectTransform>();

                float buttonSize = button.sizeDelta.x;

                float newSize = buttonSize * (buttonCount / (buttonCount + 1f));

                LayoutElement layoutElement = button.GetComponent<LayoutElement>();
                layoutElement.preferredWidth = newSize;

                button.sizeDelta = new Vector2(newSize, button.sizeDelta.y);
                button.anchoredPosition -= new Vector2(newSize * 0.2f * (i + 0), 0);
            }

            GameObject buttonContainerPrefab = settingsButtonHolder.GetChild(0).gameObject;
            RectTransform spawnedButtonContainer = Instantiate(buttonContainerPrefab, settingsButtonHolder).GetComponent<RectTransform>();
            spawnedButtonContainer.GetComponentInChildren<Text>().text = "Mod-Bot";

            SettingsMenuTabButton[] buttons = new SettingsMenuTabButton[GameUIRoot.Instance.SettingsMenu.TabButtons.Length + 1];
            for (int i = 0; i < GameUIRoot.Instance.SettingsMenu.TabButtons.Length; i++)
            {
                buttons[i] = GameUIRoot.Instance.SettingsMenu.TabButtons[i];
            }
            SettingsMenuTabButton tabButton = spawnedButtonContainer.GetComponentInChildren<SettingsMenuTabButton>();
            buttons[buttons.Length - 1] = tabButton;
            GameUIRoot.Instance.SettingsMenu.TabButtons = buttons;
            GameUIRoot.Instance.SettingsMenu.TabNavigationSetter.TabButtons = null;
            GameUIRoot.Instance.SettingsMenu.TabNavigationSetter.InitializeSetter();

            GameObject settingsPage = Instantiate(InternalAssetBundleReferences.ModBot.GetObject("ModBotSettings"), tabButton.ContentToShow.parent);
            tabButton.ContentToShow = settingsPage.transform;
            ModBotSettingsManager.Init(settingsPage.GetComponent<ModdedObject>());
        }

        private void openModsMenu()
        {
            ModBotUIRoot.Instance.ModList.Show();
        }
    }
}