using System;
using System.Collections.Generic;
using System.Text;
using ModLibrary;

namespace InternalModBot
{
    /// <summary>
    /// Used to manage the cost of upgrades in singleplayer since doborog doesnt have a built in system to do this.
    /// </summary>
    public static class UpgradeCosts
    {
        /// <summary>
        /// Gets the cost of the inputed upgrade type and level acording to the system.
        /// </summary>
        /// <param name="upgradeType"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int GetCostOfUpgrade(UpgradeType upgradeType, int level = 1)
        {
            if (!upgradeCostsDictionary.ContainsKey(new ModdedUpgradeTypeAndLevel(upgradeType, level)))
            {
                return 1;
            }

            return upgradeCostsDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)].SecondValue;
        }

        /// <summary>
        /// Sets the cost of the upgrade in the system
        /// </summary>
        /// <param name="upgradeType"></param>
        /// <param name="level"></param>
        /// <param name="newCost"></param>
        /// <param name="mod">The mod you set this from</param>
        public static void SetCostOfUpgrade(UpgradeType upgradeType, int level, int newCost, Mod mod)
        {
            if (upgradeCostsDictionary.ContainsKey(new ModdedUpgradeTypeAndLevel(upgradeType, level)))
            {
                upgradeCostsDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)] = new DoubleValueHolder<Mod, int>(mod, newCost);
                return;
            }

            upgradeCostsDictionary.Add(new ModdedUpgradeTypeAndLevel(upgradeType, level), new DoubleValueHolder<Mod, int>(mod, newCost));
        }
        /// <summary>
        /// Removes all set custom upgrade costs
        /// </summary>
        public static void Reset()
        {
            upgradeCostsDictionary = new Dictionary<ModdedUpgradeTypeAndLevel, DoubleValueHolder<Mod, int>>();
        }

        private static Dictionary<ModdedUpgradeTypeAndLevel, DoubleValueHolder<Mod, int>> upgradeCostsDictionary = new Dictionary<ModdedUpgradeTypeAndLevel, DoubleValueHolder<Mod, int>>();
    }
}
