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

        private static ModInfo _theModUserGoingToDelete;

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

            ModBotUIRoot.Instance.ModsWindow.WindowObject.SetActive(false);

            mainMenuModsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent(); // This is used to remove the persistent listeners that the options button has
            mainMenuModsButton.GetComponent<Button>().onClick.AddListener(openModsMenu); // Add open menu callback
            pauseScreenModsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent(); // This is used to remove the persistent listeners that the options button has
            pauseScreenModsButton.GetComponent<Button>().onClick.AddListener(openModsMenu); // Add open menu callback

            ModBotUIRoot.Instance.ModsWindow.CloseButton.onClick.AddListener(closeModsMenu); // Add close menu button callback
            ModBotUIRoot.Instance.ModsWindow.GetMoreModsButton.onClick.AddListener(onGetMoreModsClicked); // Add more mods clicked callback
            ModBotUIRoot.Instance.ModsWindow.OpenModsFolderButton.onClick.AddListener(onModsFolderClicked); // Add mods folder clicked callback

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

            ReloadModItems();

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

            ModBotUIRoot.Instance.gameObject.AddComponent<ModBotUIRootNew>().Init();

            _theModUserGoingToDelete = null;
        }

        /// <summary>
        /// Opens mods panel. I actually made it for CDO mod
        /// </summary>
        /// <param name="onWindowClose"></param>
        public void OpenModsWindows(Action onWindowClose = null)
        {
            _actionOnModsPanelClose = onWindowClose;
            openModsMenu();
        }

        private void openModsMenu()
        {
            GameUIRoot.Instance.SetEscMenuDisabled(true);

            ModBotUIRoot.Instance.ModsWindow.WindowObject.SetActive(true);
            ModBotUIRoot.Instance.ModsWindow.CreateModButton.gameObject.SetActive(ModCreationWindow.CanBeShown);
            ReloadModItems();
        }

        private void closeModsMenu()
        {
            ModBotUIRoot.Instance.ModsWindow.WindowObject.SetActive(false);
            GameUIRoot.Instance.SetEscMenuDisabled(false);

            if (_actionOnModsPanelClose != null)
            {
                _actionOnModsPanelClose();
            }

            _actionOnModsPanelClose = null;
        }

        private void onGetMoreModsClicked()
        {
            ModBotUIRootNew.DownloadWindow.Show();
            return;
            ModBotUIRoot.Instance.ModDownloadPage.WindowObject.SetActive(true);

            ModBotUIRoot.Instance.ModDownloadPage.XButton.onClick = new Button.ButtonClickedEvent();
            ModBotUIRoot.Instance.ModDownloadPage.XButton.onClick.AddListener(delegate
            {
                ModBotUIRoot.Instance.ModDownloadPage.WindowObject.SetActive(false);
            });

            //ModBotUIRoot.Instance.ModDownloadPage.StartCoroutine(downloadModData(ModBotUIRoot.Instance.ModDownloadPage));
            ModBotUIRoot.Instance.ModDownloadPage.ErrorWindow.gameObject.SetActive(false);
            ModBotUIRoot.Instance.ModDownloadPage.LoadingPopup.gameObject.SetActive(true);
            TransformUtils.DestroyAllChildren(ModBotUIRoot.Instance.ModDownloadPage.Content.transform);
            ModsDownloadManager.DownloadModsData(onModInfosLoadingEnd, onModInfosLoadingError);
        }

        private static void onModInfosLoadingEnd(ModsHolder? loadedData)
        {
            ModBotUIRoot.Instance.ModDownloadPage.LoadingPopup.gameObject.SetActive(false);
            if (loadedData == null)
            {
                return;
            }
            GameObject modDownloadInfoPrefab = InternalAssetBundleReferences.ModBot.GetObject("ModDownloadInfo");
            foreach (ModInfo modInfo in loadedData.Value.Mods)
            {
                ModBotUIRoot.Instance.ModDownloadPage.StartCoroutine(downloadSpecialModDataAndAdd(ModBotUIRoot.Instance.ModDownloadPage.Content.gameObject, modInfo, modDownloadInfoPrefab));
            }
        }

        private static void onModInfosLoadingError(string error)
        {
            ModBotUIRoot.Instance.ModDownloadPage.ErrorText.text = error;
            ModBotUIRoot.Instance.ModDownloadPage.ErrorWindow.SetActive(true);
        }

        private void onModsFolderClicked()
        {
            Process.Start(ModsManager.Instance.ModFolderPath);
        }

        private static IEnumerator downloadSpecialModDataAndAdd(GameObject content, ModInfo modInfo, GameObject modDownloadInfoPrefab)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://modbot.org/api?operation=getSpecialModData&id=" + modInfo.UniqueID))
            {
                yield return webRequest.SendWebRequest(); // wait for the web request to send

                if (webRequest.isNetworkError || webRequest.isHttpError)
                    yield break;

                Dictionary<string, JToken> specialModData = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(webRequest.downloadHandler.text);

                if (!specialModData["Verified"].ToObject<bool>()) // do not want unchecked mods to come up in-game.
                    yield break;
            }

            GameObject holder = Instantiate(modDownloadInfoPrefab);
            holder.transform.SetParent(content.transform, false);
            holder.AddComponent<ModDownloadInfoItem>().Init(modInfo);
        }

        private void openModsOptionsWindowForMod(LoadedModInfo mod)
        {
            ModOptionsWindowBuilder builder = new ModOptionsWindowBuilder(ModBotUIRoot.Instance.ModsWindow.WindowObject, mod.ModReference);
            mod.ModReference.CreateSettingsWindow(builder);
        }

        private void toggleIsModDisabled(LoadedModInfo mod)
        {
            if (mod == null)
                return;
            mod.IsEnabled = !mod.IsEnabled;

            ReloadModItems();
        }

        private void addModToList(LoadedModInfo mod, GameObject parent)
        {
            bool isModActive = mod.IsEnabled;

            GameObject modItem = InternalAssetBundleReferences.ModBot.InstantiateObject("ModItemPrefab");
            modItem.transform.SetParent(parent.transform, false);

            string modName = mod.OwnerModInfo.DisplayName;

            _modItems.Add(modItem);

            ModdedObject modItemModdedObject = modItem.GetComponent<ModdedObject>();

            modItemModdedObject.GetObject<Text>(0).text = modName; // Set title
            modItemModdedObject.GetObject<Text>(1).text = mod.OwnerModInfo.Description; // Set description
            modItemModdedObject.GetObject<Text>(5).text = ModBotLocalizationManager.FormatLocalizedStringFromID("mods_menu_mod_id", mod.OwnerModInfo.UniqueID);

            modItemModdedObject.GetObject<RawImage>(2).texture = mod.OwnerModInfo.HasImage ? mod.OwnerModInfo.CachedImage : null;

            Button enableOrDisableButton = modItem.GetComponent<ModdedObject>().GetObject<Button>(3);

            if (!isModActive)
            {
                modItem.GetComponent<Image>().color = DisabledModColor;
                LocalizedTextField localizedTextField = enableOrDisableButton.transform.GetChild(0).GetComponent<LocalizedTextField>();
                localizedTextField.LocalizationID = "mods_menu_enable_mod";
                localizedTextField.tryLocalizeTextField();
                localizedTextField.gameObject.SetActive(!mod.OwnerModInfo.ModIsAboutToDelete());

                enableOrDisableButton.colors = new ColorBlock() { normalColor = Color.green * 1.2f, highlightedColor = Color.green, pressedColor = Color.green * 0.8f, colorMultiplier = 1 };
            }
            enableOrDisableButton.interactable = !mod.OwnerModInfo.ModIsAboutToDelete();

            Button BroadcastButton = modItemModdedObject.GetObject<Button>(6);
            BroadcastButton.onClick.AddListener(delegate { onBroadcastButtonClicked(mod.ModReference); });
            BroadcastButton.gameObject.SetActive(GameModeManager.IsMultiplayer());

            Button deleteModButton = modItemModdedObject.GetObject_Alt<Button>(7);
            deleteModButton.onClick.AddListener(delegate { deleteMod(mod); });
            deleteModButton.interactable = !mod.OwnerModInfo.ModIsAboutToDelete();

            modItemModdedObject.GetObject_Alt<Transform>(8).gameObject.SetActive(mod.OwnerModInfo.ModIsAboutToDelete());

            modItemModdedObject.GetObject<Button>(3).onClick.AddListener(delegate { toggleIsModDisabled(mod); }); // Add disable button callback
            //modItemModdedObject.GetObject<Button>(4).GetComponentInChildren<Text>().gameObject.AddComponent<LocalizedTextField>().LocalizationID = "mods_menu_mod_options";
            Button modsOptionButton = modItemModdedObject.GetObject<Button>(4);
            modsOptionButton.onClick.AddListener(delegate { openModsOptionsWindowForMod(mod); }); // Add Mod Options button callback
            modsOptionButton.interactable = mod.ModReference != null && mod.ModReference.ImplementsSettingsWindow() && isModActive;

        }

        private static void onBroadcastButtonClicked(Mod mod)
        {
            new Generic2ButtonDialogue(ModBotLocalizationManager.FormatLocalizedStringFromID("mods_menu_broadcast_confirm_message", mod.ModInfo.DisplayName),
            LocalizationManager.Instance.GetTranslatedString("mods_menu_broadcast_confirm_no"), null,
            LocalizationManager.Instance.GetTranslatedString("mods_menu_broadcast_confirm_yes"), delegate
            {
                ModSharingManager.SendModToAllModBotClients(mod.ModInfo.UniqueID);
            });
        }

        private static void deleteMod(LoadedModInfo mod)
        {
            _theModUserGoingToDelete = mod.OwnerModInfo;
            if (_theModUserGoingToDelete == null)
            {
                return;
            }
            new Generic2ButtonDialogue("Confirm mod deletion? - " + mod.OwnerModInfo.DisplayName, "Yes, delete", confirmDeletingMod, "Nevermind", null, Generic2ButtonDialogeUI.ModDeletionSizeDelta);
        }

        private static void confirmDeletingMod()
        {
            if (_theModUserGoingToDelete == null)
            {
                return;
            }

            _theModUserGoingToDelete.MarkModAboutToDelete();
            _theModUserGoingToDelete.IsModEnabled = false;
            ModsPanelManager.Instance.ReloadModItems();
        }

        private void Update()
        {
            if (!ModBotUIRootNew.DownloadWindow.gameObject.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    closeModsMenu();
                }
            }
            //if (ModsDownloadManager.IsLoadingModInfos())
            //{
            //    UnityWebRequest r = ModsDownloadManager.GetModInfosWebRequest();
            //    ModBotUIRoot.Instance.ModDownloadPage.ProgressBarSlider.value = r.downloadProgress;
            //}
        }


        /// <summary>
        /// Refereshes what mods should be displayed in the mods menu
        /// </summary>
        public void ReloadModItems()
        {
            _modItems.Clear();

            // Remove all mods from list
            foreach (Transform child in ModBotUIRoot.Instance.ModsWindow.Content.transform)
            {
                Destroy(child.gameObject);
            }

            List<LoadedModInfo> mods = ModsManager.Instance.GetAllMods();

            // Set the Content panel (ModdedObjectModsWindow.objects[0]) to appropriate height
            RectTransform size = ModBotUIRoot.Instance.ModsWindow.Content.GetComponent<RectTransform>();
            size.sizeDelta = new Vector2(size.sizeDelta.x, MOD_ITEM_HEIGHT * mods.Count);

            // Add all mods back to list
            foreach (LoadedModInfo info in mods)
            {
                addModToList(info, ModBotUIRoot.Instance.ModsWindow.Content);
            }
        }

        private readonly List<GameObject> _modItems = new List<GameObject>();
        private const int MOD_ITEM_HEIGHT = 100;
    }
}
