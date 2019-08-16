using ModLibrary;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    public class UpdateChecker : MonoBehaviour
    {
        private void Start()
        {
            if (!GameModeManager.Is(GameMode.None))
                return;

            GameObject prefab = AssetLoader.GetObjectFromFile("newversionalert", "Canvas", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedObject = GameObject.Instantiate(prefab);
            spawnedModdedObject = spawnedObject.GetComponent<moddedObject>();

            spawnedModdedObject.gameObject.SetActive(false);

            Thread updateThread = new Thread(ThreadSequence); // In separate thread because it waits for a web response
            updateThread.Start();
        }

        private void ThreadSequence()
        {
            string installedGameVersion = VersionNumberManager.Instance.GetVersionString(); // Current game version
            string newestModBotGameVersion = FirebaseAccessor.ReadFromFirebaseURL("https://modbot-d8a58.firebaseio.com/cloneDroneVer/.json"); // The latest version ModBot is updated for
            string installedModBotVersion = File.ReadAllLines(AssetLoader.GetSubdomain(Application.dataPath) + "\\version.txt")[1].Remove(0, 8); // Current ModBot version
            string newestModBotVersion = FirebaseAccessor.ReadFromFirebaseURL("https://modbot-d8a58.firebaseio.com/ModBotVer/.json"); // Latest ModBot version

            GameUIRoot.Instance.TitleScreenUI.VersionLabel.text += "\nModBot Version: " + installedModBotVersion; // Add ModBot version in corner
            
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
    }
}
