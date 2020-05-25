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
        Dictionary<ModdedUpgradeRepresenter, float> _changedIconAngles;

        GameObject _saveButtonObject = null;

        internal bool DebugModeEnabled = false;

        void Start()
        {
            _changedIconAngles = new Dictionary<ModdedUpgradeRepresenter, float>();

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.UpgradeUIOpened, RefreshIconEventTriggers);
        }

        void OnDestroy()
        {
            _changedIconAngles.Clear();

            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.UpgradeUIOpened, RefreshIconEventTriggers);

            if (_saveButtonObject != null)
                Destroy(_saveButtonObject);
        }

        void createSaveButton()
        {
            _saveButtonObject = InternalAssetBundleReferences.ModsWindow.InstantiateObject("GenerateButton");
            _saveButtonObject.transform.SetParent(GameUIRoot.Instance.UpgradeUI.transform.GetChild(1), false);
			_saveButtonObject.GetComponent<RectTransform>().localPosition = new Vector3(300f, -25f, 0f);


			Button saveButton = _saveButtonObject.GetComponentInChildren<Button>();
            saveButton.onClick.AddListener(saveAngleChangesToFile);

            saveButton.GetComponentInChildren<LocalizedTextField>().LocalizationID = "upgrade_screen_generate";
        }

        void saveAngleChangesToFile()
        {
            string fileName = "UpgradeAnglesCode.txt";
            string fullFilePath = Path.Combine(Application.persistentDataPath, fileName);

            List<string> lines = new List<string>();
            foreach (KeyValuePair<ModdedUpgradeRepresenter, float> upgradeAngle in _changedIconAngles)
            {
                string item = "UpgradeManager.Instance.SetUpgradeAngle({0}, {1}, {2}f, this); // UpgradeName: {3}, UpgradeType: {0}, Level: {1}"; // {0}: UpgradeType, {1}: Level, {2}: Angle, {3}: UpgradeName

                string upgradeType = convertUpgradeTypeToString(upgradeAngle.Key.UpgradeType);
                string level = upgradeAngle.Key.Level.ToString();
                string angle = upgradeAngle.Value.ToString();
                string upgradeName = getUpgradeName(upgradeAngle.Key);

                string formatted = string.Format(item, upgradeType, level, angle, upgradeName);

                lines.Add(formatted);
            }

            File.WriteAllLines(fullFilePath, lines);
            Process.Start("notepad.exe", fullFilePath);
        }

        static string getUpgradeName(ModdedUpgradeRepresenter upgrade)
        {
            if (upgrade == null)
                return string.Empty;

            UpgradeDescription upgradeDescription = UpgradeManager.Instance.GetUpgrade(upgrade.UpgradeType, upgrade.Level);
            if (upgradeDescription == null)
                return string.Empty;

            return upgradeDescription.UpgradeName;
        }

        static string convertUpgradeTypeToString(UpgradeType upgradeType)
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

        static float getAngleForIconAtCurrentPage(UpgradeUIIcon icon)
        {
            UpgradeDescription upgradeDescription = icon.GetDescription();
            return UpgradePagesManager.GetAngleOfUpgrade(upgradeDescription.UpgradeType, upgradeDescription.Level);
        }

        static void setAngleOfUpgrade(UpgradeUIIcon icon, float newAngle)
        {
            UpgradeDescription upgradeDescription = icon.GetDescription();
            if (upgradeDescription == null)
                return;

            upgradeDescription.SetAngleOffset(newAngle, UpgradePagesManager.TryGetModForPage(UpgradePagesManager.CurrentPage));
        }

        void updateIcon(UpgradeUIIcon icon, BaseEventData eventData)
        {
            float scrollDelta = eventData.currentInputModule.input.mouseScrollDelta.y;
            float newAngle = getAngleForIconAtCurrentPage(icon) + scrollDelta;

            setAngleOfUpgrade(icon, newAngle);

            UpgradeDescription upgradeDescription = icon.GetDescription();
            ModdedUpgradeRepresenter upgrade = new ModdedUpgradeRepresenter(upgradeDescription.UpgradeType, upgradeDescription.Level);

            _changedIconAngles[upgrade] = newAngle;

            Accessor.CallPrivateMethod("PopulateIcons", GameUIRoot.Instance.UpgradeUI);
            RefreshIconEventTriggers();
        }

        bool canCurrentlyEditIconAngles()
        {
            return DebugModeEnabled && UpgradePagesManager.CurrentPage != 0 && GameModeManager.IsSinglePlayer();
        }

        internal void UpdateSaveButtonState()
        {
            if (_saveButtonObject == null)
                return;

            _saveButtonObject.SetActive(canCurrentlyEditIconAngles());
        }

        internal void RefreshIconEventTriggers()
        {
            if (!canCurrentlyEditIconAngles())
            {
                if (_saveButtonObject != null)
                    _saveButtonObject.SetActive(false);

                return;
            }
            else if (_saveButtonObject == null)
            {
                createSaveButton();
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
                scrollCallback.callback.AddListener(delegate (BaseEventData eventData) { updateIcon(icon, eventData); });

                eventTrigger.triggers.Add(scrollCallback);
            }
        }

    }
}
