using System;
using System.Collections.Generic;
using System.Text;
using InternalModBot;

namespace ModLibrary
{
    /// <summary>
    /// Used to implement Extension methods for upgrades, dont call these directly from this class.
    /// </summary>
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


        /// <summary>
        /// Checks if the upgrade is a modded upgrade
        /// </summary>
        /// <param name="upgrade"></param>
        /// <returns></returns>
        public static bool IsModdedUpgradeType(this UpgradeDescription upgrade)
        {
            return !ModTools.EnumTools.GetValues<UpgradeType>().Contains(upgrade.UpgradeType);
        }

        /// <summary>
        /// Sets the cost of the upgrade in singleplayer (Endless, Twichmode, storymode ect)
        /// </summary>
        /// <param name="upgradeDescription"></param>
        /// <param name="cost"></param>
        /// <param name="mod"></param>
        public static void SetSingleplayerCost(this UpgradeDescription upgradeDescription, int cost, Mod mod)
        {
            UpgradeCosts.SetCostOfUpgrade(upgradeDescription.UpgradeType, upgradeDescription.Level, cost, mod);
        }

        /// <summary>
        /// Gets the cost set by SetSingleplayerCost
        /// </summary>
        /// <param name="upgradeDescription"></param>
        /// <returns></returns>
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
