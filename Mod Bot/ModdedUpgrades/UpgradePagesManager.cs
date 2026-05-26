// New mod loading system
using System.Collections.Generic;

namespace InternalModBot
{
    internal static class UpgradePagesManager
    {
        static readonly List<ModdedUpgradesPage> _upgradePages = new List<ModdedUpgradesPage>() { ModdedUpgradesPage.CreateDummyPage() };

        static int _currentPageIndex = 0;

        internal static bool AreModdedUpgradesAllowed => GameModeManager.IsSinglePlayer();

        internal static bool IsCurrentlyShowingModdedUpgrades => AreModdedUpgradesAllowed && !CurrentPage.IsDummyForVanillaPage;

        static ModdedUpgradesPage CurrentPage => _upgradePages[_currentPageIndex];

        static ModdedUpgradeRepresenter findUpgradeOnCurrentPage(UpgradeType upgradeType, int level)
        {
            return CurrentPage.GetUpgrade(upgradeType, level);
        }

        internal static float GetUpgradeAngle(UpgradeType upgradeType, int level)
        {
            ModdedUpgradeRepresenter moddedUpgrade = findUpgradeOnCurrentPage(upgradeType, level);
            if (moddedUpgrade != null)
                return moddedUpgrade.GetAngleOffset();

            UpgradeDescription upgradeDescription = UpgradeManager.Instance.GetUpgrade(upgradeType, level);
            if (upgradeDescription != null)
                return upgradeDescription.GetAngleOffset();

            return 0f;
        }

        internal static bool IsUpgradeOnCurrentPage(UpgradeType upgradeType, int level)
        {
            return findUpgradeOnCurrentPage(upgradeType, level) != null;
        }

        static ModdedUpgradesPage getPageForMod(string modID)
        {
            return _upgradePages.Find(page => !page.IsDummyForVanillaPage && page.ModID == modID);
        }

        static ModdedUpgradesPage getOrCreatePageForMod(string modID)
        {
            ModdedUpgradesPage page = getPageForMod(modID);
            if (page == null)
            {
                page = new ModdedUpgradesPage(modID);
                _upgradePages.Add(page);
            }

            return page;
        }

        internal static void AddUpgrade(UpgradeType upgradeType, int level, string modID)
        {
            ModdedUpgradesPage page = getOrCreatePageForMod(modID);
            page.AddUpgrade(upgradeType, level);
        }

        internal static void OverrideAngleOfUpgrade(float angle, UpgradeType upgradeType, int level, string modID)
        {
            ModdedUpgradesPage page = getPageForMod(modID);
            if (page != null)
            {
                ModdedUpgradeRepresenter upgrade = page.GetUpgrade(upgradeType, level);
                if (upgrade != null)
                {
                    upgrade.SetCustomAngle(angle);
                }
            }
        }

        internal static void RemoveUpgradePage(string modID)
        {
            ModdedUpgradesPage page = getPageForMod(modID);
            if (page != null && _upgradePages.Remove(page))
            {
                if (_currentPageIndex >= _upgradePages.Count)
                    _currentPageIndex = 0;
            }
        }

        internal static void PreviousPage()
        {
            if (--_currentPageIndex < 0)
                _currentPageIndex = _upgradePages.Count - 1;
        }

        internal static void NextPage()
        {
            if (++_currentPageIndex >= _upgradePages.Count)
                _currentPageIndex = 0;
        }

        internal static string GetModIDForCurrentPage()
        {
            return CurrentPage.ModID;
        }

        internal static bool HasPageForMod(string modID)
        {
            return getPageForMod(modID) != null;
        }

        internal static bool IsOnModdedUpgradesPage()
        {
            return AreModdedUpgradesAllowed && _currentPageIndex > 0;
        }
    }
}