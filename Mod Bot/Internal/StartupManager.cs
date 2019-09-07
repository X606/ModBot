using ModLibrary;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Used to start Mod-Bot when the game starts
    /// </summary>
    public static class StartupManager
    {
        /// <summary>
        /// Sets up Mod-Mot in general, called on game start
        /// </summary>
        public static void OnStartUp()
        {
            ErrorChanger.ChangeError();

            if (!Directory.Exists(AssetLoader.GetModsFolderDirectory()))
            {
                throw new DirectoryNotFoundException("Mods folder not found!");
            }
            
            GameObject gameFlowManager = GameFlowManager.Instance.gameObject;
            
            gameFlowManager.AddComponent<UpdateChecker>();
            gameFlowManager.AddComponent<ModsPanelManager>();
            gameFlowManager.AddComponent<CustomUpgradeManager>();
            gameFlowManager.AddComponent<UpgradeIconDownloader>();
            gameFlowManager.AddComponent<ModdedMultiplayerEventListener>();
            gameFlowManager.AddComponent<ModSharingManager>();
            gameFlowManager.AddComponent<ModBotUserIdentifier>();

            try
            {
                InitilizeUI();

                ModsManager modsManager = gameFlowManager.AddComponent<ModsManager>();
                modsManager.Initialize();
            }
            catch (Exception e)
            {
                debug.Log(e.Message + "\n" + e.StackTrace, Color.red);
                Logger.Instance.animator.Play("hideConsole");
            }

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.UpgradesRefreshed, new Action<FirstPersonMover>(CalledFromInjections.FromRefreshUpgradesEnd));
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.LevelEditorStarted, new Action(ModsManager.Instance.PassOnMod.OnLevelEditorStarted));
            
            IgnoreCrashesManager.Start();
        }

        private static void InitilizeUI()
        {
            GameObject spawnedUI = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("twitchmode", "Canvas", "Clone Drone in the Danger Zone_Data/"));
            ModdedObject spawedUIModdedObject = spawnedUI.GetComponent<ModdedObject>();

            Logger logger = spawnedUI.AddComponent<Logger>();
            logger.animator = spawedUIModdedObject.GetObject<Animator>(0);
            logger.LogText = spawedUIModdedObject.GetObject<Text>(1);
            logger.Container = spawedUIModdedObject.GetObject<GameObject>(2);
            logger.input = spawedUIModdedObject.GetObject<InputField>(3);

            FPSCount fps = spawnedUI.AddComponent<FPSCount>();
            fps.counter = spawedUIModdedObject.GetObject<Text>(4);

            ModSuggestingManager modSuggestingManager = spawnedUI.AddComponent<ModSuggestingManager>();
            ModdedObject modSuggestingManagerInfo = spawedUIModdedObject.GetObject<ModdedObject>(5);
            modSuggestingManager.displayText = modSuggestingManagerInfo.GetObject<Text>(0);
            modSuggestingManager.ModSuggestionAnimator = modSuggestingManagerInfo.GetObject<Animator>(1);
        }
    }
}
