using System;
using System.Collections.Generic;
using System.Text;
using InternalModBot;
using ModLibrary;

namespace InternalModBot
{
    public static class CalledFromInjections
    {
        public static void FromRefreshUpgradesStart(FirstPersonMover owner)
        {
            ModsManager.Instance.PassOnMod.OnUpgradesRefreshed(owner, owner.GetComponent<UpgradeCollection>());
        }
        public static void FromRefreshUpgradesEnd(FirstPersonMover owner)
        {
            ModsManager.Instance.PassOnMod.AfterUpgradesRefreshed(owner, owner.GetComponent<UpgradeCollection>());
        }

        public static void FromOnCharacterStart(Character owner)
        {
            ModsManager.Instance.PassOnMod.OnCharacterSpawned(owner);
        }
        public static void FromOnCharacterUpdate(Character owner)
        {
            ModsManager.Instance.PassOnMod.OnCharacterUpdate(owner);
        }
        public static void FromOnCharacterDeath(Character owner, Character killer, DamageSourceType damageSource)
        {
            ModsManager.Instance.PassOnMod.OnCharacterKilled(owner, killer, damageSource);
        }

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
        public static bool FromIsUpgradeCurrentlyVisible(UpgradeDescription upgrade)
        {
            if (!UpgradePagesMangaer.GetUpgradePages(upgrade.UpgradeType, upgrade.Level).Contains(UpgradePagesMangaer.currentPage))
            {
                return false;
            }
            return (GameModeManager.ShowsStoryBlockedUpgrades() || (!UpgradeManager.Instance.IsUpgradeLockedByCurrentMetagameProgress(upgrade) && !upgrade.HideInStoryMode)) && (!GameModeManager.UsesMultiplayerUpgrades() || upgrade.IsAvailableInMultiplayer) && (!GameModeManager.IsBattleRoyale() || !upgrade.IsDisabledInBattleRoyale) && upgrade.IsUpgradeVisible && upgrade.IsCompatibleWithCharacter(CharacterTracker.Instance.GetPlayer());
        }
        public static bool FromIsRepairUpgradeCurrentlyVisible(UpgradeDescription upgrade)
        {
            if (!UpgradePagesMangaer.GetUpgradePages(upgrade.UpgradeType, upgrade.Level).Contains(UpgradePagesMangaer.currentPage))
            {
                return false;
            }
            if (!upgrade.IsUpgradeVisible)
            {
                return false;
            }
            if (GameModeManager.Is(GameMode.LevelEditor))
            {
                return true;
            }
            FirstPersonMover player = Singleton<CharacterTracker>.Instance.GetPlayer();
            return !(player == null) && player.HasLimbDamage();
        }

        public static void FromCreateInactiveArrow(ArrowProjectile arrow)
        {
            ModsManager.Instance.PassOnMod.OnProjectileCreated(arrow);
        }
    }
}
