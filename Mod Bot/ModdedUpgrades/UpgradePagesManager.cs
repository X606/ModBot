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
        private static List<KeyValuePair<Mod, List<ModdedUpgradeRepresenter>>> AllModdedUpgradePages = new List<KeyValuePair<Mod, List<ModdedUpgradeRepresenter>>>(); // first string is uniqe ids of mods returned by mod.GetUniqeId(), the second value are all of the upgrades on that page (this also includes disabled mods)
        
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

            AllModdedUpgradePages.Clear();
            CurrentPage = 0;
        }

        /// <summary>
        /// Removes all of the upgrades in UpgradeManager.Instance.UpgradeDescriptions placed there by a mod 
        /// </summary>
        /// <param name="mod"></param>
        public static void RemoveModdedUpgradesFor(Mod mod)
        {
            foreach (var item in AllModdedUpgradePages)
            {
                if (item.Key == mod)
                {
                    foreach (ModdedUpgradeRepresenter upgradeType in item.Value)
                    {
                        for (int i = 0; i < UpgradeManager.Instance.UpgradeDescriptions.Count; i++)
                        {
                            if (UpgradeManager.Instance.UpgradeDescriptions[i].UpgradeType == upgradeType.UpgradeType && UpgradeManager.Instance.UpgradeDescriptions[i].Level == upgradeType.Level && IsModdedUpgradeType(upgradeType.UpgradeType))
                            {
                                UpgradeManager.Instance.UpgradeDescriptions.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// Adds a upgrade to keep track of, this upgrade will be displayed on the page of the mod passed
        /// </summary>
        /// <param name="upgradeType"></param>
        /// <param name="level"></param>
        /// <param name="mod"></param>
        public static void AddUpgrade(UpgradeType upgradeType, int level, Mod mod)
        {
            if (ModAlreadyHasUpgrade(mod, new ModdedUpgradeRepresenter(upgradeType, level))) // we dont want to add the same upgrade twice to the same page
                return;


            TryAddPage(mod);
            var pages = GenerateModPages();
            int page = GetPageForMod(pages, mod);
            pages[page].Value.Add(new ModdedUpgradeRepresenter(upgradeType, level));

        }

        /// <summary>
        /// Sets the angle of a modded upgrade, this method will throw an exeption if the upgrade is not on the page of the mod
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="upgradeType"></param>
        /// <param name="upgradeLevel"></param>
        /// <param name="mod"></param>
        public static void SetAngleOfModdedUpgrade(float angle, UpgradeType upgradeType, int upgradeLevel, Mod mod)
        {
            foreach(var _mod in AllModdedUpgradePages)
            {
                if (_mod.Key != mod)
                    continue;

                foreach(ModdedUpgradeRepresenter _upgrade in _mod.Value)
                {
                    if (_upgrade.UpgradeType != upgradeType || _upgrade.Level != upgradeLevel)
                        continue;

                    _upgrade.SetCustomAngle(angle);
                    return;
                }
            }

            throw new Exception("Cant find upgrade \"" + upgradeType.ToString() + "\" with level " + upgradeLevel + " in upgrades for mod with id: " + mod.GetUniqueID());
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

            if (CurrentPage == 0) // if we are on the first page, only display the normal values
            {
                if (upgradeInUpgradeList == null)
                    return 0;

                return upgradeInUpgradeList.AngleOffset;
            }

            var currentMod = GenerateModPages()[CurrentPage - 1];
            foreach(var upgrade in currentMod.Value)
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
        public static void TryAddPage(Mod mod)
        {
            if (ContainsMod(mod))
                return;

            List<ModdedUpgradeRepresenter> newList = new List<ModdedUpgradeRepresenter>();
            AllModdedUpgradePages.Add(new KeyValuePair<Mod, List<ModdedUpgradeRepresenter>>(mod, newList));
        }

        /// <summary>
        /// Generates a list where each instance in the list is a diffrent page, and each list in that list is all the moddedUpgradeTypeAndLevels for that page (only includes active mods)
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<Mod, List<ModdedUpgradeRepresenter>>> GenerateModPages()
        {
            List<KeyValuePair<Mod, List<ModdedUpgradeRepresenter>>> pages = new List<KeyValuePair<Mod, List<ModdedUpgradeRepresenter>>>();
            foreach(var page in AllModdedUpgradePages)
            {
                if (!page.Key.IsModEnabled())
                    continue;

                pages.Add(page);
            }

            return pages;
        }


        /// <summary>
        /// Generates a list of pages, and then gets the page index of the mod passed
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static int GetPageForMod(Mod mod)
        {
            var generatedPages = GenerateModPages();
            int page = GetPageForMod(generatedPages, mod);
            return page;
        }
        /// <summary>
        /// Gets the page index of the mod passed from the pages list passed
        /// </summary>
        /// <param name="pages"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static int GetPageForMod(List<KeyValuePair<Mod, List<ModdedUpgradeRepresenter>>> pages, Mod mod)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i].Key != mod)
                    continue;

                return i;
            }

            throw new Exception("Didnt find page for mod \"" + mod.GetModName() + "\" with the unique id: \"" + mod.GetUniqueID() + "\""); 
        }

        /// <summary>
        /// Tries to get the mod responsable for a page, note that this generates a new pages list and uses that
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static Mod TryGetModForPage(int page)
        {
            page -= 1;

            var pages = GenerateModPages();
            Mod mod = TryGetModForPage(pages, page);
            return mod;
        }
        /// <summary>
        /// Tries to get the mod responsable for a page
        /// </summary>
        /// <param name="pages"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static Mod TryGetModForPage(List<KeyValuePair<Mod, List<ModdedUpgradeRepresenter>>> pages, int page)
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
            {
                CurrentPage = 0;
            }
        }
        /// <summary>
        /// Moves the page to the previus avaliable page
        /// </summary>
        public static void PreviusPage()
        {
            CurrentPage--;
            if (CurrentPage < 0)
            {
                CurrentPage = GetMaxPage();
            }
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
                if (CurrentPage == 0) // on the normal page the normal upgrades should always display
                    return true;

                bool state = ForceUpgradeVisible(type, level); // checks if the upgrade should be active acording to ForceUpgradeVisible
                return state;
            }

            if (CurrentPage == 0) // on the normal page no modded upgrades should be displayed
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
            return !ModLibrary.ModTools.EnumTools.GetValues<UpgradeType>().Contains(UpgradeType);
        }

        /// <summary>
        /// returns true if AllModdedUpgradePages contains the passed mod
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        private static bool ContainsMod(Mod mod)
        {
            foreach(var valuePair in AllModdedUpgradePages)
            {
                if (valuePair.Key == mod)
                    return true;
            }

            return false;
        }

        private static bool ModAlreadyHasUpgrade(Mod mod, ModdedUpgradeRepresenter upgrade)
        {
            foreach (var valuePair in AllModdedUpgradePages)
            {
                if (valuePair.Key != mod)
                    continue;

                return valuePair.Value.Contains(upgrade);

            }

            return false;
        }
    }
}
