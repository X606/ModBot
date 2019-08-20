using System;
using System.Collections.Generic;
using System.Text;

namespace InternalModBot
{
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
            if (!(obj is ModdedUpgradeTypeAndLevel))
            {
                return false;
            }

            return this == (obj as ModdedUpgradeTypeAndLevel);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (UpgradeType.GetHashCode() * 397) ^ Level;
            }
        }

        public UpgradeType UpgradeType;

        public int Level = 1;
    }
}
