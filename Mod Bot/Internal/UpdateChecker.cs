using ModLibrary;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to check if there is a newer version available
    /// </summary>
    public class UpdateChecker : MonoBehaviour
    {
        private void Start()
        {
            if (!GameModeManager.Is(GameMode.None)) // If the user is currently playing any game mode, dont even check for updates
            {
                return;
            }

            StartCoroutine(CheckVersion()); // Needs to be a Coroutine since the web requests are not asynchronous
        }

        private IEnumerator CheckVersion()
        {
            string installedModBotVersion = File.ReadAllLines(AssetLoader.GetSubdomain(Application.dataPath) + "\\version.txt")[1].Remove(0, 8); // Current ModBot version
            
            UnityWebRequest modBotVersionRequest = UnityWebRequest.Get("https://modbot-d8a58.firebaseio.com/ModBotVer/.json");
            yield return modBotVersionRequest.SendWebRequest();
            if(modBotVersionRequest.isNetworkError || modBotVersionRequest.isHttpError)
                yield break;

            string newestModBotVersion = modBotVersionRequest.downloadHandler.text.Replace("\"", ""); // Latest ModBot version

            GameUIRoot.Instance.TitleScreenUI.VersionLabel.text += "\nMod-Bot Version: " + installedModBotVersion; // Add ModBot version in corner
            
            if (installedModBotVersion == newestModBotVersion)
            {
                debug.Log("Mod-Bot version '" + installedModBotVersion + "' up to date!", Color.green);
                yield break;
            }

            string message = "New Mod-Bot version available: " + newestModBotVersion + "\n(current version: " + installedModBotVersion + ")";

            Generic2ButtonDialogue generic = new Generic2ButtonDialogue(message, "Dismiss", null, "Install", OnInstallButtonClicked);
            generic.SetColorOfFirstButton(Color.red);
            generic.SetColorOfSecondButton(Color.green);
        }

        private void OnInstallButtonClicked()
        {
            Application.OpenURL("http://clonedronemodbot.com/");
        }
    }
}
