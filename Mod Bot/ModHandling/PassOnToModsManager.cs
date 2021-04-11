using ModLibrary;
using System.Collections.Generic;
using UnityEngine;
using System;
using HarmonyLib;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to call events on all loaded active mods, you probably dont want to use this from mods
    /// </summary>
    public class PassOnToModsManager : IMod
    {
        string IMod.HarmonyID => throw new NotImplementedException();

        Harmony IMod.HarmonyInstance => throw new NotImplementedException();

        ModInfo IMod.ModInfo => throw new NotImplementedException();

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="me"></param>
        public void OnFirstPersonMoverSpawned(FirstPersonMover me)
        {
            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnFirstPersonMoverSpawned(me);
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="me"></param>
        public void OnFirstPersonMoverUpdate(FirstPersonMover me)
        {
            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnFirstPersonMoverUpdate(me);
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        public void OnModRefreshed()
        {
            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnModRefreshed();
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        public void OnLevelEditorStarted()
        {
            LevelEditorObjectAdder.OnLevelEditorStarted();
            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnLevelEditorStarted();
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="command"></param>
        public void OnCommandRan(string command)
        {
            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCommandRan(command);
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="me"></param>
        /// <param name="upgrades"></param>
        public void OnUpgradesRefreshed(FirstPersonMover me, UpgradeCollection upgrades)
        {
            FirstPersonMover firstPersonMover = me.GetComponent<FirstPersonMover>();
            if (!firstPersonMover.IsAlive() || firstPersonMover.GetCharacterModel() == null)
                return;

            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnUpgradesRefreshed(me, upgrades);
            }
        }

        /// <summary>
        /// Calls this method on all mods, also calls OnFirstPersonMoverSpawned if the passed character is a FirstPersonMover
        /// </summary>
        /// <param name="me"></param>
        public void OnCharacterSpawned(Character me)
        {
            if (me.GetComponent<Character>() is FirstPersonMover)
                OnFirstPersonMoverSpawned(me as FirstPersonMover);

            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCharacterSpawned(me);
            }
        }

        /// <summary>
        /// Calls this method on all mods, also calls OnFirstPersonMoverUpdate if the passed character is a firstpersonmover
        /// </summary>
        /// <param name="me"></param>
        public void OnCharacterUpdate(Character me)
        {
            if (me.GetComponent<Character>() is FirstPersonMover)
                OnFirstPersonMoverUpdate(me as FirstPersonMover);

            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCharacterUpdate(me);
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
        public void AfterUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].AfterUpgradesRefreshed(owner, upgrades);
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="killedCharacter"></param>
        /// <param name="killerCharacter"></param>
        /// <param name="damageSourceType"></param>
        /// <param name="attackID"></param>
        public void OnCharacterKilled(Character killedCharacter, Character killerCharacter, DamageSourceType damageSourceType, int attackID)
        {
            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCharacterKilled(killedCharacter, killerCharacter, damageSourceType, attackID);
            }
        }

        /// <summary>
        /// Gets the response from this from all loaded mods, and uses the or operator on all of them, then returns
        /// </summary>
        /// <returns></returns>
        public bool ShouldCursorBeEnabled() // if any mod tells the game that the cursor should be enabled, it will be
        {
            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            foreach(IMod mod in mods)
            {
                if (mod.ShouldCursorBeEnabled())
                    return true;
            }

            return Generic2ButtonDialogue.IsWindowOpen;
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        public void GlobalUpdate()
        {
            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].GlobalUpdate();
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="moddedEvent"></param>
        public void OnMultiplayerEventReceived(GenericStringForModdingEvent moddedEvent)
        {
            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnMultiplayerEventReceived(moddedEvent);
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public UnityEngine.Object OnResourcesLoad(string path)
        {
            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for(int i = 0; i < mods.Count; i++)
            {
                UnityEngine.Object obj = mods[i].OnResourcesLoad(path);
                if(obj != null)
                    return obj;
            }

            return null;
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="newLanguageID"></param>
        /// <param name="localizationDictionary"></param>
        public void OnLanguageChanged(string newLanguageID, Dictionary<string, string> localizationDictionary)
        {
            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnLanguageChanged(newLanguageID, localizationDictionary);
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        public void OnClientConnectedToServer()
		{
			List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
			for(int i = 0; i < mods.Count; i++)
			{
				mods[i].OnClientConnectedToServer();
			}
		}

		/// <summary>
		/// Calls this method on all mods
		/// </summary>
		public void OnClientDisconnectedFromServer()
		{
			List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
			for(int i = 0; i < mods.Count; i++)
			{
				mods[i].OnClientDisconnectedFromServer();
			}
		}

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        public void OnCharacterModelCreated(FirstPersonMover owner)
        {
            List<IMod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCharacterModelCreated(owner);
            }
        }

        bool IMod.ImplementsSettingsWindow()
        {
            throw new NotImplementedException();
        }

        void IMod.CreateSettingsWindow(ModOptionsWindowBuilder builder)
        {
            throw new NotImplementedException();
        }

        void IMod.OnModEnabled()
        {
            throw new NotImplementedException();
        }

        void IMod.OnModLoaded()
        {
            throw new NotImplementedException();
        }

        void IMod.OnModDeactivated()
        {
            throw new NotImplementedException();
        }
    }
}
