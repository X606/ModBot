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
            {
                return;
            }

            GameObject prefab = AssetLoader.GetObjectFromFile("newversionalert", "Canvas", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedObject = Instantiate(prefab);
            spawnedModdedObject = spawnedObject.GetComponent<ModdedObject>();

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

            GameUIRoot.Instance.TitleScreenUI.VersionLabel.text += "\nMod-Bot Version: " + installedModBotVersion; // Add ModBot version in corner
            
            if (installedGameVersion != newestModBotGameVersion || installedModBotVersion == newestModBotVersion)
            {
                debug.Log("Mod-Bot version '" + installedModBotVersion + "' up to date!", Color.green);
                return;
            }

            spawnedModdedObject.GetObject<Text>(0).text = "New Mod-Bot version available: " + newestModBotVersion + "\n(current version: " + installedModBotVersion + ")";
            spawnedModdedObject.GetObject<Button>(1).onClick.AddListener(OnInstallButtonClicked);
            spawnedModdedObject.GetObject<Button>(2).onClick.AddListener(OnDismissButtonClicked);
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

        private ModdedObject spawnedModdedObject;
    }
}
