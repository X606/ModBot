using InternalModBot;
using ModLibrary;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

namespace InternalModBot
{
    /// <summary>
    /// Contains methods that get called from the game itself
    /// </summary>
    public static class CalledFromInjections
    {
        /// <summary>
        /// Called at the start of <see cref="FirstPersonMover.RefreshUpgrades"/>
        /// </summary>
        /// <param name="owner"></param>
        public static void FromRefreshUpgradesStart(FirstPersonMover owner)
        {
            if (owner == null || owner.gameObject == null || !owner.IsAlive() || owner.GetCharacterModel() == null)
            {
                return;
            }

            UpgradeCollection upgrade = owner.GetComponent<UpgradeCollection>();
            ModsManager.Instance.PassOnMod.OnUpgradesRefreshed(owner, upgrade);
        }

        /// <summary>
        /// Called at the end of <see cref="FirstPersonMover.RefreshUpgrades"/>
        /// </summary>
        /// <param name="owner"></param>
        public static void FromRefreshUpgradesEnd(FirstPersonMover owner)
        {
            if (owner.gameObject == null)
            {
                return;
            }

            ModsManager.Instance.PassOnMod.AfterUpgradesRefreshed(owner, owner.GetComponent<UpgradeCollection>());
        }

        /// <summary>
        /// Called from <see cref="Character.Start"/>
        /// </summary>
        /// <param name="owner"></param>
        public static void FromOnCharacterStart(Character owner)
        {
            ModsManager.Instance.PassOnMod.OnCharacterSpawned(owner);
        }

        /// <summary>
        /// Called from <see cref="Character.Update"/>
        /// </summary>
        /// <param name="owner"></param>
        public static void FromOnCharacterUpdate(Character owner)
        {
            ModsManager.Instance.PassOnMod.OnCharacterUpdate(owner);
        }

        /// <summary>
        /// Called from <see cref="Character.onDeath(Character, DamageSourceType)"/>
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="killer"></param>
        /// <param name="damageSource"></param>
        public static void FromOnCharacterDeath(Character owner, Character killer, DamageSourceType damageSource)
        {
            ModsManager.Instance.PassOnMod.OnCharacterKilled(owner, killer, damageSource);
        }

        /// <summary>
        /// Called from <see cref="UpgradeDescription.GetAngleOffset"/> and returns the angle the upgrade should be on the current page
        /// </summary>
        /// <param name="upgrade"></param>
        /// <returns></returns>
        public static float FromGetAngleOffset(UpgradeDescription upgrade)
        {
            return UpgradePagesManager.GetAngleOfUpgrade(upgrade.UpgradeType, upgrade.Level);
        }

        /// <summary>
        /// Called from <see cref="UpgradeDescription.IsUpgradeCurrentlyVisible"/> and returns if the upgrade should be visible on the current page
        /// </summary>
        /// <param name="upgrade"></param>
        /// <returns></returns>
        public static bool FromIsUpgradeCurrentlyVisible(UpgradeDescription upgrade)
        {
            if (!UpgradePagesManager.IsUpgradeVisible(upgrade.UpgradeType, upgrade.Level))
            {
                return false;
            }
            if (UpgradePagesManager.ForceUpgradeVisible(upgrade.UpgradeType, upgrade.Level))
            {
                return true;
            }

            return (GameModeManager.ShowsStoryBlockedUpgrades() || (!Singleton<UpgradeManager>.Instance.IsUpgradeLockedByCurrentMetagameProgress(upgrade) && !upgrade.HideInStoryMode)) && (!GameModeManager.IsMultiplayerDuel() || upgrade.IsAvailableInDuels) && (!GameModeManager.IsBattleRoyale() || upgrade.IsAvailableInBattleRoyale) && (!GameModeManager.IsEndlessCoop() || upgrade.IsAvailableInCoop) && (upgrade.IsUpgradeVisible || GameModeManager.IsMultiplayer()) && upgrade.IsCompatibleWithCharacter(Singleton<CharacterTracker>.Instance.GetPlayer());
        }

        /// <summary>
        /// Called from <see cref="Projectile.FixedUpdate"/>
        /// </summary>
        /// <param name="projectile"></param>
        public static void FromFixedUpdate(Projectile projectile)
        {
            if (!projectile.IsFlying())
            {
                return;
            }

            ModsManager.Instance.PassOnMod.OnProjectileUpdate(projectile);
        }

        /// <summary>
        /// Called from <see cref="Projectile.StartFlying(Vector3, Vector3, bool, Character)"/>
        /// </summary>
        /// <param name="arrow"></param>
        public static void FromStartFlying(Projectile arrow)
        {
            ModsManager.Instance.PassOnMod.OnProjectileStartedMoving(arrow);
        }

        /// <summary>
        /// Called from <see cref="Projectile.DestroyProjectile"/>
        /// </summary>
        /// <param name="arrow"></param>
        public static void FromDestroyProjectile(Projectile arrow)
        {
            ModsManager.Instance.PassOnMod.OnProjectileDestroyed(arrow);
        }

        /// <summary>
        /// Gets from <see cref="Projectile.OnEnvironmentCollided(bool)"/>
        /// </summary>
        /// <param name="arrow"></param>
        public static void FromOnEnvironmentCollided(Projectile arrow)
        {
            if (arrow.PassThroughEnvironment)
            {
                return;
            }
            if (!arrow.IsFlying())
            {
                return;
            }
            if (arrow.MainCollider != null)
            {
                arrow.MainCollider.enabled = false;
            }

            ModsManager.Instance.PassOnMod.OnProjectileDestroyed(arrow);
        }

        /// <summary>
        /// Called from <see cref="GameUIRoot.RefreshCursorEnabled"/> and returns if the cursor has been disabled by a mod
        /// </summary>
        /// <returns></returns>
        public static bool FromRefreshCursorEnabled()
        {
            if (RegisterShouldCursorBeEnabledDelegate.ShouldMouseBeEnabled())
            {
                return true;
            }

            if (ModsManager.Instance == null)
                return true;
            
            if (ModsManager.Instance.PassOnMod.ShouldCursorBeEnabled())
            {
                InputManager.Instance.SetCursorEnabled(true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called from <see cref="MortarWalker.GetPositionForAIToAimAt"/>
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static Vector3 FromGetPositionForAIToAimAt(Character character)
        {
            List<MechBodyPart> powerCrystals = character.GetBodyParts(MechBodyPartType.PowerCrystal);
            if (powerCrystals.Count == 0)
            {
                return character.transform.position;
            }

            return powerCrystals[0].transform.position;
        }

        /// <summary>
        /// Called from <see cref="Resources.Load(string)"/>, <see cref="Resources.Load{T}(string)"/> and <see cref="ResourceRequest.asset"/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static UnityEngine.Object FromResourcesLoad(string path)
        {
            UnityEngine.Object obj = LevelEditorObjectAdder.GetObjectData(path);
            if (obj != null)
            {
                return obj;
            }

            if(ModsManager.Instance == null)
                return null;
            return ModsManager.Instance.PassOnMod.OnResourcesLoad(path);
        }

    }

}
