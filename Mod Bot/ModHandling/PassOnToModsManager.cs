using ModLibrary;
using System.Collections.Generic;
using UnityEngine;
using System;
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
        public override void OnFirstPersonMoverSpawned(FirstPersonMover me)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnFirstPersonMoverSpawned(me);
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="me"></param>
        public override void OnFirstPersonMoverUpdate(FirstPersonMover me)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnFirstPersonMoverUpdate(me);
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        public override void OnModRefreshed()
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnModRefreshed();
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        public override void OnLevelEditorStarted()
        {
            LevelEditorObjectAdder.OnLevelEditorStarted();
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnLevelEditorStarted();
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="command"></param>
        public override void OnCommandRan(string command)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
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
        public override void OnUpgradesRefreshed(FirstPersonMover me, UpgradeCollection upgrades)
        {
            FirstPersonMover firstPersonMover = me.GetComponent<FirstPersonMover>();
            if (!firstPersonMover.IsAlive() || firstPersonMover.GetCharacterModel() == null)
            {
                return;
            }

            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnUpgradesRefreshed(me, upgrades);
            }
        }

        /// <summary>
        /// Calls this method on all mods, also calls OnFirstPersonMoverSpawned if the passed character is a FirstPersonMover
        /// </summary>
        /// <param name="me"></param>
        public override void OnCharacterSpawned(Character me)
        {
            if (me.GetComponent<Character>() is FirstPersonMover)
            {
                OnFirstPersonMoverSpawned(me as FirstPersonMover);
            }

            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCharacterSpawned(me);
            }
        }

        /// <summary>
        /// Calls this method on all mods, also calls OnFirstPersonMoverUpdate if the passed character is a firstpersonmover
        /// </summary>
        /// <param name="me"></param>
        public override void OnCharacterUpdate(Character me)
        {
            if (me.GetComponent<Character>() is FirstPersonMover)
            {
                OnFirstPersonMoverUpdate(me as FirstPersonMover);
            }

            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCharacterUpdate(me);
            }
        }

        /// <summary>
        /// Moved from <see cref="CalledFromInjections"/>, checks for <see langword="null"/> and calls <see cref="AfterUpgradesRefreshed(FirstPersonMover, UpgradeCollection)"/>
        /// </summary>
        /// <param name="firstPersonMover"></param>
        public static void AfterUpgradesRefreshed(FirstPersonMover firstPersonMover)
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
        public override void AfterUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
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
        public override void OnCharacterKilled(Character killedCharacter, Character killerCharacter, DamageSourceType damageSourceType, int attackID)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnCharacterKilled(killedCharacter, killerCharacter, damageSourceType, attackID);
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        public override void OnModDeactivated()
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnModDeactivated();
            }
        }

        /// <summary>
        /// Gets the response from this from all loaded mods, and uses the or operator on all of them, then returns
        /// </summary>
        /// <returns></returns>
        public override bool ShouldCursorBeEnabled() // if any mod tells the game that the cursor should be enabled, it will be
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            foreach(Mod mod in mods)
            {
                if (mod.ShouldCursorBeEnabled())
                    return true;
            }

            return Generic2ButtonDialogue.IsWindowOpen;
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        public override void GlobalUpdate()
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].GlobalUpdate();
            }
        }

        /// <summary>
        /// Calls this method on all mods
        /// </summary>
        /// <param name="moddedEvent"></param>
        public override void OnMultiplayerEventReceived(GenericStringForModdingEvent moddedEvent)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
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
        public override UnityEngine.Object OnResourcesLoad(string path)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
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
        public override void OnLanguageChanged(string newLanguageID, Dictionary<string, string> localizationDictionary)
        {
            List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
            for (int i = 0; i < mods.Count; i++)
            {
                mods[i].OnLanguageChanged(newLanguageID, localizationDictionary);
            }
        }

		/// <summary>
		/// Calls this method on all mods
		/// </summary>
		public override void OnClientConnectedToServer()
		{
			List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
			for(int i = 0; i < mods.Count; i++)
			{
				mods[i].OnClientConnectedToServer();
			}
		}
		/// <summary>
		/// Calls this method on all mods
		/// </summary>
		public override void OnClientDisconnectedToServer()
		{
			List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();
			for(int i = 0; i < mods.Count; i++)
			{
				mods[i].OnClientDisconnectedToServer();
			}
		}

	}
}