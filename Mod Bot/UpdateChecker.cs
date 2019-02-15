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
        private void Awake()
        {
            GameObject prefab = AssetLoader.getObjectFromFile("newversionalert", "Canvas", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedObject = GameObject.Instantiate(prefab);
            spawnedModdedObject = spawnedObject.GetComponent<moddedObject>();
            
            spawnedObject.SetActive(false);

            Thread updateThread = new Thread(ThreadSequence);
            updateThread.Start();
        }

        private void ThreadSequence()
        {
            string installedGameVersion = VersionNumberManager.Instance.GetVersionString();
            string newestModBotGameVersion = FirebaseAccessor.ReadFromFirebaseURL("https://modbot-d8a58.firebaseio.com/cloneDroneVer/.json");
            string installedModBotVersion = File.ReadAllLines(AssetLoader.getSubdomain(Application.dataPath) + "\\version.txt")[1].Remove(0, 8);
            string newestModBotVersion = FirebaseAccessor.ReadFromFirebaseURL("https://modbot-d8a58.firebaseio.com/ModBotVer/.json");

            GameUIRoot.Instance.TitleScreenUI.VersionLabel.text += "\nModBot Version: " + installedModBotVersion;

            if (installedGameVersion != newestModBotGameVersion || installedModBotVersion == newestModBotVersion)
            {
                debug.Log("ModBot version '" + installedModBotVersion + "' up to date!", Color.green);
                return;
            }
            
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
