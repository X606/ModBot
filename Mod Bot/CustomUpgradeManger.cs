using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InternalModBot;
using ModLibrary;
using UnityEngine.UI;

namespace ModLibrary
{
    public class CustomUpgradeManger : Singleton<CustomUpgradeManger>
    {
        public void Start()
        {


            GameObject button = GameUIRoot.Instance.UpgradeUI.transform.GetChild(1).GetChild(6).gameObject;
            CreateButtonAt(button, new Vector3(-300, -200, 0), "Back",BackClicked);
            CreateButtonAt(button, new Vector3(300, -200, 0), "Next",NextClicked);

        }
        void CreateButtonAt(GameObject prefab, Vector3 position, string text, UnityEngine.Events.UnityAction call)
        {
            GameObject spawedButton = Instantiate(prefab);
            spawedButton.transform.SetParent(GameUIRoot.Instance.UpgradeUI.transform.GetChild(1), false);
            spawedButton.GetComponent<RectTransform>().localPosition = position;

            Button button = spawedButton.transform.GetChild(0).GetChild(1).GetComponent<Button>();

            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(call);

            spawedButton.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = text;
        }
        
        public void BackClicked()
        {
            UpgradePagesMangaer.PreviusPage();
            OnBackOrNextClicked();
        }
        public void NextClicked()
        {
            UpgradePagesMangaer.NextPage();
            OnBackOrNextClicked();
        }

        void OnBackOrNextClicked()
        {

            Accessor.CallPrivateMethod(typeof(UpgradeUI), "PopulateIcons", GameUIRoot.Instance.UpgradeUI);
            /* didnt work, might fix it later
            Mod mod = UpgradePagesMangaer.GetModForPage(UpgradePagesMangaer.currentPage);
            if (mod != null)
            {
                GameUIRoot.Instance.UpgradeUI.TitleText.text = "Select upgrade [" + mod.GetModName() + "]";
            }
            else
            {
                GameUIRoot.Instance.UpgradeUI.TitleText.text = "Select upgrade";
            }
            */

            
        }



    }

    public static class UpgradeCosts
    {
        public static int GetCostOfUpgrade(UpgradeType upgradeType, int level = 1)
        {
            if (!upgradeCostsDictionary.ContainsKey(new ModdedUpgradeTypeAndLevel(upgradeType, level)))
            {
                return 1;
            }
            return upgradeCostsDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)];
        }
        public static void SetCostOfUpgrade(UpgradeType upgradeType, int level, int newCost)
        {
            if (upgradeCostsDictionary.ContainsKey(new ModdedUpgradeTypeAndLevel(upgradeType, level)))
            {
                upgradeCostsDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)] = newCost;
                return;
            }

            upgradeCostsDictionary.Add(new ModdedUpgradeTypeAndLevel(upgradeType, level), newCost);
            
        }
        public static void Reset()
        {
            upgradeCostsDictionary = new Dictionary<ModdedUpgradeTypeAndLevel, int>();
        }
        private static Dictionary<ModdedUpgradeTypeAndLevel, int> upgradeCostsDictionary = new Dictionary<ModdedUpgradeTypeAndLevel, int>();
    }



    public static class UpgradeExtensionMethods
    {
        public static void AddUpgrade(this UpgradeManager upgradeManager, UpgradeDescription upgrade, Mod mod)
        {
            upgradeManager.UpgradeDescriptions.Add(upgrade);
            int page = UpgradePagesMangaer.GetPageForMod(mod);

            UpgradePagesMangaer.AddUpgradePage(upgrade.UpgradeType, upgrade.Level, page);

            if (upgrade.Requirement != null)
            {
                RecursivelyAddRequirments(upgrade, page);
            }
        }
        public static void AddUpgradeToModPage(this UpgradeManager upgradeManager, UpgradeDescription upgrade, Mod mod)
        {
            int page = UpgradePagesMangaer.GetPageForMod(mod);
            UpgradePagesMangaer.AddUpgradePage(upgrade.UpgradeType, upgrade.Level, page);
        }
        public static void AddUpgradeAlreadyInGame(this UpgradeManager upgradeManager, UpgradeDescription upgrade, Mod mod)
        {
            UpgradePagesMangaer.AddUpgradeAlreadyInGame(upgrade.UpgradeType, upgrade.Level, mod);
        }


        public static bool IsModdedUpgradeType(this UpgradeDescription upgrade)
        {
            return !ModTools.EnumTools.GetValues<UpgradeType>().Contains(upgrade.UpgradeType);
        }

        public static void SetSingleplayerCost(this UpgradeDescription upgradeDescription, int cost)
        {
            UpgradeCosts.SetCostOfUpgrade(upgradeDescription.UpgradeType, upgradeDescription.Level, cost);
        }
        public static int GetSinglePlayerCost(this UpgradeDescription upgradeDescription)
        {
            return UpgradeCosts.GetCostOfUpgrade(upgradeDescription.UpgradeType, upgradeDescription.Level);
        }






        static void RecursivelyAddRequirments(UpgradeDescription upgrade, int page)
        {
            if (upgrade == null)
                return;

            UpgradePagesMangaer.AddUpgradePage(upgrade.UpgradeType, upgrade.Level, page);


            if (upgrade.Requirement2 != null)
            {
                RecursivelyAddRequirments(upgrade.Requirement2, page);
            }

            RecursivelyAddRequirments(upgrade.Requirement, page);
        }
    }




}
namespace InternalModBot
{
    public static class UpgradePagesMangaer
    {
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
            
            upgradePagesDictionary.Clear();
            modIds.Clear();
            MaxPage = 0;
            currentPage = 0;
        }

        public static bool IsModdedUpgradeType(UpgradeType UpgradeType)
        {
            return !ModLibrary.ModTools.EnumTools.GetValues<UpgradeType>().Contains(UpgradeType);
        }

        public static List<int> GetUpgradePages(UpgradeType upgradeType, int level)
        {

            if (!upgradePagesDictionary.ContainsKey(new ModdedUpgradeTypeAndLevel(upgradeType, level)))
            {
                return new List<int>() { 0 };
            }
            return upgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)];
        }
        public static void AddUpgradePage(UpgradeType upgradeType, int level, int newPage)
        {
            if (!upgradePagesDictionary.ContainsKey(new ModdedUpgradeTypeAndLevel(upgradeType, level)))
            {
                upgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)] = new List<int>();
            }
            if (!IsModdedUpgradeType(upgradeType))
            {
                if (!upgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)].Contains(0))
                {
                    upgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)].Add(0);
                }
            }
            upgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)].Add(newPage);

        }

        public static void AddUpgradeAlreadyInGame(UpgradeType upgradeType, int level, Mod mod)
        {
            int page = GetPageForMod(mod);

            if (!upgradePagesDictionary.ContainsKey(new ModdedUpgradeTypeAndLevel(upgradeType, level)))
            {
                upgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)] = new List<int>();
            }
            if (upgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)].Contains(0))
            {
                upgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)].Remove(0);
            }

            upgradePagesDictionary[new ModdedUpgradeTypeAndLevel(upgradeType, level)].Add(page);
        }

        private static Dictionary<ModdedUpgradeTypeAndLevel, List<int>> upgradePagesDictionary = new Dictionary<ModdedUpgradeTypeAndLevel, List<int>>();


        private static int TryAddPage(Mod mod)
        {
            for (int i = 0; i < modIds.Count; i++)
            {
                if (modIds[i] == mod.GetHashCode())
                {
                    return i;
                }
            }
            MaxPage++;

            while (modIds.Count - 1 < MaxPage)
            {
                modIds.Add(0);
            }

            modIds[MaxPage] = mod.GetHashCode();
            return MaxPage;
        }
        public static int GetPageForMod(Mod mod)
        {
            for (int i = 0; i < modIds.Count; i++)
            {
                if (modIds[i] == mod.GetHashCode())
                {
                    return i;
                }
            }
            return TryAddPage(mod);
        }

        public static Mod GetModForPage(int page)
        {

            for (int i = 0; i < ModsManager.Instance.mods.Count; i++)
            {
                if (ModsManager.Instance.mods[i].GetHashCode() == modIds[page])
                {
                    return ModsManager.Instance.mods[i];
                }
            }

            return null;
        }

        public static void NextPage()
        {
            currentPage++;
            if (currentPage > MaxPage)
            {
                currentPage = 0;
            }
        }
        public static void PreviusPage()
        {
            currentPage--;
            if (currentPage < 0)
            {
                currentPage = MaxPage;
            }
        }
        public static int currentPage = 0;
        private static int MaxPage = 0;
        private static List<int> modIds = new List<int>();

        public static int GetMaxPage()
        {
            return MaxPage;
        }
    }


















    public class ModdedUpgradeTypeAndLevel
    {
        public ModdedUpgradeTypeAndLevel(UpgradeType type, int level)
        {
            UpgradeType = type;
            Level = level;
        }

        public static bool operator ==(ModdedUpgradeTypeAndLevel a, ModdedUpgradeTypeAndLevel b)
        {
            return a.Level == b.Level && a.UpgradeType == b.UpgradeType;

        }
        public static bool operator !=(ModdedUpgradeTypeAndLevel a, ModdedUpgradeTypeAndLevel b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return this == (ModdedUpgradeTypeAndLevel)obj;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)UpgradeType.GetHashCode() * 397) ^ Level;
            }
        }


        // Token: 0x04004D7B RID: 19835
        public UpgradeType UpgradeType;

        // Token: 0x04004D7C RID: 19836
        public int Level = 1;
    }
}