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
            
            UpgradePagesManager.AddUpgrade(upgrade.UpgradeType, upgrade.Level, mod);

            if (upgrade.Requirement != null)
            {
                RecursivelyAddRequirments(upgrade, mod);
            }
        }

        public static void AddUpgradeAlreadyInGame(this UpgradeManager upgradeManager, UpgradeDescription upgrade, Mod mod)
        {
            UpgradePagesManager.AddUpgrade(upgrade.UpgradeType, upgrade.Level, mod);

            if (upgrade.Requirement != null)
            {
                RecursivelyAddRequirments(upgrade, mod);
            }
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

        private static void RecursivelyAddRequirments(UpgradeDescription upgrade, Mod mod)
        {
            if (upgrade == null)
                return;

            UpgradePagesManager.AddUpgrade(upgrade.UpgradeType, upgrade.Level, mod);


            if (upgrade.Requirement2 != null)
            {
                RecursivelyAddRequirments(upgrade.Requirement2, mod);
            }

            RecursivelyAddRequirments(upgrade.Requirement, mod);
        }
    }
}
