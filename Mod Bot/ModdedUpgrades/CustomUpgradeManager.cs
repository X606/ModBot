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
        private GameObject backButton;
        private GameObject nextButton;

        private void Start()
        {
            GameObject prefab = GameUIRoot.Instance.UpgradeUI.transform.GetChild(1).GetChild(6).gameObject;
            backButton = CreateButtonAt(prefab, new Vector3(-300, -200, 0), "Back", BackClicked);
            nextButton = CreateButtonAt(prefab, new Vector3(300, -200, 0), "Next", NextClicked);

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.UpgradeUIOpened, RefreshPageContents);
        }

        private void OnDestroy()
        {
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.UpgradeUIOpened, RefreshPageContents);

            if (backButton != null)
            {
                Destroy(backButton);
            }
            if (nextButton != null)
            {
                Destroy(nextButton);
            }
        }

        private void Update()
        {
            if (backButton == null || nextButton == null)
            {
                return;
            }

            bool state = GameModeManager.IsSinglePlayer();
            backButton.SetActive(state);
            nextButton.SetActive(state);
            UpgradeAngleSetter.Instance.UpdateSaveButtonState();
        }

        private GameObject CreateButtonAt(GameObject prefab, Vector3 position, string text, UnityAction call)
        {
            GameObject spawedButton = Instantiate(prefab);
            spawedButton.transform.SetParent(GameUIRoot.Instance.UpgradeUI.transform.GetChild(1), false);
            spawedButton.GetComponent<RectTransform>().localPosition = position;

            Button button = spawedButton.transform.GetChild(0).GetChild(1).GetComponent<Button>();

            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(call);

            spawedButton.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = text;

            return spawedButton;
        }
        
        /// <summary>
        /// Should be called when the back button is clicked on the UI.
        /// </summary>
        public void BackClicked()
        {
            UpgradePagesManager.PreviousPage();
            RefreshPageContents();
        }

        /// <summary>
        /// Should be called when the next button is clicked on the UI.
        /// </summary>
        public void NextClicked()
        {
            UpgradePagesManager.NextPage();
            RefreshPageContents();
        }

        private void RefreshPageContents()
        {
            Accessor.CallPrivateMethod("PopulateIcons", GameUIRoot.Instance.UpgradeUI);

            Mod mod = UpgradePagesManager.TryGetModForPage(UpgradePagesManager.CurrentPage);

            if (mod != null)
            {
                GameUIRoot.Instance.UpgradeUI.TitleText.text = "Select upgrade\n[" + mod.GetModName() + "]";
                GameUIRoot.Instance.UpgradeUI.TitleText.resizeTextForBestFit = true;

                UpgradeAngleSetter.Instance.RefreshIconEventTriggers();
            }
            else
            {
                GameUIRoot.Instance.UpgradeUI.TitleText.text = "Select upgrade";
                GameUIRoot.Instance.UpgradeUI.TitleText.resizeTextForBestFit = false;
            }
        }
    }
}