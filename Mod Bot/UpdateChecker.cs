using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using ModLibrary;

namespace InternalModBot
{
    public class UpdateChecker : MonoBehaviour
    {
        private void Start()
        {
            if (!GameModeManager.Is(GameMode.None))
                return;

            GameObject prefab = AssetLoader.getObjectFromFile("newversionalert", "Canvas", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedObject = GameObject.Instantiate(prefab);
            spawnedModdedObject = spawnedObject.GetComponent<moddedObject>();
            
            spawnedObject.SetActive(false);

            Thread updateThread = new Thread(ThreadSequence); // In separate thread because it waits for a web response
            updateThread.Start();
        }

        private void ThreadSequence()
        {
            string installedGameVersion = VersionNumberManager.Instance.GetVersionString(); // Current game version
            string newestModBotGameVersion = FirebaseAccessor.ReadFromFirebaseURL("https://modbot-d8a58.firebaseio.com/cloneDroneVer/.json"); // The latest version ModBot is updated for
            string installedModBotVersion = File.ReadAllLines(AssetLoader.getSubdomain(Application.dataPath) + "\\version.txt")[1].Remove(0, 8); // Current ModBot version
            string newestModBotVersion = FirebaseAccessor.ReadFromFirebaseURL("https://modbot-d8a58.firebaseio.com/ModBotVer/.json"); // Latest ModBot version

            GameUIRoot.Instance.TitleScreenUI.VersionLabel.text += "\nModBot Version: " + installedModBotVersion; // Add ModBot version in corner


           
            if (installedGameVersion != newestModBotGameVersion || installedModBotVersion == newestModBotVersion)
            {
                debug.Log("ModBot version '" + installedModBotVersion + "' up to date!", Color.green);
                return;
            }
            if (spawnedModdedObject.transform.GetChild(1).GetComponent<Image>().sprite == null)
                debug.Log("nulk");
            if (GameUIRoot.Instance.SettingsMenu.GetComponent<Image>().sprite)
                debug.Log("mulk2");

            debug.Log("milk");

            ((Text)spawnedModdedObject.objects[0]).text = "New ModBot version available: " + newestModBotVersion + "\n(current version: " + installedModBotVersion + ")";
            ((Button)spawnedModdedObject.objects[1]).onClick.AddListener(OnInstallButtonClicked);
            ((Button)spawnedModdedObject.objects[2]).onClick.AddListener(OnDismissButtonClicked);
            spawnedModdedObject.gameObject.SetActive(true);
        }

        private void OnInstallButtonClicked()
        {
            Application.OpenURL("http://clonedronemodbot.com/");
        }

        private void OnDismissButtonClicked()
        {
            spawnedModdedObject.gameObject.SetActive(false);
        }

        private moddedObject spawnedModdedObject;

        // https://modbot-d8a58.firebaseio.com/ModBotDownloadLink/.json
        // https://modbot-d8a58.firebaseio.com/ModBotVer/.json
        // https://modbot-d8a58.firebaseio.com/cloneDroneVer/.json
    }
}
