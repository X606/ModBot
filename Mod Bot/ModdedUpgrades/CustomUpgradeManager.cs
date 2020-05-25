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
            _backButton = createButtonAt(InternalAssetBundleReferences.ModsWindow.GetObject("PreviousPageButton"), new Vector3(-300f, 50f, 0f), BackClicked);
			_nextButton = createButtonAt(InternalAssetBundleReferences.ModsWindow.GetObject("NextPageButton"), new Vector3(300f, 50f, 0f), NextClicked);

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

        static GameObject createButtonAt(GameObject prefab, Vector3 position, UnityAction call)
        {
            GameObject spawedButton = Instantiate(prefab);
            spawedButton.transform.SetParent(GameUIRoot.Instance.UpgradeUI.transform.GetChild(1), false);
            spawedButton.GetComponent<RectTransform>().localPosition = position;

			Button button = spawedButton.GetComponent<Button>();
            button.onClick.AddListener(call);

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