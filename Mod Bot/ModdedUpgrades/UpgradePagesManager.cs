using System;
using System.Collections.Generic;
using System.Text;
using ModLibrary;

namespace InternalModBot
{
    public static class UpgradePagesManager
    {
        private static Dictionary<ModdedUpgradeTypeAndLevel, List<int>> UpgradePagesDictionary = new Dictionary<ModdedUpgradeTypeAndLevel, List<int>>();

        public static int CurrentPage = 0;
        private static int MaxPage = 0;

        private static List<int> ModIDs = new List<int>();

        public static void Reset()
        {
            for (int i = 0; i < UpgradeManager.Instance.UpgradeDescriptions.Count; i++)
            {
                if (UpgradeManager.Instance.UpgradeDescriptions[i].IsModdedUpgradeType())
                {
                    UpgradeManager.Instance.UpgradeDescriptions.RemoveAt(i);
                    i--;
                }
            }

            UpgradePagesDictionary.Clear();
            ModIDs.Clear();
            MaxPage = 0;
            CurrentPage = 0;
        }

        public static bool IsModdedUpgradeType(UpgradeType UpgradeType)
        {
            return !ModLibrary.ModTools.EnumTools.GetValues<UpgradeType>().Contains(UpgradeType);
        }

        public static List<int> GetUpgradePages(UpgradeType upgradeType, int level)
        {
            if (!UpgradePagesDictionary.ContainsKey(new ModdedUpgradeTypeAndLevel(upgradeType, level)))
            {
                return new List<int>() { 0 };
            }

            return UpgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)];
        }

        public static void AddUpgradePage(UpgradeType upgradeType, int level, int newPage)
        {
            if (!UpgradePagesDictionary.ContainsKey(new ModdedUpgradeTypeAndLevel(upgradeType, level)))
            {
                UpgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)] = new List<int>();
            }

            if (!IsModdedUpgradeType(upgradeType) && !UpgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)].Contains(0))
            {
                UpgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)].Add(0);
            }

            UpgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)].Add(newPage);
        }

        public static void AddUpgradeAlreadyInGame(UpgradeType upgradeType, int level, Mod mod)
        {
            int page = GetPageForMod(mod);

            if (!UpgradePagesDictionary.ContainsKey(new ModdedUpgradeTypeAndLevel(upgradeType, level)))
            {
                UpgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)] = new List<int>();
            }
            if (UpgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)].Contains(0))
            {
                UpgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)].Remove(0);
            }

            UpgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)].Add(page);
        }

        private static int TryAddPage(Mod mod)
        {
            for (int i = 0; i < ModIDs.Count; i++)
            {
                if (ModIDs[i] == mod.GetHashCode())
                {
                    return i;
                }
            }
            MaxPage++;

            while (ModIDs.Count - 1 < MaxPage)
            {
                ModIDs.Add(0);
            }

            ModIDs[MaxPage] = mod.GetHashCode();
            return MaxPage;
        }

        public static int GetPageForMod(Mod mod)
        {
            for (int i = 0; i < ModIDs.Count; i++)
            {
                if (ModIDs[i] == mod.GetHashCode())
                {
                    return i;
                }
            }

            return TryAddPage(mod);
        }

        public static Mod GetModForPage(int page)
        {
            if (ModIDs.Count - 1 < page)
                return null;

            for (int i = 0; i < ModsManager.Instance.Mods.Count; i++)
            {
                if (ModsManager.Instance.Mods[i].GetHashCode() == ModIDs[page])
                {
                    return ModsManager.Instance.Mods[i];
                }
            }

            return null;
        }

        public static void NextPage()
        {
            CurrentPage++;
            if (CurrentPage > MaxPage)
            {
                CurrentPage = 0;
            }
        }

        public static void PreviusPage()
        {
            CurrentPage--;
            if (CurrentPage < 0)
            {
                CurrentPage = MaxPage;
            }
        }

        public static int GetMaxPage()
        {
            return MaxPage;
        }
    }
}
