using ModLibrary;
using UnityEngine;

namespace InternalModBot
{
    public static class MethodsCalledFromInjections
    {
        public static int GetSkillPointCost(UpgradeDescription upgradeDescription)
        {
            if (GameModeManager.UsesBattleRoyaleUpgradeCosts() && upgradeDescription.SkillPointCostBattleRoyale > 0)
            {
                return upgradeDescription.SkillPointCostBattleRoyale;
            }
            if (GameModeManager.UsesMultiplayerUpgrades())
            {
                return upgradeDescription.SkillPointCostMultiplayer;
            }
            return UpgradeCosts.GetCostOfUpgrade(upgradeDescription.UpgradeType, upgradeDescription.Level);
        }

        public static bool IsUpgradeNotCurrentlyVisible(UpgradeDescription upgradeDescription)
        {
            if (!UpgradePagesMangaer.GetUpgradePages(upgradeDescription.UpgradeType, upgradeDescription.Level).Contains(UpgradePagesMangaer.currentPage))
            {
                return true;
            }

            return false;
        }

        public static void PassCharacterKilledInfoToMods(Character killed, Character killer, DamageSourceType damageSourceType)
        {
            GameObject killerGameObject = null;
            if (killer != null)
            {
                killerGameObject = killer.gameObject;
            }

            ModsManager.Instance.passOnMod.OnCharacterKilled(killed.gameObject, killerGameObject, damageSourceType);
        }

        public static void PassOnUpgradesRefreshedInfoToMods(FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover.IsAlive() || firstPersonMover.GetCharacterModel() == null)
            {
                return;
            }

            ModsManager.Instance.passOnMod.OnUpgradesRefreshed(firstPersonMover.gameObject, firstPersonMover.GetComponent<UpgradeCollection>());
        }

        public static void PassAfterUpgradesRefreshedInfoToMods(FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover.IsAlive() || firstPersonMover.GetCharacterModel() == null)
            {
                return;
            }

            ModsManager.Instance.passOnMod.AfterUpgradesRefreshed(firstPersonMover.gameObject, firstPersonMover.GetComponent<UpgradeCollection>());
        }
    }
}
