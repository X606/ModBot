using ModLibrary;
using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to check if there is a newer version available
    /// </summary>
    internal class UpdateChecker : MonoBehaviour
    {
        public const string BETA_POSTFIX = "-beta";

        private static bool s_hasChecked;

        private void Start()
        {
            if (s_hasChecked) return; // check only once per session
            StartCoroutine(checkVersion()); // Needs to be a Coroutine since the web requests are not asynchronous
        }

        private IEnumerator checkVersion()
        {
            string localVersionString = ModLibrary.Properties.Resources.ModBotVersion;
            if (localVersionString.EndsWith(BETA_POSTFIX))
                yield break;

            if (!Version.TryParse(localVersionString, out Version localVersion))
            {
                debug.Log("Could not parse the LOCAL version of Mod-Bot", Color.yellow);
                yield break;
            }

            using (UnityWebRequest modBotGitHubVersionRequest = UnityWebRequest.Get("https://api.github.com/repos/X606/ModBot/tags"))
            {
                modBotGitHubVersionRequest.timeout = 10;
                yield return modBotGitHubVersionRequest.SendWebRequest();
                s_hasChecked = true;

                if (modBotGitHubVersionRequest.result != UnityWebRequest.Result.Success)
                    yield break;

                TagData[] tags;
                try
                {
                    tags = JsonConvert.DeserializeObject<TagData[]>(modBotGitHubVersionRequest.downloadHandler.text);
                }
                catch (Exception exc)
                {
                    debug.Log("Could not deserialize tags", Color.yellow);
                    yield break;
                }

                for (int i = 0; i < tags.Length; i++)
                {
                    TagData tag = tags[i];
                    if (tag.IsBeta()) continue;

                    Version remoteVersion = tag.GetVersion();
                    if (remoteVersion > localVersion)
                    {
                        string message = ModBotLocalizationManager.FormatLocalizedStringFromID("newversion_message", remoteVersion.ToString(), localVersionString);
                        string dismissButtonText = LocalizationManager.Instance.GetTranslatedString("newversion_dismiss");
                        string installButtonText = LocalizationManager.Instance.GetTranslatedString("newversion_install");
                        Generic2ButtonDialogue generic = new Generic2ButtonDialogue(message, dismissButtonText, null, installButtonText, onInstallButtonClicked);
                        generic.SetColorOfFirstButton(Color.red);
                        generic.SetColorOfSecondButton(Color.green);
                        yield break;
                    }
                }

                string modBotUpToDateMessage = ModBotLocalizationManager.FormatLocalizedStringFromID("modbotuptodate", localVersionString);
                debug.Log(modBotUpToDateMessage, Color.green);
            }
        }

        private void onInstallButtonClicked()
        {
            Application.OpenURL("https://modbot.org/");
        }

        private struct TagData
        {
            public string name;

            public bool IsBeta() => name.EndsWith(BETA_POSTFIX);

            public Version GetVersion()
            {
                string versionString = name.Trim().Trim('v');
                if (versionString.EndsWith(BETA_POSTFIX))
                {
                    versionString = versionString.Substring(0, versionString.Length - BETA_POSTFIX.Length);
                }

                return new Version(versionString);
            }
        }
    }
}