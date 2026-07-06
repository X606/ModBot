using ModLibrary;
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
        private static bool s_hasChecked;

        private void Start()
        {
            if (s_hasChecked) return; // check only once per session
            StartCoroutine(checkVersion()); // Needs to be a Coroutine since the web requests are not asynchronous
        }

        private IEnumerator checkVersion()
        {
            string localVersionString = ModLibrary.Properties.Resources.ModBotVersion;
            if (localVersionString.ToLower().Contains("beta"))
                yield break;

            if (!Version.TryParse(localVersionString, out Version localVersion))
            {
                debug.Log("Could not parse the LOCAL version of Mod-Bot", Color.yellow);
                yield break;
            }

            using (UnityWebRequest modBotVersionRequest = UnityWebRequest.Get("https://modbot.org/api?operation=getCurrentModBotVersion"))
            {
                modBotVersionRequest.timeout = 10;
                yield return modBotVersionRequest.SendWebRequest();
                s_hasChecked = true;

                if (modBotVersionRequest.result != UnityWebRequest.Result.Success)
                    yield break;

                string remoteVersionString = modBotVersionRequest.downloadHandler.text.Trim().Replace("\"", string.Empty);
                if (!Version.TryParse(remoteVersionString, out Version remoteVersion))
                {
                    debug.Log("Could not parse the REMOTE version of Mod-Bot", Color.yellow);
                    yield break;
                }

                if (localVersion >= remoteVersion)
                {
                    string modBotUpToDateMessage = ModBotLocalizationManager.FormatLocalizedStringFromID("modbotuptodate", localVersionString);
                    debug.Log(modBotUpToDateMessage, Color.green);
                    yield break;
                }

                string message = ModBotLocalizationManager.FormatLocalizedStringFromID("newversion_message", remoteVersionString, localVersionString);
                string dismissButtonText = LocalizationManager.Instance.GetTranslatedString("newversion_dismiss");
                string installButtonText = LocalizationManager.Instance.GetTranslatedString("newversion_install");
                Generic2ButtonDialogue generic = new Generic2ButtonDialogue(message, dismissButtonText, null, installButtonText, onInstallButtonClicked);
                generic.SetColorOfFirstButton(Color.red);
                generic.SetColorOfSecondButton(Color.green);
            }
        }

        private void onInstallButtonClicked()
        {
            Application.OpenURL("https://modbot.org/");
        }
    }
}