using InternalModBot;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ModLibrary
{
    /// <summary>
    /// Used by Mod-Bot to handle the custom upgrade pages. (Does things like handle the next and back buttons)
    /// </summary>
    public class CustomUpgradesUIManager : Singleton<CustomUpgradesUIManager>
    {
        GameObject _backButton;
        GameObject _nextButton;

        void Start()
        {
            _backButton = createButtonAt(InternalAssetBundleReferences.ModBot.GetObject("PreviousPageButton"), new Vector3(-300f, 50f, 0f), BackClicked);
            _nextButton = createButtonAt(InternalAssetBundleReferences.ModBot.GetObject("NextPageButton"), new Vector3(300f, 50f, 0f), NextClicked);

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.UpgradeUIOpened, refreshPageContents);
        }

        void OnDestroy()
        {
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.UpgradeUIOpened, refreshPageContents);
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
            UpgradeUI upgradeUI = GameUIRoot.Instance.UpgradeUI;
            upgradeUI.PopulateIcons();

            Text titleText = upgradeUI.TitleText;
            titleText.GetComponent<LocalizedTextField>().tryLocalizeTextField(); // Re-localize "Select Upgrade" text field
            titleText.resizeTextForBestFit = true;

            if (UpgradePagesManager.IsCurrentlyShowingModdedUpgrades)
            {
                string modID = UpgradePagesManager.GetModIDForCurrentPage();
                if (modID != null)
                {
                    LoadedModInfo modInfo = ModsManager.Instance.GetLoadedModWithID(modID);
                    if (modInfo != null)
                    {
                        string text;
                        int totalPagesForMod = UpgradePagesManager.GetNumPagesAddedByMod(modID);
                        if (totalPagesForMod > 1)
                        {
                            text = $"\n[{modInfo.OwnerModInfo.DisplayName}] [{UpgradePagesManager.GetIndexOfCurrentPage() + 1}/{totalPagesForMod}]";
                        }
                        else
                        {
                            text = $"\n[{modInfo.OwnerModInfo.DisplayName}]";
                        }

                        titleText.text += text;
                        UpgradeAngleSetter.Instance.RefreshIconEventTriggers();
                    }
                }
            }
        }
    }
}