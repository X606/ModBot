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
        /// Sets angle offset of this upgrade on the mod page
        /// </summary>
        /// <param name="upgradeDescription"></param>
        /// <param name="angle"></param>
        /// <param name="mod"></param>
        public static void SetAngleOffset(this UpgradeDescription upgradeDescription, float angle, Mod mod)
        {
            UpgradePagesManager.SetAngleOfModdedUpgrade(angle, upgradeDescription.UpgradeType, upgradeDescription.Level, mod);
        }

        /// <summary>
        /// Sets the icon of the upgrade to a image from a url, this needs a internet connection (NOTE: this has a cache so if you want to change picture you might want to remove the cache in the mods directory)
        /// </summary>
        /// <param name="upgradeDescription"></param>
        /// <param name="url">the url of the image you want to set the object to</param>
        public static void SetIconFromURL(this UpgradeDescription upgradeDescription, string url)
        {
            UpgradeIconDownloader.Instance.AddUpgradeIcon(upgradeDescription, url);
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
