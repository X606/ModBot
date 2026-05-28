using ModBotWebsiteAPI;
using ModLibrary;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Handles settings on mod-bot page of the settings
    /// </summary>
    internal static class ModBotSettingsManager
    {
        static ModdedObject _settingsPageModdedObject;

        /// <summary>
        /// Sets up the <see cref="ModBotSettingsManager"/>
        /// </summary>
        /// <param name="moddedObject"></param>
        public static void Init(ModdedObject moddedObject)
        {
            _settingsPageModdedObject = moddedObject;
        }

        public static ModdedObject GetSettingsPageModdedObject() => _settingsPageModdedObject;

        public class ModBotSettingsBuilder
        {
            Transform _holder;

            GameObject _modBotOptionsLabelPrefab;
            GameObject _modBotOptionsLabelAndButton;
            GameObject _modBotOptionsSingleButton;
            GameObject _modBotOptionsCheckbox;

            public ModBotSettingsBuilder(ModdedObject moddedObject)
            {
                _holder = moddedObject.GetObject<GameObject>(0).transform;

                TransformUtils.DestroyAllChildren(_holder);

                _modBotOptionsLabelPrefab = InternalAssetBundleReferences.ModBot.GetObject("ModBotOptionsLabel");
                _modBotOptionsLabelAndButton = InternalAssetBundleReferences.ModBot.GetObject("ModBotOptionsLabelAndButton");
                _modBotOptionsSingleButton = InternalAssetBundleReferences.ModBot.GetObject("ModBotOptionsSingleButton");
                _modBotOptionsCheckbox = InternalAssetBundleReferences.ModBot.GetObject("ModBotOptionsCheckbox");
            }

            public Text AddLabel(string label)
            {
                ModdedObject moddedObject = GameObject.Instantiate(_modBotOptionsLabelPrefab, _holder).GetComponent<ModdedObject>();
                Text text = moddedObject.GetObject<Text>(0);
                text.text = label;
                return text;
            }

            public void AddLabelAndButton(string label, string buttonText, Color buttonColor, Action<Button> onClick)
            {
                ModdedObject moddedObject = GameObject.Instantiate(_modBotOptionsLabelAndButton, _holder).GetComponent<ModdedObject>();
                moddedObject.GetObject<Text>(0).text = label;
                Button button = moddedObject.GetObject<Button>(1);
                button.GetComponentInChildren<Text>().text = buttonText;
                button.GetComponent<Image>().color = buttonColor;
                button.onClick.AddListener(delegate { onClick?.Invoke(button); });
            }

            public void AddSingleButton(string buttonText, Color buttonColor, Action<Button> onClick)
            {
                ModdedObject moddedObject = GameObject.Instantiate(_modBotOptionsSingleButton, _holder).GetComponent<ModdedObject>();
                Button button = moddedObject.GetObject<Button>(0);
                button.GetComponentInChildren<Text>().text = buttonText;
                button.GetComponent<Image>().color = buttonColor;
                button.onClick.AddListener(delegate { onClick?.Invoke(button); });
            }

            public void AddCheckbox(string text, bool isOn, Action<Toggle> onClick)
            {
                ModdedObject moddedObject = GameObject.Instantiate(_modBotOptionsCheckbox, _holder).GetComponent<ModdedObject>();
                Toggle toggle = moddedObject.GetObject<Toggle>(0);
                toggle.isOn = isOn;
                toggle.GetComponentInChildren<Text>().text = text;
                toggle.onValueChanged.AddListener(delegate { onClick?.Invoke(toggle); });
            }
        }

        /// <summary>
        /// Populates the settings widow using the builder
        /// </summary>
        /// <param name="builder"></param>
        internal static void CreateSettingsWindow(ModBotSettingsBuilder builder)
        {
            builder.AddLabel("Interface");
            builder.AddCheckbox("Show max FPS", ModBotPrefs.ShowMaxFPS, delegate (Toggle toggle)
            {
                ModBotPrefs.ShowMaxFPS = toggle.isOn;
                ModBotUIRoot.Instance.FPSCounter.ForceRefreshNextFrame();
            });
            builder.AddCheckbox("Hide custom player tags", ModBotPrefs.HideCustomTags, delegate (Toggle toggle)
            {
                ModBotPrefs.HideCustomTags = toggle.isOn;
                MultiplayerPlayerNameManager.Instance.TriggerRefreshNameTagsEvent();

                MultiplayerPlayerList playerList = GameUIRoot.Instance.MultiplayerPlayerList;
                if (playerList.isActiveAndEnabled) playerList.refreshLabels();
            });

            builder.AddLabel("Controls");
            foreach (ModBotPrefs.InputOption inputOption in ModBotPrefs.InputOptions)
            {
                builder.AddLabelAndButton(inputOption.DisplayName, inputOption.Key.ToString(), Color.lightGray, delegate (Button button)
                {
                    StaticCoroutineRunner.StartStaticCoroutine(assignKeyFromNextInput(button, inputOption, 3f));
                });
            }

            builder.AddLabel("Website Integration");
            if (API.HasSession)
            {
                if (ModBotUserIdentifier.SignInStatus != ModBotUserIdentifier.SignInStatuses.Success)
                {
                    if (ModBotUserIdentifier.SignInStatus == ModBotUserIdentifier.SignInStatuses.Failed)
                    {
                        Text erorText = builder.AddLabel($"Authorization failed. Check your internet connection");
                        erorText.alignment = TextAnchor.MiddleCenter;
                        erorText.color = Color.softRed;
                        (erorText.transform.parent as RectTransform).sizeDelta = new Vector2(200f, 40f);
                        return;
                    }

                    Text text1 = builder.AddLabel($"Signing in...");
                    text1.alignment = TextAnchor.MiddleCenter;
                    text1.color = Color.gray;
                    (text1.transform.parent as RectTransform).sizeDelta = new Vector2(300f, 20f);
                    return;
                }

                Text text = builder.AddLabel($"Signed in as: {ModBotUserIdentifier.UserNameColored}");
                text.alignment = TextAnchor.MiddleCenter;
                text.color = Color.gray;
                (text.transform.parent as RectTransform).sizeDelta = new Vector2(300f, 20f);

                builder.AddSingleButton("View profile", Color.dodgerBlue, delegate
                {
                    Process.Start($"https://modbot.org/userPage.html?userID={ModBotUserIdentifier.UserID}");
                });
                builder.AddSingleButton("Edit tags", Color.dodgerBlue, delegate
                {
                    Process.Start("https://modbot.org/tagBrowsing.html");
                });
                builder.AddSingleButton("Sign out", Color.red, delegate (Button button)
                {
                    button.interactable = false;
                    debug.Log("Logging out...");
                    API.SignOut(delegate (JsonObject json)
                    {
                        ModBotUserIdentifier.Instance.SignOut();
                        CreateSettingsWindow(new ModBotSettingsBuilder(_settingsPageModdedObject));
                    });
                });
            }
            else
            {
                if (!ModBotUserIdentifier.Instance.CanSignIn())
                {
                    Text text = builder.AddLabel("You are offline. Check your internet connection");
                    text.alignment = TextAnchor.LowerCenter;
                    text.color = Color.softRed;
                    (text.transform.parent as RectTransform).sizeDelta = new Vector2(200f, 40f);
                    return;
                }

                builder.AddSingleButton("Sign in", Color.green, delegate
                {
                    GameUIRoot.Instance.SettingsMenu.Hide();
                    ModBotUIRoot.Instance.ModBotSignInUI.OpenSignInForm();
                });
            }

        }

        static IEnumerator assignKeyFromNextInput(Button button, ModBotPrefs.InputOption input, float timeoutTime)
        {
            yield return new WaitForSecondsRealtime(0.1f); // wait a little so we dont pick up the mouse button

            Text buttonText = button.GetComponentInChildren<Text>();
            buttonText.text = "INPUT NEW KEY";

            float passedTime = 0;
            KeyCode[] allKeys = (KeyCode[])Enum.GetValues(typeof(KeyCode));
            KeyCode? foundKey = null;
            while (foundKey == null && passedTime < timeoutTime)
            {
                for (int i = 0; i < allKeys.Length; i++)
                {
                    if (Input.GetKeyDown(allKeys[i]))
                    {
                        foundKey = allKeys[i];
                        break;
                    }
                }
                passedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            if (foundKey == null) // did we time out?
            {
                buttonText.text = input.Key.ToString();
                yield break;
            }

            input.Key = foundKey.Value;

            buttonText.text = input.Key.ToString();
        }
    }
}