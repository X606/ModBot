using ModLibrary;
using ModLibrary.LevelEditor;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

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
            // If the mods folder does not exist, something probably went wrong during installation, but try creating it anyway
            string modsDirectory = AssetLoader.GetModsFolderDirectory();
            if (!Directory.Exists(modsDirectory))
            {
                try
                {
                    Directory.CreateDirectory(modsDirectory);
                }
                catch (Exception e)
                {
                    throw new Exception("Can't create mods folder at: " + modsDirectory, e);
                }
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // disable unity analytics to fix possible unity crash
            /*AnalyticsManager.Instance.SendDataToUnityAnalytics = false;
            UnityEngine.Analytics.Analytics.enabled = false;*/

            ModBotHarmonyInjectionManager.TryInject();
            CustomLevelEditorManager.Initialize();
            OptionsSaver.PopulateSettingDictionary();
            ModBotPrefs.Initialize();

            GameObject modBotManagers = new GameObject("ModBotManagers");
            modBotManagers.AddComponent<ModsManager>();                       // Handles mods
            modBotManagers.AddComponent<ModImagesManager>();                  // Gets images of installed mods
            modBotManagers.AddComponent<UpdateChecker>();                     // Checks for new Mod-Bot versions
            modBotManagers.AddComponent<ModsPanelManager>();                  // Adds the mods button in the main menu and pause screen
            modBotManagers.AddComponent<CustomUpgradesUIManager>();           // Handles modded upgrades
            modBotManagers.AddComponent<UpgradeIconDownloader>();             // Downloads images from a URL to be used as an upgrade icon
            modBotManagers.AddComponent<ModdedMultiplayerEventListener>();    // Recieves all multiplayer events and sends them to any mods that has configured to recieve them
            modBotManagers.AddComponent<ModSharingManager>();                 // Handles sharing of mods to all clients on the same server
            modBotManagers.AddComponent<ModBotUserIdentifier>();              // Handles singing in and keeps track of what users are currently using Mod-Bot
            modBotManagers.AddComponent<UpgradeAngleSetter>();                // Handles setting upgrade angles while in-game
            modBotManagers.AddComponent<DebugLineDrawingManager>();           // Handles drawing lines on screen
            modBotManagers.AddComponent<VersionLabelManager>();               // Handles custom version label stuff
            modBotManagers.AddComponent<MultiplayerPlayerNameManager>();      // Handles custom player tags and name overrides in multiplayer
            modBotManagers.AddComponent<ModdedTwitchManager>();               // Handles twitch chat messages

            try // If an exception is thrown here, the crash screen wont appear, so we have to implement our own
            {
                initilizeUI(); // Initialize all custom UI

                ModsManager.Instance.Initialize(); // Loads all mods in the mods folder
            }
            catch (Exception e)
            {
                debug.Log(e.Message + "\n" + e.StackTrace, Color.red);
                ModBotUIRoot.Instance.ConsoleUI.ShowConsole();
            }

            GlobalEventManager.Instance.AddEventListener<FirstPersonMover>(GlobalEvents.UpgradesRefreshed, afterUpgradesRefreshed);
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.LevelEditorStarted, onLevelEditorStarted);

            IgnoreCrashesManager.Start();

            stopwatch.Stop();
            debug.Log("Initialized Mod-Bot in " + stopwatch.Elapsed.TotalSeconds + " seconds");
        }

        static void initilizeUI()
        {
            GameObject spawnedUI = InternalAssetBundleReferences.ModBot.InstantiateObject("Canvas");
            ModBotUIRoot modBotUIRoot = spawnedUI.AddComponent<ModBotUIRoot>();
            modBotUIRoot.Init();
        }

        static void afterUpgradesRefreshed(FirstPersonMover firstPersonMover)
        {
            if (firstPersonMover == null || firstPersonMover.gameObject == null || !firstPersonMover.IsAlive() || firstPersonMover.GetCharacterModel() == null)
                return;

            ModsManager.Instance.PassOnMod.AfterUpgradesRefreshed(firstPersonMover, firstPersonMover.GetComponent<UpgradeCollection>());
        }

        static void onLevelEditorStarted()
        {
            ModsManager.Instance.PassOnMod.OnLevelEditorStarted();
        }
    }
}