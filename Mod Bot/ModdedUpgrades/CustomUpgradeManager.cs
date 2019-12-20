using InternalModBot;
using ModLibrary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ModLibrary
{
    /// <summary>
    /// Used by Mod-Bot to handle the custom upgrade pages. (Does things like handle the next and back buttons)
    /// </summary>
    public class CustomUpgradeManager : Singleton<CustomUpgradeManager>
    {
        GameObject _backButton;
        GameObject _nextButton;

        void Start()
        {
            GameObject prefab = GameUIRoot.Instance.UpgradeUI.transform.GetChild(1).GetChild(6).gameObject;
            _backButton = createButtonAt(prefab, new Vector3(-300f, -200f, 0f), "upgrade_screen_back", BackClicked);
            _nextButton = createButtonAt(prefab, new Vector3(300f, -200f, 0f), "upgrade_screen_next", NextClicked);

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.UpgradeUIOpened, refreshPageContents);
        }

        void OnDestroy()
        {
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.UpgradeUIOpened, refreshPageContents);

            if (_backButton != null)
                Destroy(_backButton);

            if (_nextButton != null)
                Destroy(_nextButton);
        }

        void Update()
        {
            if (_backButton == null || _nextButton == null)
                return;

            bool isSinglePlayer = GameModeManager.IsSinglePlayer();
            _backButton.SetActive(isSinglePlayer);
            _nextButton.SetActive(isSinglePlayer);

            UpgradeAngleSetter.Instance.UpdateSaveButtonState();
        }

        static GameObject createButtonAt(GameObject prefab, Vector3 position, string localizationID, UnityAction call)
        {
            GameObject spawedButton = Instantiate(prefab);
            spawedButton.transform.SetParent(GameUIRoot.Instance.UpgradeUI.transform.GetChild(1), false);
            spawedButton.GetComponent<RectTransform>().localPosition = position;

            Button button = spawedButton.transform.GetChild(0).GetChild(1).GetComponent<Button>();

            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(call);

            spawedButton.GetComponentInChildren<LocalizedTextField>().LocalizationID = localizationID;

            return spawedButton;
        }
        
        /// <summary>
        /// Should be called when the back button is clicked on the UI.
        /// </summary>
        public static void BackClicked()
        {
            UpgradePagesManager.PreviousPage();
            refreshPageContents();
        }

        /// <summary>
        /// Should be called when the next button is clicked on the UI.
        /// </summary>
        public static void NextClicked()
        {
            UpgradePagesManager.NextPage();
            refreshPageContents();
        }

        static void refreshPageContents()
        {
            Accessor.CallPrivateMethod("PopulateIcons", GameUIRoot.Instance.UpgradeUI);

            Accessor.CallPrivateMethod("tryLocalizeTextField", GameUIRoot.Instance.UpgradeUI.TitleText.GetComponent<LocalizedTextField>()); // Re-localize "Select Upgrade" text field
            GameUIRoot.Instance.UpgradeUI.TitleText.resizeTextForBestFit = false;

            Mod mod = UpgradePagesManager.TryGetModForPage(UpgradePagesManager.CurrentPage);
            if (mod != null)
            {
                GameUIRoot.Instance.UpgradeUI.TitleText.text += "\n[" + mod.GetModName() + "]";
                GameUIRoot.Instance.UpgradeUI.TitleText.resizeTextForBestFit = true;

                UpgradeAngleSetter.Instance.RefreshIconEventTriggers();
            }
        }
    }
}