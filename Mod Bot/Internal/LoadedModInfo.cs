﻿/*
 * This class is only used in the new mod loading system,
 * and since that system isn't done yet I am reverting back to the old system
 * 
 */


/*
using HarmonyLib;
using ModLibrary;
using System;
using UnityEngine;

namespace InternalModBot
{
    /// <summary>
    /// Stores all necessary data about a loaded mod
    /// </summary>
    internal class LoadedModInfo
    {
        /// <summary>
        /// Sets the mod field to the passed mod, and will not deactivate the mod
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="modInfo"></param>
        internal LoadedModInfo(Mod mod, ModInfo modInfo)
        {
            ModReference = mod;
            OwnerModInfo = modInfo;
        }

        internal Mod ModReference;

        internal readonly ModInfo OwnerModInfo;

        public bool IsEnabled
        {
            get
            {
				return OwnerModInfo.IsModEnabled;
            }
            internal set
            {
                if (IsEnabled == value) // If the mod's state hasn't changed
                    return;

                PlayerPrefs.SetInt(OwnerModInfo.UniqueID, value ? 1 : 0);

                if (value) // If the mod is being enabled
                {
					if (ModReference == null)
					{
						ModsManager.Instance.LoadMod(OwnerModInfo);

					} else
					{
						ModReference.OnModEnabled();
					}

						
				}
                else // If the mod is being disabled
                {
                    CustomUpgradeManager.NextClicked();
                    UpgradePagesManager.RemoveModdedUpgradesFor(OwnerModInfo.UniqueID);

                    new Harmony(ModReference.HarmonyID).UnpatchAll(ModReference.HarmonyID); // unpatches all of the patches made by the mod

                    ModReference.OnModDeactivated();
                }

				ModsManager.Instance.RefreshAllLoadedActiveMods();
            }
        }

    }
}
*/