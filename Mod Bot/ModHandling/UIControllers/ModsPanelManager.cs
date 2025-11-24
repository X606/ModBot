using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to control most of the UI in Mod-Bot, this has control over the mod buttons and mods window. Note that all functions and fields on this class are private since they more or less work on their own.
    /// </summary>
    public class ModsPanelManager : Singleton<ModsPanelManager>
    {
        private static readonly string[] _modBotDevs = new string[]
        {
            "X606",
            "Gorakh",
            "Niek_Alexander",
            "nyxkrage", // HSCarsten
            "Aresiel",
            "fyrkantis",
            "Nikioto4242",
            "A TVCat"
        };

        private void Start()
        {
            patchTitleScreen();
            patchPauseMenu();
            patchSettingsMenu();
            patchCreditsScreen();
        }

        private void patchTitleScreen()
        {
            GameObject titleScreenContainer = TransformUtils.FindChildRecursive(GameUIRoot.Instance.TitleScreenUI.RootButtonsContainer, "BottomButtons").gameObject; // Gets the lower buttons container

            // Copy the options button to make into the Mods button
            GameObject modsButtonPrefab = TransformUtils.FindChildRecursive(titleScreenContainer.transform, "OptionsButton").gameObject; // Gets the options button (we copy it and replace its organs and face)
            GameObject mainMenuModsButton = Instantiate(modsButtonPrefab, titleScreenContainer.transform);

            mainMenuModsButton.GetComponentInChildren<LocalizedTextField>().LocalizationID = "modsbutton"; // Set LocalizationID
            mainMenuModsButton.transform.SetSiblingIndex(1);

            mainMenuModsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent(); // This is used to remove the persistent listeners that the options button has
            mainMenuModsButton.GetComponent<Button>().onClick.AddListener(openModsMenu); // Add open menu callback
        }

        private void patchPauseMenu()
        {
            EscMenu escMenu = GameUIRoot.Instance.EscMenu;

            RectTransform pauseMenuBgTransform = (RectTransform)escMenu.ButtonsCanvasGroup.transform;
            pauseMenuBgTransform.anchoredPosition = new Vector2(0f, -15f);
            pauseMenuBgTransform.sizeDelta = new Vector2(200f, 240f);
            RectTransform backToLvlEditorButtonTransform = (RectTransform)escMenu.BackToLevelEditorButton.transform;
            backToLvlEditorButtonTransform.anchoredPosition = new Vector2(0f, -250f);

            Vector3 pauseScreenButtonOffset = new Vector3(0f, 1.2f, 0f);
            GameObject pauseScreenModsButton = Instantiate(GameUIRoot.Instance.EscMenu.SettingsButton.transform.gameObject, GameUIRoot.Instance.EscMenu.SettingsButton.transform.parent); // All of these lines edit the buttons on the pause menu
            escMenu.ReturnToGameButton.transform.position += pauseScreenButtonOffset;
            escMenu.SettingsButton.transform.position += pauseScreenButtonOffset;
            escMenu.AchievementsButton.transform.position -= pauseScreenButtonOffset;
            escMenu.ExitButton.transform.position -= pauseScreenButtonOffset;
            escMenu.ExitConfirmUI.transform.position -= pauseScreenButtonOffset;
            escMenu.MainMenuButton.transform.position -= pauseScreenButtonOffset;
            escMenu.MainMenuConfirmUI.transform.position -= pauseScreenButtonOffset;

            pauseScreenModsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent(); // This is used to remove the persistent listeners that the options button has
            pauseScreenModsButton.GetComponent<Button>().onClick.AddListener(openModsMenu); // Add open menu callback

            pauseScreenModsButton.transform.position -= pauseScreenButtonOffset;
            pauseScreenModsButton.GetComponentInChildren<LocalizedTextField>().LocalizationID = "modsbutton";
        }

        private void patchSettingsMenu()
        {
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

        private void patchOldCreditsMenu()
        {
            Transform image = Instantiate(GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform.GetChild(1), GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform);
            image.gameObject.SetActive(true);
            image.GetComponent<Image>().sprite = InternalAssetBundleReferences.ModBot.GetObject<Sprite>("modbot");
            image.transform.localScale = new Vector3(image.transform.localScale.x * 1.5f, image.transform.localScale.y * 0.375f, 1f);
            image.transform.position -= new Vector3(7f, 0f);

            Transform spawnedObject = Instantiate(GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform.GetChild(4), GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform);
            spawnedObject.gameObject.SetActive(true);
            spawnedObject.gameObject.AddComponent<LocalizedTextField>().LocalizationID = "mod_bot_credits_developers_list";
            spawnedObject.transform.position -= new Vector3(7f, -2f);

            Transform upperTitle = Instantiate(GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform.GetChild(3), GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform);
            upperTitle.gameObject.SetActive(true);
            upperTitle.GetComponent<LocalizedTextField>().LocalizationID = "mod_bot_credits_by";
            upperTitle.GetComponent<Text>().color = new Color32(255, 165, 0, 255);
            upperTitle.transform.position -= new Vector3(7f, -2f);

            GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform.GetChild(1).transform.position += new Vector3(7f, 0f);
            GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform.GetChild(3).transform.position += new Vector3(7f, 0f);
            GameUIRoot.Instance.TitleScreenUI.CreditsUI.transform.GetChild(4).transform.position += new Vector3(7f, 0f);
        }

        private void patchCreditsScreen()
        {
            CreditsCrawlAnimation creditsCrawlAnimation = GameUIRoot.Instance.CreditsCrawl;
            Transform scrollView = creditsCrawlAnimation.ScrollView;

            Transform spacer = TransformUtils.FindChildRecursive(scrollView, "spacer");
            Transform multiplePersonsLabel = TransformUtils.FindChildRecursive(scrollView, "Image");
            Transform kouHeader = TransformUtils.FindChildRecursive(scrollView, "kou_header");
            Transform container = spacer.parent;

            int siblingIndex = kouHeader.GetSiblingIndex() - 1;

            RectTransform newSpacer = (RectTransform)Instantiate(spacer, container);
            newSpacer.SetSiblingIndex(siblingIndex);

            RectTransform modBotLogo = (RectTransform)Instantiate(kouHeader, container);
            modBotLogo.SetSiblingIndex(siblingIndex+1);
            ((RectTransform)modBotLogo.GetChild(0)).sizeDelta = new Vector2(435f, 85f);
            Image image = modBotLogo.GetChild(0).GetComponent<Image>();
            image.sprite = InternalAssetBundleReferences.ModBot.GetObject<Sprite>("modbot");
            image.color = Color.white;

            RectTransform modbotCreatorsLabel = (RectTransform)Instantiate(multiplePersonsLabel, container);
            modbotCreatorsLabel.SetSiblingIndex(siblingIndex+2);
            Text text = modbotCreatorsLabel.GetChild(0).GetComponent<Text>();
            StringBuilder stringBuilder = new StringBuilder();
            for(int i = 0; i < _modBotDevs.Length; i++)
            {
                stringBuilder.AppendLine(_modBotDevs[i]);
                stringBuilder.AppendLine();
            }
            text.text = stringBuilder.ToString();
            modbotCreatorsLabel.sizeDelta = new Vector2(800f, 430f);
            Destroy(modbotCreatorsLabel.GetComponent<LayoutElement>());

            float addY = modbotCreatorsLabel.sizeDelta.y + modBotLogo.sizeDelta.y + newSpacer.sizeDelta.y;
            creditsCrawlAnimation.StopScrollingAtY += addY;
            creditsCrawlAnimation.ShowExitAtY += addY;
        }

        private void openModsMenu()
        {
            ModBotUIRoot.Instance.ModList.Show();
        }
    }
}