// New mod loading system
using ModLibrary;
using System;
using System.Collections.Generic;
using TwitchChatter;
using UnityEngine;
#pragma warning disable CS0618 // We dont care if its depricated sincei

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to call events on all loaded active mods, you probably dont want to use this from mods
    /// </summary>
    public class PassOnToModsManager : Mod
    {
        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="me"></param>
        protected internal override void OnFirstPersonMoverSpawned(FirstPersonMover me)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].OnFirstPersonMoverSpawned(me);
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnFirstPersonMoverSpawned)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="me"></param>
        protected internal override void OnFirstPersonMoverUpdate(FirstPersonMover me)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].OnFirstPersonMoverUpdate(me);
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnFirstPersonMoverUpdate)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        protected internal override void OnModRefreshed()
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].OnModRefreshed();
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnModRefreshed)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        protected internal override void OnLevelEditorStarted()
        {
#if MODDED_LEVEL_OBJECTS
            LevelEditorObjectAdder.OnLevelEditorStarted();
#endif
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].OnLevelEditorStarted();
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnLevelEditorStarted)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="command"></param>
        protected internal override void OnCommandRan(string command)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].OnCommandRan(command);
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnCommandRan)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="me"></param>
        /// <param name="upgrades"></param>
        protected internal override void OnUpgradesRefreshed(FirstPersonMover me, UpgradeCollection upgrades)
        {
            FirstPersonMover firstPersonMover = me.GetComponent<FirstPersonMover>();
            if (!firstPersonMover.IsAlive() || firstPersonMover.GetCharacterModel() == null)
            {
                return;
            }

            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].OnUpgradesRefreshed(me, upgrades);
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnUpgradesRefreshed)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Calls this method on all mods, also calls OnFirstPersonMoverSpawned if the passed character is a FirstPersonMover
        /// </summary>
        /// <param name="me"></param>
        protected internal override void OnCharacterSpawned(Character me)
        {
            if (me.GetComponent<Character>() is FirstPersonMover)
            {
                OnFirstPersonMoverSpawned(me as FirstPersonMover);
            }

            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].OnCharacterSpawned(me);
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnCharacterSpawned)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Calls this method on all mods, also calls OnFirstPersonMoverUpdate if the passed character is a firstpersonmover
        /// </summary>
        /// <param name="me"></param>
        protected internal override void OnCharacterUpdate(Character me)
        {
            if (me.GetComponent<Character>() is FirstPersonMover)
            {
                OnFirstPersonMoverUpdate(me as FirstPersonMover);
            }

            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].OnCharacterUpdate(me);
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnCharacterUpdate)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Moved from <see cref="CalledFromInjections"/>, checks for <see langword="null"/> and calls <see cref="AfterUpgradesRefreshed(FirstPersonMover, UpgradeCollection)"/>
        /// </summary>
        /// <param name="firstPersonMover"></param>
        protected internal static void AfterUpgradesRefreshed(FirstPersonMover firstPersonMover)
        {
            if (firstPersonMover == null || firstPersonMover.gameObject == null || !firstPersonMover.IsAlive() || firstPersonMover.GetCharacterModel() == null)
                return;

            ModsManager.Instance.PassOnMod.AfterUpgradesRefreshed(firstPersonMover, firstPersonMover.GetComponent<UpgradeCollection>());
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="upgrades"></param>
        protected internal override void AfterUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].AfterUpgradesRefreshed(owner, upgrades);
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(AfterUpgradesRefreshed)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="killedCharacter"></param>
        /// <param name="killerCharacter"></param>
        /// <param name="damageSourceType"></param>
        /// <param name="attackID"></param>
        protected internal override void OnCharacterKilled(Character killedCharacter, Character killerCharacter, DamageSourceType damageSourceType, int attackID)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].OnCharacterKilled(killedCharacter, killerCharacter, damageSourceType, attackID);
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnCharacterKilled)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        protected internal override void OnModDeactivated()
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].OnModDeactivated();
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnModDeactivated)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Gets the response from this from all loaded mods, and uses the or operator on all of them, then returns
        /// </summary>
        /// <returns></returns>
        protected internal override bool ShouldCursorBeEnabled() // if any mod tells the game that the cursor should be enabled, it will be
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                bool result;
                try
                {
                    result = mods[i].ShouldCursorBeEnabled();
                }
                catch (Exception exc)
                {
                    result = false;
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(ShouldCursorBeEnabled)}.\n", exc)));
                }

                if (result)
                    return true;
            }

            return Generic2ButtonDialogue.IsWindowOpen;
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        protected internal override void GlobalUpdate()
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].GlobalUpdate();
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(GlobalUpdate)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="moddedEvent"></param>
        protected internal override void OnMultiplayerEventReceived(GenericStringForModdingEvent moddedEvent)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].OnMultiplayerEventReceived(moddedEvent);
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnMultiplayerEventReceived)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected internal override UnityEngine.Object OnResourcesLoad(string path)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                UnityEngine.Object obj;
                try
                {
                    obj = mods[i].OnResourcesLoad(path);
                }
                catch (Exception exc)
                {
                    obj = null;
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnResourcesLoad)}.\n", exc)));
                }

                if (obj != null)
                    return obj;
            }

            return null;
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="newLanguageID"></param>
        /// <param name="localizationDictionary"></param>
        protected internal override void OnLanguageChanged(string newLanguageID, Dictionary<string, string> localizationDictionary)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].OnLanguageChanged(newLanguageID, localizationDictionary);
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnLanguageChanged)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        protected internal override void OnClientConnectedToServer()
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].OnClientConnectedToServer();
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnClientConnectedToServer)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        protected internal override void OnClientDisconnectedFromServer()
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].OnClientDisconnectedFromServer();
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnClientDisconnectedFromServer)}.\n", exc)));
                }
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        protected internal override void OnTwitchChatMessage(TwitchChatMessage message)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                try
                {
                    mods[i].OnTwitchChatMessage(message);
                }
                catch (Exception exc)
                {
                    Debug.LogException(new Exception(string.Concat($"{mods[i].ModInfo.DisplayName} caused an exception at {nameof(OnTwitchChatMessage)}.\n", exc)));
                }
            }
        }
    }
}

