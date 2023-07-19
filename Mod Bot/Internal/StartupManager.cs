using ModLibrary;
using Rewired;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using InternalModBot.LevelEditor;

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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ModBotHarmonyInjectionManager.TryInject();

            if (!Directory.Exists(AssetLoader.GetModsFolderDirectory())) // If the mods folder does not exist, something probably went wrong during installation
                throw new DirectoryNotFoundException("Mods folder not found!");

            GameObject modBotManagers = new GameObject("ModBotManagers");

            modBotManagers.AddComponent<ModsManager>();
            modBotManagers.AddComponent<UpdateChecker>();                     // Checks for new Mod-Bot versions
            modBotManagers.AddComponent<ModsPanelManager>();                  // Adds the mods button in the main menu and pause screen
            modBotManagers.AddComponent<CustomUpgradeManager>();              // Handles modded upgrades
            modBotManagers.AddComponent<UpgradeIconDownloader>();             // Downloads images from a URL to be used as an upgrade icon
            modBotManagers.AddComponent<ModdedMultiplayerEventListener>();    // Recieves all multiplayer events and sends them to any mods that has configured to recieve them
            modBotManagers.AddComponent<ModSharingManager>();                 // Handles sharing of mods to all clients on the same server
            modBotManagers.AddComponent<ModBotUserIdentifier>();              // Keeps track of what users are currently using Mod-Bot
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
                ModBotUIRoot.Instance.ConsoleUI.Animator.Play("hideConsole");
            }

            ModBotCustomLevelEditorManager.Init();

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.UpgradesRefreshed, new Action<FirstPersonMover>(PassOnToModsManager.AfterUpgradesRefreshed));
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.LevelEditorStarted, new Action(ModsManager.Instance.PassOnMod.OnLevelEditorStarted));

            IgnoreCrashesManager.Start();

            stopwatch.Stop();
            debug.Log("Initialized Mod-Bot in " + stopwatch.Elapsed.TotalSeconds + " seconds");
        }

        static void initilizeUI()
        {
            GameObject spawnedUI = InternalAssetBundleReferences.ModBot.InstantiateObject("Canvas");
            ModdedObject spawedUIModdedObject = spawnedUI.GetComponent<ModdedObject>();

            ModBotUIRoot modBotUIRoot = spawnedUI.AddComponent<ModBotUIRoot>();
            modBotUIRoot.Init(spawedUIModdedObject);
            
        }
    }
}
