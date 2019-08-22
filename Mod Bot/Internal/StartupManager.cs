using ModLibrary;
using System;
using System.IO;
using UnityEngine;

namespace InternalModBot
{
    public static class StartupManager
    {
        public static void OnStartUp()
        {
            if (!Directory.Exists(AssetLoader.GetModsFolderDirectory()))
            {
                throw new DirectoryNotFoundException("Mods folder not found!");
            }

            ErrorChanger.ChangeError();

            GameObject gameFlowManager = GameFlowManager.Instance.gameObject;
            
            gameFlowManager.AddComponent<ModsManager>();
            gameFlowManager.AddComponent<UpdateChecker>();
            gameFlowManager.AddComponent<ModsPanelManager>();
            gameFlowManager.AddComponent<CustomUpgradeManager>();
            gameFlowManager.AddComponent<UpgradeIconDownloader>();

            ModsManager.Instance.PassOnMod.OnSceneChanged(GameMode.None);

            InitilizeUI();
            
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.UpgradesRefreshed, new Action<FirstPersonMover>(CalledFromInjections.FromRefreshUpgradesEnd));
            
            IgnoreCrashesManager.Start();
        }

        private static void InitilizeUI()
        {
            GameObject spawnedUI = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("twitchmode", "Canvas", "Clone Drone in the Danger Zone_Data/"));
            ModdedObject spawedUIModdedObject = spawnedUI.GetComponent<ModdedObject>();

            Logger logger = spawnedUI.AddComponent<Logger>();
            logger.animator = spawedUIModdedObject.GetObject<Animator>(0);
            logger.LogText = spawedUIModdedObject.GetObject<UnityEngine.UI.Text>(1);
            logger.Container = spawedUIModdedObject.GetObject<GameObject>(2);
            logger.input = spawedUIModdedObject.GetObject<UnityEngine.UI.InputField>(3);

            FPSCount fps = spawnedUI.AddComponent<FPSCount>();
            fps.counter = spawedUIModdedObject.GetObject<UnityEngine.UI.Text>(4);
        }
    }
}
