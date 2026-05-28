using System.Collections.Generic;
using System.Linq;

namespace InternalModBot
{
    internal class ModdedUpgradesPage
    {
        public readonly string ModID;
        public readonly int Index;
        public readonly List<ModdedUpgradeRepresenter> Upgrades;

        public readonly bool IsDummyForVanillaPage;

        public ModdedUpgradesPage(string modID, int index) : this(modID, index, new List<ModdedUpgradeRepresenter>(), false)
        {
        }

        ModdedUpgradesPage(string modID, int index, List<ModdedUpgradeRepresenter> upgrades, bool isDummyForVanillaPage)
        {
            ModID = modID;
            Index = index;
            Upgrades = upgrades;
            IsDummyForVanillaPage = isDummyForVanillaPage;
        }

        public static ModdedUpgradesPage CreateDummyPage()
        {
            return new ModdedUpgradesPage(null, 0, null, true);
        }

        public ModdedUpgradeRepresenter GetUpgrade(UpgradeType upgradeType, int level)
        {
            if (IsDummyForVanillaPage)
                return null;

            return Upgrades.Find(upgrade => upgrade.UpgradeType == upgradeType && upgrade.Level == level);
        }

        public void AddUpgrade(UpgradeType upgradeType, int level)
        {
            if (IsDummyForVanillaPage)
                return;

            if (Upgrades.Any(upgrade => upgrade.UpgradeType == upgradeType && upgrade.Level == level))
                return;

            Upgrades.Add(new ModdedUpgradeRepresenter(upgradeType, level));

            UpgradeDescription upgradeDescription = UpgradeManager.Instance.GetUpgrade(upgradeType, level);

            if (upgradeDescription.Requirement != null)
                AddUpgrade(upgradeDescription.Requirement.UpgradeType, upgradeDescription.Requirement.Level);

            if (upgradeDescription.Requirement2 != null)
                AddUpgrade(upgradeDescription.Requirement2.UpgradeType, upgradeDescription.Requirement2.Level);
        }
    }
}
