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
        private LoadedModInfo() // this will prevent people from creating new LoadedModInfo instances outside of Mod-Bot
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the mod field to the passed mod, and will not deactivate the mod
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="modInfo"></param>
        /// <param name="appDomain"></param>
        internal LoadedModInfo(Mod mod, ModInfo modInfo, AppDomain appDomain)
        {
            _modReference = mod;
            OwnerModInfo = modInfo;
            AppDomain = appDomain;
        }

        readonly Mod _modReference;

        internal readonly ModInfo OwnerModInfo;

        internal readonly AppDomain AppDomain;

        public bool IsEnabled
        {
            get
            {
                return PlayerPrefs.GetInt(OwnerModInfo.UniqueID, 1) == 1;
            }
            internal set
            {
                if (IsEnabled == value) // If the mod's state hasn't changed
                    return;

                PlayerPrefs.SetInt(OwnerModInfo.UniqueID, value ? 1 : 0);

                if (value) // If the mod is being enabled
                {
                    _modReference.OnModEnabled();
                }
                else // If the mod is being disabled
                {
                    CustomUpgradeManager.NextClicked();
                    UpgradePagesManager.RemoveModdedUpgradesFor(_modReference);

                    new Harmony(_modReference.HarmonyID).UnpatchAll(_modReference.HarmonyID); // unpatches all of the patches made by the mod

                    _modReference.OnModDeactivated();
                }
            }
        }

        internal void Unload()
        {
            AppDomain.Unload(AppDomain);
        }
    }
}