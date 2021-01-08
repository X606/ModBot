// New mod loading system
using System;
using System.Collections.Generic;
using System.Text;
using ModLibrary;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to keep track of what modded upgrades are on what page
    /// </summary>
    public static class UpgradePagesManager
    {
        static List<KeyValuePair<string, List<ModdedUpgradeRepresenter>>> _allModdedUpgradePages = new List<KeyValuePair<string, List<ModdedUpgradeRepresenter>>>();

        /// <summary>
        /// The page that is currently selected
        /// </summary>
        public static int CurrentPage = 0;
        
        /// <summary>
        /// Removes all modded upgrades and sets the current page to 0
        /// </summary>
        public static void Reset()
        {
            for (int i = 0; i < UpgradeManager.Instance.UpgradeDescriptions.Count; i++)
            {
                if (UpgradeManager.Instance.UpgradeDescriptions[i].IsModdedUpgradeType())
                {
                    UpgradeManager.Instance.UpgradeDescriptions.RemoveAt(i);
                    i--;
                }
            } // Removes all modded upgrades from the UpgradeDescriptions List

            _allModdedUpgradePages.Clear();
            CurrentPage = 0;
        }

        /// <summary>
        /// Removes all of the upgrades in <see cref="UpgradeManager.UpgradeDescriptions"/> placed there by a mod 
        /// </summary>
        /// <param name="modID"></param>
        public static void RemoveModdedUpgradesFor(string modID)
        {
            for (int i = 0; i < _allModdedUpgradePages.Count; i++)
            {
                KeyValuePair<string, List<ModdedUpgradeRepresenter>> item = _allModdedUpgradePages[i];
                if (item.Key == modID)
                {
                    foreach (ModdedUpgradeRepresenter upgradeType in item.Value)
                    {
                        for (int o = 0; o < UpgradeManager.Instance.UpgradeDescriptions.Count; o++)
                        {
                            if (UpgradeManager.Instance.UpgradeDescriptions[o].UpgradeType == upgradeType.UpgradeType && UpgradeManager.Instance.UpgradeDescriptions[o].Level == upgradeType.Level && IsModdedUpgradeType(upgradeType.UpgradeType))
                            {
                                UpgradeManager.Instance.UpgradeDescriptions.RemoveAt(o);
                                o--;
                            }
                        }
                    }

                    _allModdedUpgradePages[i] = new KeyValuePair<string, List<ModdedUpgradeRepresenter>>(modID, new List<ModdedUpgradeRepresenter>());

                    return;
                }
            }
        }

        /// <summary>
        /// Adds an upgrade to keep track of, this upgrade will be displayed on the page of the mod passed
        /// </summary>
        /// <param name="upgradeType"></param>
        /// <param name="level"></param>
        /// <param name="mod"></param>
        public static void AddUpgrade(UpgradeType upgradeType, int level, string modID)
        {
            if (modAlreadyHasUpgrade(modID, new ModdedUpgradeRepresenter(upgradeType, level))) // If the mod has already defined the upgrade on its page
                return;

            TryAddPage(modID);
            var pages = GenerateModPages();
            int page = GetPageForMod(pages, modID);
            pages[page].Value.Add(new ModdedUpgradeRepresenter(upgradeType, level));
        }

        /// <summary>
        /// Sets the angle of a modded upgrade, this method will throw an exeption if the upgrade is not on the page of the mod
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="upgradeType"></param>
        /// <param name="upgradeLevel"></param>
        /// <param name="modID"></param>
        public static void SetAngleOfModdedUpgrade(float angle, UpgradeType upgradeType, int upgradeLevel, string modID)
        {
            foreach(var _mod in _allModdedUpgradePages)
            {
                if (_mod.Key != modID)
                    continue;

                foreach (ModdedUpgradeRepresenter _upgrade in _mod.Value)
                {
                    if (_upgrade.UpgradeType != upgradeType || _upgrade.Level != upgradeLevel)
                        continue;

                    _upgrade.SetCustomAngle(angle);
                    return;
                }
            }

            throw new Exception("Cant find upgrade \"" + upgradeType.ToString() + "\" with level " + upgradeLevel + " in upgrades for mod with id: " + modID);
        }

        /// <summary>
        /// Gets the angle offset of a upgrade on the current page, if the upgrade isnt in the modded list, returns the defualt angleOffset of that upgrade
        /// </summary>
        /// <param name="upgradeType"></param>
        /// <param name="upgradeLevel"></param>
        /// <returns></returns>
        public static float GetAngleOfUpgrade(UpgradeType upgradeType, int upgradeLevel)
        {
            UpgradeDescription upgradeInUpgradeList = UpgradeManager.Instance.GetUpgrade(upgradeType, upgradeLevel);

            if (CurrentPage == 0) // If we are on the first page, only display the normal values
                return upgradeInUpgradeList != null ? upgradeInUpgradeList.AngleOffset : 0f;

            var currentMod = GenerateModPages()[CurrentPage - 1];
            foreach(ModdedUpgradeRepresenter upgrade in currentMod.Value)
            {
                if (upgrade.UpgradeType != upgradeType || upgrade.Level != upgradeLevel)
                    continue;

                return upgrade.GetAngleOffset();
            }

            return upgradeInUpgradeList.AngleOffset;
        }

        /// <summary>
        /// If mod already has a page does nothing
        /// </summary>
        /// <param mod=""></param>
        public static void TryAddPage(string modID)
        {
            if (containsMod(modID))
                return;

            List<ModdedUpgradeRepresenter> newList = new List<ModdedUpgradeRepresenter>();
            _allModdedUpgradePages.Add(new KeyValuePair<string, List<ModdedUpgradeRepresenter>>(modID, newList));
        }

        /// <summary>
        /// Generates a list where each instance in the list is a different page, and each list in that list is all the moddedUpgradeTypeAndLevels for that page (only includes active mods)
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<string, List<ModdedUpgradeRepresenter>>> GenerateModPages()
        {
            List<KeyValuePair<string, List<ModdedUpgradeRepresenter>>> pages = new List<KeyValuePair<string, List<ModdedUpgradeRepresenter>>>();
            foreach(var page in _allModdedUpgradePages)
            {
				LoadedModInfo mod = ModsManager.Instance.GetLoadedModWithID(page.Key);

				if(!mod.IsEnabled)
					continue;
				
                pages.Add(page);
            }

            return pages;
        }

        /// <summary>
        /// Generates a list of pages, and then gets the page index of the mod passed
        /// </summary>
        /// <param name="modID"></param>
        /// <returns></returns>
        public static int GetPageForMod(string modID)
        {
            var generatedPages = GenerateModPages();
            int page = GetPageForMod(generatedPages, modID);
            return page;
        }

        /// <summary>
        /// Gets the page index of the mod passed from the pages list passed
        /// </summary>
        /// <param name="pages"></param>
        /// <param name="modID"></param>
        /// <returns></returns>
        public static int GetPageForMod(List<KeyValuePair<string, List<ModdedUpgradeRepresenter>>> pages, string modID)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i].Key != modID)
                    continue;

                return i;
            }

            throw new Exception("Could not find page for mod \"" + ModsManager.Instance.GetLoadedModWithID(modID).OwnerModInfo.DisplayName + "\" with the unique id: \"" + modID + "\""); 
        }

        /// <summary>
        /// Tries to get the mod responsable for a page, note that this generates a new pages list and uses that
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string TryGetModIDForPage(int page)
        {
            page -= 1;

            var pages = GenerateModPages();
            string mod = TryGetModIDForPage(pages, page);
            return mod;
        }

        /// <summary>
        /// Tries to get the mod responsable for a page
        /// </summary>
        /// <param name="pages"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string TryGetModIDForPage(List<KeyValuePair<string, List<ModdedUpgradeRepresenter>>> pages, int page)
        {
            if (page < 0 || page >= pages.Count)
                return null;

            return pages[page].Key;
        }

        /// <summary>
        /// Moves the page to the next avaliable page
        /// </summary>
        public static void NextPage()
        {
            CurrentPage++;

            if (CurrentPage > GetMaxPage())
                CurrentPage = 0;
        }

        /// <summary>
        /// Moves the page to the previus avaliable page
        /// </summary>
        public static void PreviousPage()
        {
            CurrentPage--;

            if (CurrentPage < 0)
                CurrentPage = GetMaxPage();
        }

        /// <summary>
        /// Gets the amount of pages avaliable
        /// </summary>
        /// <returns></returns>
        public static int GetMaxPage()
        {
            var pages = GenerateModPages();
            return pages.Count;
        }

        
        /// <summary>
        /// Called from FromIsUpgradeCurrentlyVisible and if this returns false the upgrade will not be displayed
        /// </summary>
        /// <param name="type"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static bool IsUpgradeVisible(UpgradeType type, int level)
        {
            if (!IsModdedUpgradeType(type))
            {
                if (CurrentPage == 0) // If the current page is the normal page, upgrade should always be visible
                    return true;

                bool state = ForceUpgradeVisible(type, level); // Checks if the upgrade should be active acording to ForceUpgradeVisible
                return state;
            }

            if (CurrentPage == 0) // If the current page is the normal page, upgrade should never be visible, since it is a modded upgrade
                return false;

            var modPages = GenerateModPages();
            if (modPages.Count == 0)
                return true;

            if ((CurrentPage - 1) < 0 || (CurrentPage - 1) >= modPages.Count) // -1 since when the current frame is at 1 the modPages index should be 0
                return true;

            return modPages[CurrentPage - 1].Value.Contains(new ModdedUpgradeRepresenter(type, level));
        }
        
        /// <summary>
        /// If this returns true the upgrade will be displayed no matter what
        /// </summary>
        /// <param name="type"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static bool ForceUpgradeVisible(UpgradeType type, int level)
        {
            if (IsModdedUpgradeType(type))
                return false;

            var modPages = GenerateModPages();
            if (modPages.Count == 0)
                return false;

            if ((CurrentPage - 1) < 0 || (CurrentPage - 1) >= modPages.Count) // -1 since when the current frame is at 1 the modPages index should be 0
                return false;

            return modPages[CurrentPage - 1].Value.Contains(new ModdedUpgradeRepresenter(type, level));
        }

        /// <summary>
        /// Checks if the upgrade is a modded upgrade
        /// </summary>
        /// <param name="UpgradeType"></param>
        /// <returns></returns>
        public static bool IsModdedUpgradeType(UpgradeType UpgradeType)
        {
            return !EnumTools.GetValues<UpgradeType>().Contains(UpgradeType);
        }

        /// <summary>
        /// returns true if AllModdedUpgradePages contains the passed mod
        /// </summary>
        /// <param name="modID"></param>
        /// <returns></returns>
        static bool containsMod(string modID)
        {
            foreach(var valuePair in _allModdedUpgradePages)
            {
                if (valuePair.Key == modID)
                    return true;
            }

            return false;
        }

        static bool modAlreadyHasUpgrade(string modID, ModdedUpgradeRepresenter upgrade)
        {
            foreach (var valuePair in _allModdedUpgradePages)
            {
                if (valuePair.Key != modID)
                    continue;

                return valuePair.Value.Contains(upgrade);
            }

            return false;
        }

    }

}
