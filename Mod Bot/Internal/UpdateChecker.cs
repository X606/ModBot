using ModLibrary;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using ModLibrary.Properties;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to check if there is a newer version available
    /// </summary>
    public class UpdateChecker : MonoBehaviour
    {
        void Start()
        {
            if (!GameFlowManager.Instance.IsOnTitleScreen()) // If the user is currently playing any game mode, dont check for updates
                return;

            StartCoroutine(checkVersion()); // Needs to be a Coroutine since the web requests are not asynchronous
        }

        IEnumerator checkVersion()
        {
            string installedModBotVersion = ModLibrary.Properties.Resources.ModBotVersion;

            UnityWebRequest modBotVersionRequest = UnityWebRequest.Get("https://modbot-d8a58.firebaseio.com/ModBotVer/.json");
            yield return modBotVersionRequest.SendWebRequest();

            if(modBotVersionRequest.isNetworkError || modBotVersionRequest.isHttpError)
                yield break;

            string newestModBotVersion = modBotVersionRequest.downloadHandler.text.Replace("\"", ""); // Latest ModBot version

            string modBotVersionLabel = ModBotLocalizationManager.FormatLocalizedStringFromID("modbotversion", installedModBotVersion);
            GameUIRoot.Instance.TitleScreenUI.VersionLabel.text += "\n" + modBotVersionLabel;

            if (installedModBotVersion == newestModBotVersion)
            {
                string modBotUpToDateMessage = ModBotLocalizationManager.FormatLocalizedStringFromID("modbotuptodate", installedModBotVersion);
                debug.Log(modBotUpToDateMessage, Color.green);
                yield break;
            }

            string message = ModBotLocalizationManager.FormatLocalizedStringFromID("newversion_message", newestModBotVersion, installedModBotVersion);
            string dismissButtonText = LocalizationManager.Instance.GetTranslatedString("newversion_dismiss");
            string installButtonText = LocalizationManager.Instance.GetTranslatedString("newversion_install");
            Generic2ButtonDialogue generic = new Generic2ButtonDialogue(message, dismissButtonText, null, installButtonText, onInstallButtonClicked);
            generic.SetColorOfFirstButton(Color.red);
            generic.SetColorOfSecondButton(Color.green);
        }

        void onInstallButtonClicked()
        {
            Application.OpenURL("http://clonedronemodbot.com/");
        }
    }
}
