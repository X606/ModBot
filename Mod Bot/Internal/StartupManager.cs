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
            ErrorChanger.ChangeError(); // Change error message so that crashes are sent to us and/or the developers of any mods installed instead of the actual game developers

            if (!Directory.Exists(AssetLoader.GetModsFolderDirectory())) // If the mods folder does not exist, something went wrong during installation
            {
                throw new DirectoryNotFoundException("Mods folder not found!");
            }
            
            GameObject gameFlowManager = GameFlowManager.Instance.gameObject;
            
            gameFlowManager.AddComponent<UpdateChecker>();                     // Checks for new Mod-Bot versions
            gameFlowManager.AddComponent<ModsPanelManager>();                  // Adds the mods button in the main menu and pause screen
            gameFlowManager.AddComponent<CustomUpgradeManager>();              // Handles modded upgrades
            gameFlowManager.AddComponent<UpgradeIconDownloader>();             // Downloads images from a URL to be used as an upgrade icon
            gameFlowManager.AddComponent<ModdedMultiplayerEventListener>();    // Recieves all multiplayer events and sends them to any mods that has configured to recieve them
            gameFlowManager.AddComponent<ModSharingManager>();                 // Handles sharing of mods to all clients on the same server
            gameFlowManager.AddComponent<ModBotUserIdentifier>();              // Keeps track of what users are currently using Mod-Bot
            gameFlowManager.AddComponent<UpgradeAngleSetter>();                // Handles setting upgrade angles while in-game

            try // If an exception is thrown here, the crash screen wont appear, so we have to implement our own
            {
                InitilizeUI(); // Initialize all custom UI

                ModsManager modsManager = gameFlowManager.AddComponent<ModsManager>();
                modsManager.Initialize(); // Loads all mods in the mods folder
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
