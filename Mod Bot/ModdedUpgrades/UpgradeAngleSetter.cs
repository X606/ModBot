using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ModLibrary;

namespace InternalModBot
{
    /// <summary>
    /// Allows for users to set upgrade angles manually while in the game
    /// </summary>
    public class UpgradeAngleSetter : Singleton<UpgradeAngleSetter>
    {
        private Dictionary<ModdedUpgradeRepresenter, float> changedIconAngles;

        internal bool DebugModeEnabled;

        private GameObject saveButtonObject = null;

        private void Start()
        {
            DebugModeEnabled = false;

            changedIconAngles = new Dictionary<ModdedUpgradeRepresenter, float>();

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.UpgradeUIOpened, delegate { RefreshIconEventTriggers(false); });
        }

        private void OnDestroy()
        {
            changedIconAngles.Clear();

            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.UpgradeUIOpened, delegate { RefreshIconEventTriggers(false); });

            if (saveButtonObject != null)
            {
                Destroy(saveButtonObject);
            }
        }

        private void CreateSaveButton()
        {
            GameObject buttonPrefab = GameUIRoot.Instance.UpgradeUI.transform.GetChild(1).GetChild(6).gameObject;

            saveButtonObject = Instantiate(buttonPrefab);
            saveButtonObject.transform.SetParent(GameUIRoot.Instance.UpgradeUI.transform.GetChild(1), false);
            saveButtonObject.GetComponent<RectTransform>().localPosition -= new Vector3(0f, 40f, 0f);

            Button saveButton = saveButtonObject.GetComponentInChildren<Button>();

            saveButton.onClick = new Button.ButtonClickedEvent();
            saveButton.onClick.AddListener(SaveAngleChangesToFile);

            saveButton.GetComponentInChildren<Text>().text = "Generate";
        }

        private void SaveAngleChangesToFile()
        {
            string fileName = "UpgradeAnglesCode.txt";
            string fullFilePath = Path.Combine(Application.persistentDataPath, fileName);

            List<string> lines = new List<string>();
            foreach (KeyValuePair<ModdedUpgradeRepresenter, float> upgradeAngle in changedIconAngles)
            {
                string item = "UpgradeManager.Instance.SetUpgradeAngle({0}, {1}, {2}, this); // UpgradeName: {3}, UpgradeType: {0}, Level: {1}"; // {0}: UpgradeType, {1}: Level, {2}: Angle, {3}: UpgradeName

                string upgradeType = ConvertUpgradeTypeToString(upgradeAngle.Key.UpgradeType);
                string level = upgradeAngle.Key.Level.ToString();
                string angle = upgradeAngle.Value.ToString();
                string upgradeName = GetUpgradeName(upgradeAngle.Key);

                string formatted = string.Format(item, upgradeType, level, angle, upgradeName);

                lines.Add(formatted);
            }

            File.WriteAllLines(fullFilePath, lines);
            Process.Start("notepad.exe", fullFilePath);
        }

        private string GetUpgradeName(ModdedUpgradeRepresenter upgrade)
        {
            if (upgrade == null)
            {
                return string.Empty;
            }

            UpgradeDescription upgradeDescription = UpgradeManager.Instance.GetUpgrade(upgrade.UpgradeType, upgrade.Level);
            if (upgradeDescription == null)
            {
                return string.Empty;
            }

            return upgradeDescription.UpgradeName;
        }

        private string ConvertUpgradeTypeToString(UpgradeType upgradeType)
        {
            string prefix;

            if (upgradeType.IsModdedUpgradeType())
            {
                prefix = "(UpgradeType)";
            }
            else
            {
                prefix = "UpgradeType.";
            }

            return prefix + upgradeType.ToString();
        }

        internal void RefreshIconEventTriggers(bool saveButtonState = true)
        {
            if (DebugModeEnabled && saveButtonObject == null)
            {
                CreateSaveButton();
            }

            if (saveButtonObject != null)
            {
                saveButtonObject.SetActive(saveButtonState);
            }

            if (!DebugModeEnabled || UpgradePagesManager.CurrentPage == 0)
            {
                return;
            }

            List<UpgradeUIIcon> icons = Accessor.GetPrivateField<UpgradeUI, List<UpgradeUIIcon>>("_icons", GameUIRoot.Instance.UpgradeUI);

            foreach (UpgradeUIIcon icon in icons)
            {
                EventTrigger eventTrigger = icon.GetComponent<EventTrigger>();

                eventTrigger.triggers.RemoveAll(item => item.eventID == EventTriggerType.PointerClick);

                EventTrigger.Entry scrollCallback = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.Scroll,
                    callback = new EventTrigger.TriggerEvent()
                };
                scrollCallback.callback.AddListener(delegate (BaseEventData eventData) { UpdateIcon(icon, eventData); });

                eventTrigger.triggers.Add(scrollCallback);
            }
        }

        private float GetAngleForIconAtCurrentPage(UpgradeUIIcon icon)
        {
            UpgradeDescription upgradeDescription = icon.GetDescription();
            return UpgradePagesManager.GetAngleOfUpgrade(upgradeDescription.UpgradeType, upgradeDescription.Level);
        }

        private void SetAngleOfUpgrade(UpgradeUIIcon icon, float newAngle)
        {
            UpgradeDescription upgradeDescription = icon.GetDescription();
            if (upgradeDescription == null)
            {
                return;
            }

            upgradeDescription.SetAngleOffset(newAngle, UpgradePagesManager.TryGetModForPage(UpgradePagesManager.CurrentPage));
        }

        private void UpdateIcon(UpgradeUIIcon icon, BaseEventData eventData)
        {
            float scrollDelta = eventData.currentInputModule.input.mouseScrollDelta.y;
            float newAngle = GetAngleForIconAtCurrentPage(icon) + scrollDelta;

            SetAngleOfUpgrade(icon, newAngle);

            UpgradeDescription upgradeDescription = icon.GetDescription();
            ModdedUpgradeRepresenter upgrade = new ModdedUpgradeRepresenter(upgradeDescription.UpgradeType, upgradeDescription.Level);

            changedIconAngles[upgrade] = newAngle;

            Accessor.CallPrivateMethod("PopulateIcons", GameUIRoot.Instance.UpgradeUI);
            RefreshIconEventTriggers();
        }
    }
}
