using System;
using System.Collections.Generic;
using System.Text;

namespace InternalModBot
{
    /// <summary>
    /// Used to represent both an <see cref="global::UpgradeType"/> and a level (<see cref="Int32"/>) in Mod-Bot (the == and != operators compare the contents and not the references)
    /// </summary>
    public class ModdedUpgradeRepresenter
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ModdedUpgradeRepresenter"/> class
        /// </summary>
        /// <param name="type">The <see cref="global::UpgradeType"/> to store</param>
        /// <param name="level">The level of the upgrade to store</param>
        public ModdedUpgradeRepresenter(UpgradeType type, int level)
        {
            UpgradeType = type;
            Level = level;
        }

        /// <summary>
        /// Sets the angle of this upgrade
        /// </summary>
        /// <param name="newAngle">The new angle</param>
        public void SetCustomAngle(float newAngle)
        {
            CustomAngle = newAngle;
        }

        /// <summary>
        /// Gets the custom angle set to this upgrade, if no custom angle is set it will return the defualt angle
        /// </summary>
        /// <returns></returns>
        public float GetAngleOffset()
        {
            float angle = UpgradeManager.Instance.GetUpgrade(UpgradeType, Level).AngleOffset;

            if (CustomAngle.HasValue)
            {
                angle = CustomAngle.Value;
            }

            return angle;
        }

        /// <summary>
        /// Compares two instances of the <see cref="ModdedUpgradeRepresenter"/> class by comparing their <see cref="global::UpgradeType"/> and level
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(ModdedUpgradeRepresenter a, ModdedUpgradeRepresenter b)
        {
            return a.Level == b.Level && a.UpgradeType == b.UpgradeType;
        }

        /// <summary>
        /// Compares two instances of the <see cref="ModdedUpgradeRepresenter"/> class by comparing their <see cref="global::UpgradeType"/> and level, and negates the result
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(ModdedUpgradeRepresenter a, ModdedUpgradeRepresenter b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Compares the current instance to the given <see cref="object"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ModdedUpgradeRepresenter))
            {
                return false;
            }

            return this == (obj as ModdedUpgradeRepresenter);
        }

        /// <summary>
        /// Gets the Hash Code for the current object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (UpgradeType.GetHashCode() * 397) ^ Level;
            }
        }

        /// <summary>
        /// The UpgradeType the object holds.
        /// </summary>
        public UpgradeType UpgradeType;

        /// <summary>
        /// The level the object holds
        /// </summary>
        public int Level = 1;

        private float? CustomAngle = 0;
    }
}
