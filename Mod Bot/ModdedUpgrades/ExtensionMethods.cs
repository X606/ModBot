using System;
using System.Collections.Generic;
using System.Text;
using InternalModBot;

namespace ModLibrary
{
    public static class UpgradeExtensionMethods
    {
        /// <summary>
        /// Adds a upgrade to the page of the specified mod, if the upgrade is a modded upgrade it will also be added to the upgradeManager.UpgradeDescriptions list
        /// </summary>
        /// <param name="upgradeManager"></param>
        /// <param name="upgrade"></param>
        /// <param name="mod"></param>
        public static void AddUpgrade(this UpgradeManager upgradeManager, UpgradeDescription upgrade, Mod mod)
        {
            if (UpgradePagesManager.IsModdedUpgradeType(upgrade.UpgradeType))
            {
                upgradeManager.UpgradeDescriptions.Add(upgrade);
            }
            
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
