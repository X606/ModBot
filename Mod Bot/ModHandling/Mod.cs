﻿using InternalModBot;
using System.Collections.Generic;
using System.Reflection;
using TwitchChatter;
using UnityEngine;

namespace ModLibrary
{
    /// <summary>
    /// Base class for all mods, contains virtual implementations for different events in the game.
    /// </summary>
    public abstract class Mod
    {
        /// <summary>
        /// The modinfo that goes with this Mod, this contains data about the mod name, version ect.
        /// </summary>
        public ModInfo ModInfo => ModsManager.Instance.GetInfo(this);

        /// <summary>
        /// Returns an ID you should use when harmony patching in this mod, this is to help mod-bot clean up patches made by this mod.
        /// </summary>
        protected internal string HarmonyID => "com.Mod-Bot.Mod." + ModInfo.UniqueID;

        internal Assembly SourceAssembly;

        /// <summary>
        /// Returns if this mod instance has an upgrade page defined, check if this is false before adding upgrades to avoid duplicates
        /// </summary>
        /// <returns></returns>
        protected bool hasRegisteredUpgradePage()
        {
            return UpgradePagesManager.HasPageForMod(ModInfo.UniqueID);
        }

        /// <summary>
        /// Called in <see cref="Character.Start"/> if the <see cref="Character"/> is of type <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="firstPersonMover">The <see cref="FirstPersonMover"/> that was spawned</param>
        protected internal virtual void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
        }

        /// <summary>
        /// Called in <see cref="Character.Update"/> if the <see cref="Character"/> is of type <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="firstPersonMover">The <see cref="FirstPersonMover"/> that was updated</param>
        protected internal virtual void OnFirstPersonMoverUpdate(FirstPersonMover firstPersonMover)
        {
        }

        /// <summary>
        /// Called in <see cref="Character.Start"/>
        /// </summary>
        /// <param name="character">The <see cref="Character"/> that was spawned</param>
        protected internal virtual void OnCharacterSpawned(Character character)
        {
        }

        /// <summary>
        /// Called in <see cref="Character.Update"/>
        /// </summary>
        /// <param name="character">The <see cref="Character"/> that was updated</param>
        protected internal virtual void OnCharacterUpdate(Character character)
        {
        }

        /// <summary>
        /// called at the end of <see cref="FirstPersonMover.CreateCharacterModel(CharacterModel)"/>
        /// </summary>
        /// <param name="owner">The owner of the new character model</param>
        protected internal virtual void OnCharacterModelCreated(FirstPersonMover owner)
        {
        }

        /// <summary>
        /// Called in <see cref="ModsManager.ReloadMods()"/>
        /// </summary>
        protected internal virtual void OnModRefreshed()
        {
        }

        /// <summary>
        /// Called when the level editor is started.
        /// </summary>
        protected internal virtual void OnLevelEditorStarted()
        {
        }

        /// <summary>
        /// Called when you run a command in the console (mostly for debuging).
        /// </summary>
        /// <param name="command">The text entered into the command field of the console</param>
        protected internal virtual void OnCommandRan(string command)
        {
        }

        /// <summary>
        /// Called at the start <see cref="FirstPersonMover.RefreshUpgrades"/>
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="upgrades">The <see cref="UpgradeCollection"/> on the <see cref="FirstPersonMover"/> object</param>
        protected internal virtual void OnUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
        }

        /// <summary>
        /// Called at the end of <see cref="FirstPersonMover.RefreshUpgrades"/>
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="upgrades">The <see cref="UpgradeCollection"/> on the <see cref="FirstPersonMover"/> object</param>
        protected internal virtual void AfterUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
        }

        /// <summary>
        /// Called in <see cref="Character.onDeath(Character, DamageSourceType, int)"/>
        /// </summary>
        /// <param name="killedCharacter">The <see cref="Character"/> that was killed</param>
        /// <param name="killerCharacter">The killer <see cref="Character"/></param>
        /// <param name="damageSourceType">The cause of death</param>
        /// <param name="attackID">The attack ID that killed <paramref name="killedCharacter"/></param>
        protected internal virtual void OnCharacterKilled(Character killedCharacter, Character killerCharacter, DamageSourceType damageSourceType, int attackID)
        {
        }

        /// <summary>
        /// Called when the mod is deactivated from the mods menu, override this method to perform additional cleanup, like remove/disable all added components, reset values in classes, etc. Modded upgrades are automatically hidden when the owner mod is disabled
        /// </summary>
        protected internal virtual void OnModDeactivated()
        {
        }

        /// <summary>
        /// If this returns <see langword="true"/> it will active the mod settings button in the mods window for this mod.
        /// </summary>
        /// <returns></returns>
        protected internal virtual bool ImplementsSettingsWindow()
        {
            return false;
        }

        /// <summary>
        /// Gets called when the user clicks on the mod settings button in the mods window. Allows you to create a neat little UI that saves the values for you. Get the values set by this with SettingsManager.Instance.GetModdedSettingsBoolValue, GetModdedSettingsStringValue, GetModdedSettingsIntValue and GetModdedSettingsFloatValue
        /// </summary>
        /// <param name="builder">The object used to build the UI.</param>
        protected internal virtual void CreateSettingsWindow(ModOptionsWindowBuilder builder)
        {
        }

        /// <summary>
        /// If this returns <see langword="true"/> the cursor will get enabled
        /// </summary>
        /// <returns></returns>
        protected internal virtual bool ShouldCursorBeEnabled()
        {
            return false;
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        protected internal virtual void GlobalUpdate()
        {
        }

        /// <summary>
        /// Called whenever we received a <see cref="GenericStringForModdingEvent"/> from a client (including your own if you used <see cref="MultiplayerMessageSender.SendToAllClients(string)"/>)
        /// </summary>
        /// <param name="moddedEvent">The received <see cref="GenericStringForModdingEvent"/></param>
        protected internal virtual void OnMultiplayerEventReceived(GenericStringForModdingEvent moddedEvent)
        {
        }

        /// <summary>
        /// Called when the mod gets loaded or enabled after previously being disabled
        /// </summary>
        protected internal virtual void OnModEnabled()
        {
        }

        /// <summary>
        /// Will be called just before anything tries to load something from the Resources folder. If this returns <see langword="null"/> it will continue like normal, but if this returns anything else than <see langword="null"/> that will be returned by <see cref="Resources.Load(string)"/> instead.
        /// </summary>
        /// <param name="path">The path specified</param>
        /// <returns></returns>
        protected internal virtual UnityEngine.Object OnResourcesLoad(string path)
        {
            return null;
        }

        /// <summary>
        /// Gets called when the language dictionary gets populated, use this method to add or change the dictionary when the language will be changed
        /// </summary>
        /// <param name="newLanguageID">The language ID that was switched to</param>
        /// <param name="localizationDictionary">The dictionary containing all IDs and localized strings, key string is an ID, value string is the text that will be displayed</param>
        protected internal virtual void OnLanguageChanged(string newLanguageID, Dictionary<string, string> localizationDictionary)
        {
        }

        /// <summary>
        /// Gets called directly after the mod is loaded. WARNING: Any exceptions thrown in the method will not be displayed by the <see cref="ErrorManager"/> since is hasn't been initialized at this point in time, if an exception is thrown, the game will pause itself before the title screen appears. The crahs log can still be found in the outbut_log.txt file
        /// </summary>
        protected internal virtual void OnModLoaded()
        {
        }

        /// <summary>
        /// Called when we connect to a multiplayer server
        /// </summary>
        protected internal virtual void OnClientConnectedToServer()
        {
        }

        /// <summary>
        /// Called when we disconnect from a multiplayer server
        /// </summary>
        protected internal virtual void OnClientDisconnectedFromServer()
        {
        }

        /// <summary>
        /// Called when someone sends a message in twitch chat.
        /// </summary>
        /// <param name="message">The twitch chat message</param>
        protected internal virtual void OnTwitchChatMessage(TwitchChatMessage message)
        {
        }
    }
}
