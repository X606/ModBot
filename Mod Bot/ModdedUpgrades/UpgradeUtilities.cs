namespace InternalModBot
{
    internal static class UpgradeUtilities
    {
        internal static UpgradeTypeAndLevel GetUpgradeRequirement(UpgradeType upgradeType, int level)
        {
            UpgradeDescription upgradeDescription = UpgradeManager.Instance.GetUpgrade(upgradeType, level);
            if (upgradeDescription == null || upgradeDescription.Requirement == null)
                return null;

            return new UpgradeTypeAndLevel
            {
                UpgradeType = upgradeDescription.Requirement.UpgradeType,
                Level = upgradeDescription.Requirement.Level
            };
        }
    }
}