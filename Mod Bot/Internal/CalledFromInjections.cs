using System;
using System.Collections.Generic;
using System.Text;
using InternalModBot;
using ModLibrary;
using UnityEngine;

namespace InternalModBot
{
    /// <summary>
    /// Contains a lot of methods that get called from injections into the game itself
    /// </summary>
    public static class CalledFromInjections
    {
        /// <summary>
        /// Called From RefreshUpgrade at the start of the method
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
        /// Called after upgrades get refreshed
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
        /// Gets called in character Start
        /// </summary>
        /// <param name="owner"></param>
        public static void FromOnCharacterStart(Character owner)
        {
            ModsManager.Instance.PassOnMod.OnCharacterSpawned(owner);
        }
        /// <summary>
        /// Gets called in character Update
        /// </summary>
        /// <param name="owner"></param>
        public static void FromOnCharacterUpdate(Character owner)
        {
            ModsManager.Instance.PassOnMod.OnCharacterUpdate(owner);
        }
        /// <summary>
        /// Called from OnDeath in Character
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="killer"></param>
        /// <param name="damageSource"></param>
        public static void FromOnCharacterDeath(Character owner, Character killer, DamageSourceType damageSource)
        {
            ModsManager.Instance.PassOnMod.OnCharacterKilled(owner, killer, damageSource);
        }
        /// <summary>
        /// Called from GetSkillPointCost and the number this returns will be the cost of the upgrade
        /// </summary>
        /// <param name="upgrade"></param>
        /// <returns></returns>
        public static int FromGetSkillPointCost(UpgradeDescription upgrade)
        {
            if (GameModeManager.UsesBattleRoyaleUpgradeCosts() && upgrade.SkillPointCostBattleRoyale > 0)
            {
                return upgrade.SkillPointCostBattleRoyale;
            }
            if (GameModeManager.UsesMultiplayerUpgrades())
            {
                return upgrade.SkillPointCostMultiplayer;
            }
            return UpgradeCosts.GetCostOfUpgrade(upgrade.UpgradeType, upgrade.Level);
        }
        /// <summary>
        /// Called from IsUpgradeCurrentlyVisible and if this returns false the upgrade will not be displayed, and if it returns true it will
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
        /// Called when a projectile is created
        /// </summary>
        /// <param name="arrow"></param>
        public static void FromSetInactive(Projectile arrow)
        {
            ModsManager.Instance.PassOnMod.OnProjectileCreated(arrow);
        }
        /// <summary>
        /// Called when a arrow starts flying
        /// </summary>
        /// <param name="arrow"></param>
        public static void FromStartFlying(Projectile arrow)
        {
            ModsManager.Instance.PassOnMod.OnProjectileStartedMoving(arrow);
        }
        /// <summary>
        /// Called when a projectile gets destroyed
        /// </summary>
        /// <param name="arrow"></param>
        public static void FromDestroyProjectile(Projectile arrow)
        {
            ModsManager.Instance.PassOnMod.OnProjectileDestroyed(arrow);
        }
        /// <summary>
        /// Gets called when a projectile collides with the enviorment
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
        /// If this method returns true the cursor will be enabled and the player will not be able to move
        /// </summary>
        /// <returns></returns>
        public static bool FromRefreshCursorEnabled()
        {
            bool shouldCursorBeEnabled = ModsManager.Instance.PassOnMod.ShouldCursorBeEnabled();

            if (shouldCursorBeEnabled)
            {
                InputManager.Instance.SetCursorEnabled(true);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Gets called from character GetPositionForAIToAimAt and is used to fix people aiming at spidertrons
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static Vector3 FromGetPositionForAIToAimAt(Character me)
        {
            List<MechBodyPart> powerCrystals = me.GetBodyParts(MechBodyPartType.PowerCrystal);
            if (powerCrystals.Count == 0)
            {
                return me.transform.position;
            }
            return powerCrystals[0].transform.position;
        }
    }
    /// <summary>
    /// used to fix spidertrons
    /// </summary>
    public class FixSpidertrons : Character
    {
        /// <summary>
        /// used by the injector to copy the msil from and paste it into the real function
        /// </summary>
        /// <returns></returns>
        public override Vector3 GetPositionForAIToAimAt()
        {
            return CalledFromInjections.FromGetPositionForAIToAimAt(this);
        }
    }

   
}
