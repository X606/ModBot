using System;
using System.Collections.Generic;
using System.Text;
using InternalModBot;

namespace ModLibrary
{
    public static class UpgradeExtensionMethods
    {
        public static void AddUpgrade(this UpgradeManager upgradeManager, UpgradeDescription upgrade, Mod mod)
        {
            upgradeManager.UpgradeDescriptions.Add(upgrade);

            int page = UpgradePagesManager.GetPageForMod(mod);
            UpgradePagesManager.AddUpgradePage(upgrade.UpgradeType, upgrade.Level, page);

            if (upgrade.Requirement != null)
            {
                RecursivelyAddRequirments(upgrade, page);
            }
        }

        public static void AddUpgradeToModPage(this UpgradeManager upgradeManager, UpgradeDescription upgrade, Mod mod)
        {
            int page = UpgradePagesManager.GetPageForMod(mod);
            UpgradePagesManager.AddUpgradePage(upgrade.UpgradeType, upgrade.Level, page);
        }

        public static void AddUpgradeAlreadyInGame(this UpgradeManager upgradeManager, UpgradeDescription upgrade, Mod mod)
        {
            UpgradePagesManager.AddUpgradeAlreadyInGame(upgrade.UpgradeType, upgrade.Level, mod);
        }

        public static bool IsModdedUpgradeType(this UpgradeDescription upgrade)
        {
            return !ModTools.EnumTools.GetValues<UpgradeType>().Contains(upgrade.UpgradeType);
        }

        public static void SetSingleplayerCost(this UpgradeDescription upgradeDescription, int cost)
        {
            UpgradeCosts.SetCostOfUpgrade(upgradeDescription.UpgradeType, upgradeDescription.Level, cost);
        }

        public static int GetSinglePlayerCost(this UpgradeDescription upgradeDescription)
        {
            return UpgradeCosts.GetCostOfUpgrade(upgradeDescription.UpgradeType, upgradeDescription.Level);
        }

        private static void RecursivelyAddRequirments(UpgradeDescription upgrade, int page)
        {
            if (upgrade == null)
                return;

            UpgradePagesManager.AddUpgradePage(upgrade.UpgradeType, upgrade.Level, page);


            if (upgrade.Requirement2 != null)
            {
                RecursivelyAddRequirments(upgrade.Requirement2, page);
            }

            RecursivelyAddRequirments(upgrade.Requirement, page);
        }
    }
}
