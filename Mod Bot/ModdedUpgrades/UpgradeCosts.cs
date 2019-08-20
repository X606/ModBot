using System;
using System.Collections.Generic;
using System.Text;

namespace InternalModBot
{
    public static class UpgradeCosts
    {
        public static int GetCostOfUpgrade(UpgradeType upgradeType, int level = 1)
        {
            if (!upgradeCostsDictionary.ContainsKey(new ModdedUpgradeTypeAndLevel(upgradeType, level)))
            {
                return 1;
            }

            return upgradeCostsDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)];
        }

        public static void SetCostOfUpgrade(UpgradeType upgradeType, int level, int newCost)
        {
            if (upgradeCostsDictionary.ContainsKey(new ModdedUpgradeTypeAndLevel(upgradeType, level)))
            {
                upgradeCostsDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)] = newCost;
                return;
            }

            upgradeCostsDictionary.Add(new ModdedUpgradeTypeAndLevel(upgradeType, level), newCost);
        }
        public static void Reset()
        {
            upgradeCostsDictionary = new Dictionary<ModdedUpgradeTypeAndLevel, int>();
        }

        private static Dictionary<ModdedUpgradeTypeAndLevel, int> upgradeCostsDictionary = new Dictionary<ModdedUpgradeTypeAndLevel, int>();
    }
}
