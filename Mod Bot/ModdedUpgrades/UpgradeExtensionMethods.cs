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
        /// Adds an upgrade to the page of the specified <see cref="Mod"/>, if the upgrade is a modded upgrade or not currently used it will also be added to <see cref="UpgradeManager.UpgradeDescriptions"/>
        /// </summary>
        /// <param name="upgradeManager"></param>
        /// <param name="upgrade">The <see cref="UpgradeDescription"/> of the upgrade to add</param>
        /// <param name="mod">The <see cref="Mod"/> that owns the upgrade</param>
        public static void AddUpgrade(this UpgradeManager upgradeManager, UpgradeDescription upgrade, Mod mod)
        {
            if (upgrade.IsModdedUpgradeType() || !UpgradeManager.Instance.IsUpgradeTypeAndLevelUsed(upgrade.UpgradeType, upgrade.Level))
                UpgradeManager.Instance.UpgradeDescriptions.Add(upgrade);

            UpgradePagesManager.AddUpgrade(upgrade.UpgradeType, upgrade.Level, mod.ModInfo.UniqueID);

            if (upgrade is AbilityUpgrade)
            {
                Dictionary<UpgradeType, bool> _abilityUpgradeTypes = UpgradeManager.Instance.GetPrivateField<Dictionary<UpgradeType, bool>>("_abilityUpgradeTypes");
                _abilityUpgradeTypes[upgrade.UpgradeType] = true;
            }

            if (upgrade.Requirement != null)
                recursivelyAddRequirments(upgrade, mod);

            string nameID = upgrade.UpgradeName.ToLower().Trim();
            ModBotLocalizationManager.TryAddModdedUpgradeLocalizationStringToDictionary(nameID, upgrade.UpgradeName);

            string descriptionID = upgrade.Description.ToLower().Trim();
            ModBotLocalizationManager.TryAddModdedUpgradeLocalizationStringToDictionary(descriptionID, upgrade.Description);
        }

        /// <summary>
        /// Sets the angle of an upgrade with the specified <see cref="UpgradeType"/> and level
        /// </summary>
        /// <param name="upgradeManager"></param>
        /// <param name="upgradeType">The <see cref="UpgradeType"/> of the <see cref="UpgradeDescription"/> to set the angle on</param>
        /// <param name="level">The level of the <see cref="UpgradeDescription"/> to set the angle on</param>
        /// <param name="angle">The new angle to set</param>
        /// <param name="mod">The <see cref="Mod"/> that owns the upgrade</param>
        public static void SetUpgradeAngle(this UpgradeManager upgradeManager, UpgradeType upgradeType, int level, float angle, Mod mod)
        {
            UpgradeDescription upgradeDescription = UpgradeManager.Instance.GetUpgrade(upgradeType, level);
            if (upgradeDescription == null)
                return;

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
            return !EnumTools.GetValues<UpgradeType>().Contains(upgradeType);
        }

        /// <summary>
        /// Sets angle offset of this upgrade on the mod page, NOTE: Needs to be run AFTER <see cref="UpgradeManager"/>.AddUpgrade(<see cref="UpgradeDescription"/>, <see cref="Mod"/>) is called
        /// </summary>
        /// <param name="upgradeDescription"></param>
        /// <param name="angle">The new angle of the <see cref="UpgradeDescription"/></param>
        /// <param name="mod">The <see cref="Mod"/> that owns the upgrade</param>
        public static void SetAngleOffset(this UpgradeDescription upgradeDescription, float angle, Mod mod)
        {
            // New mod loading system
            UpgradePagesManager.SetAngleOfModdedUpgrade(angle, upgradeDescription.UpgradeType, upgradeDescription.Level, mod.ModInfo.UniqueID);
        }

        /// <summary>
        /// Sets the icon of the upgrade to an image, this needs a internet connection (NOTE: This has a cache so if you want to change the icon you'll want to remove it from the cache in the data folder first)
        /// </summary>
        /// <param name="upgradeDescription"></param>
        /// <param name="url">The url to get the image from</param>
        public static void SetIconFromURL(this UpgradeDescription upgradeDescription, string url)
        {
            UpgradeIconDownloader.Instance.SetIconOnUpgrade(upgradeDescription, url);
        }

        static void recursivelyAddRequirments(UpgradeDescription upgrade, Mod mod)
        {
            if (upgrade == null)
                return;

            // New mod loading system
            UpgradePagesManager.AddUpgrade(upgrade.UpgradeType, upgrade.Level, mod.ModInfo.UniqueID);

            if (upgrade.Requirement2 != null)
                recursivelyAddRequirments(upgrade.Requirement2, mod);

            recursivelyAddRequirments(upgrade.Requirement, mod);
        }

        /// <summary>
        /// Enables setting the angles in the upgrade UI by scrolling on them and generating the code to set the angles again
        /// </summary>
        /// <param name="upgradeManager"></param>
        public static void EnterUpgradeIconAngleDebugMode(this UpgradeManager upgradeManager)
        {
            UpgradeAngleSetter.Instance.DebugModeEnabled = true;
        }
    }
}
