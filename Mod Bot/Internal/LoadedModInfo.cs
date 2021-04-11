using HarmonyLib;
using ModLibrary;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace InternalModBot
{
    /// <summary>
    /// Stores all necessary data about a loaded mod
    /// </summary>
    internal class LoadedModInfo : IMod
    {
        /// <summary>
        /// Sets the mod field to the passed mod, and will not deactivate the mod
        /// </summary>
        /// <param name="mod"></param>
        internal LoadedModInfo(IMod mod)
        {
            ModReference = mod;
        }

        internal IMod ModReference;

        public bool IsEnabled
        {
            get
            {
				return ModInfo.IsModEnabled;
            }
            internal set
            {
                if (IsEnabled == value) // If the mod's state hasn't changed
                    return;

                PlayerPrefs.SetInt(ModInfo.UniqueID, value ? 1 : 0);

                if (value) // If the mod is being enabled
                {
					if (ModReference == null)
					{
						ModsManager.Instance.LoadMod(ModInfo);
					}
                    else
					{
						ModReference.OnModEnabled();
					}

                    Harmony harmony = new Harmony(ModReference.HarmonyID);
                    KeyValuePair<InjectionTargetAttribute, MethodInfo>[] injections = InjectionTargetAttribute.GetInjectionTargetsInAssembly(ModReference.GetType().Assembly);
					foreach (KeyValuePair<InjectionTargetAttribute, MethodInfo> injection in injections)
					{
                        HarmonyMethod prefix = null;
                        if (injection.Key.InjectionType == InjectionType.Prefix)
                            prefix = new HarmonyMethod(injection.Value);

                        HarmonyMethod postfix = null;
                        if (injection.Key.InjectionType == InjectionType.Postfix)
                            postfix = new HarmonyMethod(injection.Value);

                        harmony.Patch(injection.Key.SelectedMethod, prefix, postfix);
                    }
				}
                else // If the mod is being disabled
                {
                    CustomUpgradeManager.NextClicked();
                    UpgradePagesManager.RemoveModdedUpgradesFor(ModInfo.UniqueID);

                    ModReference.HarmonyInstance.UnpatchAll(); // unpatches all of the patches made by the mod
                    ModReference.OnModDeactivated();
                }

				ModsManager.Instance.RefreshAllLoadedActiveMods();
            }
        }

        public string HarmonyID => ModReference.HarmonyID;

        public Harmony HarmonyInstance => ModReference.HarmonyInstance;

        public ModInfo ModInfo => ModReference.ModInfo;

        public void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
            ModReference.OnFirstPersonMoverSpawned(firstPersonMover);
        }

        public void OnFirstPersonMoverUpdate(FirstPersonMover firstPersonMover)
        {
            ModReference.OnFirstPersonMoverUpdate(firstPersonMover);
        }

        public void OnCharacterSpawned(Character character)
        {
            ModReference.OnCharacterSpawned(character);
        }

        public void OnCharacterUpdate(Character character)
        {
            ModReference.OnCharacterUpdate(character);
        }

        public void OnCharacterModelCreated(FirstPersonMover owner)
        {
            ModReference.OnCharacterModelCreated(owner);
        }

        public void OnModRefreshed()
        {
            ModReference.OnModRefreshed();
        }

        public void OnLevelEditorStarted()
        {
            ModReference.OnLevelEditorStarted();
        }

        public void OnCommandRan(string command)
        {
            ModReference.OnCommandRan(command);
        }

        public void OnUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            ModReference.OnUpgradesRefreshed(owner, upgrades);
        }

        public void AfterUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            ModReference.AfterUpgradesRefreshed(owner, upgrades);
        }

        public void OnCharacterKilled(Character killedCharacter, Character killerCharacter, DamageSourceType damageSourceType, int attackID)
        {
            ModReference.OnCharacterKilled(killedCharacter, killerCharacter, damageSourceType, attackID);
        }

        public void OnModDeactivated()
        {
            ModReference.OnModDeactivated();
        }

        public bool ImplementsSettingsWindow()
        {
            return ModReference.ImplementsSettingsWindow();
        }

        public void CreateSettingsWindow(ModOptionsWindowBuilder builder)
        {
            ModReference.CreateSettingsWindow(builder);
        }

        public bool ShouldCursorBeEnabled()
        {
            return ModReference.ShouldCursorBeEnabled();
        }

        public void GlobalUpdate()
        {
            ModReference.GlobalUpdate();
        }

        public void OnMultiplayerEventReceived(GenericStringForModdingEvent moddedEvent)
        {
            ModReference.OnMultiplayerEventReceived(moddedEvent);
        }

        public void OnModEnabled()
        {
            ModReference.OnModEnabled();
        }

        public UnityEngine.Object OnResourcesLoad(string path)
        {
            return ModReference.OnResourcesLoad(path);
        }

        public void OnLanguageChanged(string newLanguageID, Dictionary<string, string> localizationDictionary)
        {
            ModReference.OnLanguageChanged(newLanguageID, localizationDictionary);
        }

        public void OnModLoaded()
        {
            ModReference.OnModLoaded();
        }

        public void OnClientConnectedToServer()
        {
            ModReference.OnClientConnectedToServer();
        }

        public void OnClientDisconnectedFromServer()
        {
            ModReference.OnClientDisconnectedFromServer();
        }
    }
}