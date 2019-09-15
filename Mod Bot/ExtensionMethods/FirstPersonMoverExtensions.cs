using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModLibrary
{
    /// <summary>
    /// Defines extension methods for <see cref="FirstPersonMover"/>
    /// </summary>
    public static class FirstPersonMoverExtensions
    {
        /// <summary>
        /// Returns the <see cref="FirstPersonMover"/>s currently equipped weapon, will return null if the <see cref="CharacterModel"/> is <see langword="null"/>, or the currently equipped <see cref="WeaponType"/> is <see cref="WeaponType.None"/>
        /// </summary>
        /// <param name="firstPersonMover"></param>
        /// <returns></returns>
        public static WeaponModel GetEquippedWeaponModel(this FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover.HasCharacterModel() || firstPersonMover.GetEquippedWeaponType() == WeaponType.None)
            {
                return null;
            }

            WeaponType equippedWeaponType = firstPersonMover.GetEquippedWeaponType();
            return firstPersonMover.GetCharacterModel().GetWeaponModel(equippedWeaponType);
        }

        /// <summary>
        /// Gives the specified upgrade to a <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="firstPersonMover"></param>
        /// <param name="upgradeType">The <see cref="UpgradeType"/> to give</param>
        /// <param name="level">The level of the upgrade</param>
        /// <exception cref="ArgumentNullException">If the given <see cref="FirstPersonMover"/> is <see langword="null"/></exception>
        public static void GiveUpgrade(this FirstPersonMover firstPersonMover, UpgradeType upgradeType, int level)
        {
            if (firstPersonMover == null)
            {
                throw new ArgumentNullException(nameof(firstPersonMover));
            }

            if (firstPersonMover.GetComponent<PreconfiguredUpgradeCollection>() != null)
            {
                UpgradeCollection upgradeCollection = firstPersonMover.GetComponent<UpgradeCollection>();

                if (upgradeCollection == null)
                {
                    debug.Log("Failed to give upgrade '" + upgradeType.ToString() + "' (Level: " + level + ") to " + firstPersonMover.CharacterName + " (UpgradeCollection is null)", Color.red);
                    return;
                }

                UpgradeTypeAndLevel upgradeToGive = new UpgradeTypeAndLevel { UpgradeType = upgradeType, Level = level };

                List<UpgradeTypeAndLevel> upgrades = ((PreconfiguredUpgradeCollection)upgradeCollection).Upgrades.ToList();

                upgrades.Add(upgradeToGive);

                ((PreconfiguredUpgradeCollection)upgradeCollection).Upgrades = upgrades.ToArray();
                ((PreconfiguredUpgradeCollection)upgradeCollection).InitializeUpgrades();

                firstPersonMover.RefreshUpgrades();
            }
            else if (firstPersonMover.GetComponent<PlayerUpgradeCollection>() != null)
            {
                GameDataManager.Instance.SetUpgradeLevel(upgradeType, level);
                UpgradeDescription upgrade = UpgradeManager.Instance.GetUpgrade(upgradeType, level);
                GlobalEventManager.Instance.Dispatch("UpgradeCompleted", upgrade);
            }

            firstPersonMover.SetUpgradesNeedsRefreshing();
        }

        /// <summary>
        /// Gives the specified <see cref="UpgradeDescription"/> to a <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="firstPersonMover"></param>
        /// <param name="Upgrade">The upgrade to give</param>
        public static void GiveUpgrade(this FirstPersonMover firstPersonMover, UpgradeDescription Upgrade)
        {
            firstPersonMover.GiveUpgrade(Upgrade.UpgradeType, Upgrade.Level);
        }
    }
}
