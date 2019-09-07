using System;
using System.Collections.Generic;
using System.Text;

namespace InternalModBot
{
    /// <summary>
    /// Used to represent both a UpgradeType and a level (int) in Mod-Bot (the == and != operators compare the contents and not the references)
    /// </summary>
    public class ModdedUpgradeRepresenter
    {
        /// <summary>
        /// Constructor, will put the inputs supplied into the values that the object holds
        /// </summary>
        /// <param name="type"></param>
        /// <param name="level"></param>
        public ModdedUpgradeRepresenter(UpgradeType type, int level)
        {
            UpgradeType = type;
            Level = level;
        }

        /// <summary>
        /// Sets the custom angle of this upgrade
        /// </summary>
        /// <param name="newAngle"></param>
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
        /// compares a and b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(ModdedUpgradeRepresenter a, ModdedUpgradeRepresenter b)
        {
            return a.Level == b.Level && a.UpgradeType == b.UpgradeType;
        }

        /// <summary>
        /// Compares a and b, but in inverse
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(ModdedUpgradeRepresenter a, ModdedUpgradeRepresenter b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Compares the input with the object.
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
