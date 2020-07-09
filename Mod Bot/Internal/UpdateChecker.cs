using ModLibrary;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using ModLibrary.Properties;
using System.Collections.Generic;
using System.Linq;
using System;

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

            if (installedModBotVersion.ToLower().Contains("beta"))
                yield break;

            UnityWebRequest modBotVersionRequest = UnityWebRequest.Get("https://modbot-d8a58.firebaseio.com/ModBotVer/.json");
            yield return modBotVersionRequest.SendWebRequest();

            if(modBotVersionRequest.isNetworkError || modBotVersionRequest.isHttpError)
                yield break;

            string newestModBotVersion = modBotVersionRequest.downloadHandler.text.Replace("\"", ""); // Latest ModBot version

            string modBotVersionLabel = ModBotLocalizationManager.FormatLocalizedStringFromID("modbotversion", installedModBotVersion);
            GameUIRoot.Instance.TitleScreenUI.VersionLabel.text += "\n" + modBotVersionLabel;

            if (!isCloudVersionNewer(installedModBotVersion, newestModBotVersion))
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

		bool isCloudVersionNewer(string installedVersion, string cloudVersion)
		{
			string[] installedVersionStrings = installedVersion.Split('.');
			string[] cloudVersionStrings = cloudVersion.Split('.');

			int lengthOfLongest = Mathf.Max(installedVersionStrings.Length, cloudVersionStrings.Length);

			int[] installedVersionNumbers = new int[lengthOfLongest];
			int[] cloudVersionNumbers = new int[lengthOfLongest];

			for(int i = 0; i < lengthOfLongest; i++)
			{
				if (i >= installedVersionStrings.Length)
				{
					installedVersionNumbers[i] = 0;
				}
				else if(int.TryParse(installedVersionStrings[i], out int number))
				{
					installedVersionNumbers[i] = number;
				}
				else
				{
					throw new Exception("The installed version string was invalid");
				}

				if(i >= cloudVersionStrings.Length)
				{
					cloudVersionNumbers[i] = 0;
				}
				else if(int.TryParse(cloudVersionStrings[i], out int number))
				{
					cloudVersionNumbers[i] = number;
				}
				else
				{
					throw new Exception("The cloud version string was invalid");
				}
			}

			for(int i = 0; i < lengthOfLongest; i++)
			{
				if(installedVersionNumbers[i] > cloudVersionNumbers[i])
					return false;
			}

			return true;
		}

        void onInstallButtonClicked()
        {
            Application.OpenURL("http://clonedronemodbot.com/");
        }
    }
}
