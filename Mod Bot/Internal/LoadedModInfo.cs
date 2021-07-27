using HarmonyLib;
using ModLibrary;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

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


                    AutoInject();
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

        public void AutoInject()
        {
            new Harmony(ModReference.HarmonyID).UnpatchAll(ModReference.HarmonyID); // unpatches all of the patches made by the mod first

            Harmony harmony = new Harmony(ModReference.HarmonyID);
            KeyValuePair<InjectionTargetAttribute, MethodInfo>[] injections = InjectionTargetAttribute.GetInjectionTargetsInAssembly(ModReference.GetType().Assembly);
            foreach (KeyValuePair<InjectionTargetAttribute, MethodInfo> injection in injections)
            {
                HarmonyMethod prefix = null;
                if (injection.Key.InjectionType == InjectionType.Prefix)
                    prefix = new HarmonyMethod(injection.Value);

                HarmonyMethod postfix = null;
                if (injection.Key.InjectionType == InjectionType.Postfix)
                    postfix = new HarmonyMethod(injection.Value);

                harmony.Patch(injection.Key.SelectedMethod, prefix, postfix);
            }
        }

    }
}