using System;
using System.Collections.Generic;
using System.Text;
using InternalModBot;

namespace ModLibrary
{
    /// <summary>
    /// Implements Extension methods for upgrades
    /// </summary>
    public static class UpgradeExtensionMethods
    {
        /// <summary>
        /// Adds a upgrade to the page of the specified <see cref="Mod"/>, if the upgrade is a modded upgrade or not currently used it will also be added to <see cref="UpgradeManager.UpgradeDescriptions"/>
        /// </summary>
        /// <param name="upgradeManager"></param>
        /// <param name="upgrade"></param>
        /// <param name="mod"></param>
        public static void AddUpgrade(this UpgradeManager upgradeManager, UpgradeDescription upgrade, Mod mod)
        {
            if (upgrade.IsModdedUpgradeType() || !UpgradeManager.Instance.IsUpgradeTypeAndLevelUsed(upgrade.UpgradeType, upgrade.Level))
            {
                upgradeManager.UpgradeDescriptions.Add(upgrade);
            }
            
            UpgradePagesManager.AddUpgrade(upgrade.UpgradeType, upgrade.Level, mod);

            if (upgrade is AbilityUpgrade)
            {
                Dictionary<UpgradeType, bool> _abilityUpgradeTypes = Accessor.GetPrivateField<UpgradeManager, Dictionary<UpgradeType, bool>>("_abilityUpgradeTypes", UpgradeManager.Instance);
                _abilityUpgradeTypes[upgrade.UpgradeType] = true;
            }

            if (upgrade.Requirement != null)
            {
                RecursivelyAddRequirments(upgrade, mod);
            }
        }

        /// <summary>
        /// Sets the angle of an upgrade with the specified <see cref="UpgradeType"/> and level
        /// </summary>
        /// <param name="upgradeManager"></param>
        /// <param name="upgradeType">The <see cref="UpgradeType"/> of the <see cref="UpgradeDescription"/> to set the angle on</param>
        /// <param name="level">The level of the <see cref="UpgradeDescription"/> to set the angle on</param>
        /// <param name="angle">The new angle to set</param>
        /// <param name="mod"></param>
        public static void SetUpgradeAngle(this UpgradeManager upgradeManager, UpgradeType upgradeType, int level, float angle, Mod mod)
        {
            UpgradeDescription upgradeDescription = UpgradeManager.Instance.GetUpgrade(upgradeType, level);
            if (upgradeDescription == null)
            {
                return;
            }

            upgradeDescription.SetAngleOffset(angle, mod);
        }

        /// <summary>
        /// Checks if the upgrade is a modded upgrade
        /// </summary>
        /// <param name="upgrade"></param>
        /// <returns></returns>
        public static bool IsModdedUpgradeType(this UpgradeDescription upgrade)
        {
            return upgrade.UpgradeType.IsModdedUpgradeType();
        }

        /// <summary>
        /// Checks if the <see cref="UpgradeType"/> is a modded type, by checking if the type is in the <see cref="UpgradeType"/> <see langword="enum"/>
        /// </summary>
        /// <param name="upgradeType"></param>
        /// <returns></returns>
        public static bool IsModdedUpgradeType(this UpgradeType upgradeType)
        {
            return !ModTools.EnumTools.GetValues<UpgradeType>().Contains(upgradeType);
        }

        /// <summary>
        /// Sets angle offset of this upgrade on the mod page, NOTE: Needs to be run AFTER <see cref="UpgradeManager"/>.AddUpgrade(<see cref="UpgradeDescription"/>, <see cref="Mod"/>) is called
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
            UpgradeIconDownloader.Instance.SetIconOnUpgrade(upgradeDescription, url);
        }

        private static void RecursivelyAddRequirments(UpgradeDescription upgrade, Mod mod)
        {
            if (upgrade == null)
            {
                return;
            }

            UpgradePagesManager.AddUpgrade(upgrade.UpgradeType, upgrade.Level, mod);


            if (upgrade.Requirement2 != null)
            {
                RecursivelyAddRequirments(upgrade.Requirement2, mod);
            }

            RecursivelyAddRequirments(upgrade.Requirement, mod);
        }

        /// <summary>
        /// Enables setting the angles in the upgrade UI by dragging the icons around
        /// </summary>
        /// <param name="upgradeManager"></param>
        public static void EnterUpgradeIconAngleDebugMode(this UpgradeManager upgradeManager)
        {
            UpgradeAngleSetter.Instance.DebugModeEnabled = true;
        }
    }
}
