using InternalModBot;
using ModLibrary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ModLibrary
{
    public class CustomUpgradeManager : Singleton<CustomUpgradeManager>
    {
        public void Start()
        {
            GameObject button = GameUIRoot.Instance.UpgradeUI.transform.GetChild(1).GetChild(6).gameObject;
            CreateButtonAt(button, new Vector3(-300, -200, 0), "Back", BackClicked);
            CreateButtonAt(button, new Vector3(300, -200, 0), "Next", NextClicked);
        }

        private void CreateButtonAt(GameObject prefab, Vector3 position, string text, UnityAction call)
        {
            GameObject spawedButton = Instantiate(prefab);
            spawedButton.transform.SetParent(GameUIRoot.Instance.UpgradeUI.transform.GetChild(1), false);
            spawedButton.GetComponent<RectTransform>().localPosition = position;

            Button button = spawedButton.transform.GetChild(0).GetChild(1).GetComponent<Button>();

            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(call);

            spawedButton.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = text;
        }
        
        public void BackClicked()
        {
            UpgradePagesManager.PreviusPage();
            OnBackOrNextClicked();
        }

        public void NextClicked()
        {
            UpgradePagesManager.NextPage();
            OnBackOrNextClicked();
        }

        private void OnBackOrNextClicked()
        {
            Accessor.CallPrivateMethod("PopulateIcons", GameUIRoot.Instance.UpgradeUI);

            Mod mod = UpgradePagesManager.TryGetModForPage(UpgradePagesManager.CurrentPage);

            if (mod != null)
            {
                GameUIRoot.Instance.UpgradeUI.TitleText.text = "Select upgrade\n[" + mod.GetModName() + "]";
                GameUIRoot.Instance.UpgradeUI.TitleText.resizeTextForBestFit = true;
            }
            else
            {
                GameUIRoot.Instance.UpgradeUI.TitleText.text = "Select upgrade";
                GameUIRoot.Instance.UpgradeUI.TitleText.resizeTextForBestFit = false;
            }
        }
    }
}