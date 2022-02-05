using System;
using System.Collections.Generic;
using System.Text;

namespace InternalModBot
{
    /// <summary>
    /// Used to represent both an <see cref="global::UpgradeType"/> and a level (<see cref="int"/>) in Mod-Bot (the == and != operators compare the contents and not the references)
    /// </summary>
    internal class ModdedUpgradeRepresenter
    {
        public ModdedUpgradeRepresenter(UpgradeType type, int level)
        {
            UpgradeType = type;
            Level = level;
        }

        public void SetCustomAngle(float newAngle)
        {
            _customAngle = newAngle;
        }

        public float GetAngleOffset()
        {
            return _customAngle ?? UpgradeManager.Instance.GetUpgrade(UpgradeType, Level).AngleOffset;
        }

        public static bool operator ==(ModdedUpgradeRepresenter a, ModdedUpgradeRepresenter b)
        {
            if (a is null || b is null)
                return a is null && b is null;

            return a.Level == b.Level && a.UpgradeType == b.UpgradeType;
        }

        public static bool operator !=(ModdedUpgradeRepresenter a, ModdedUpgradeRepresenter b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is ModdedUpgradeRepresenter other && this == other;
        }

        public override int GetHashCode()
        {
            int hashCode = 1545011012;
            hashCode = (hashCode * -1521134295) + UpgradeType.GetHashCode();
            hashCode = (hashCode * -1521134295) + Level.GetHashCode();
            return hashCode;
        }

        public UpgradeType UpgradeType;

        public int Level = 1;

        float? _customAngle = 0;
    }
}
