using Steamworks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLibrary
{
    /// <summary>
    /// Defines methods for disabling and enabling certain inputs for a <see cref="FirstPersonMover"/>
    /// </summary>
    public static class CharacterInputRestrictor
    {
        static Dictionary<FirstPersonMover, InputRestrictions> _characterInputRestrictions = new Dictionary<FirstPersonMover, InputRestrictions>();

        /// <summary>
        /// Adds an input restriction on the given <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="firstPersonMover">The target to add a restriction to</param>
        /// <param name="inputRestriction">A bitfield of input restrictions to apply</param>
        public static void AddRestriction(FirstPersonMover firstPersonMover, InputRestrictions inputRestriction)
        {
            if (!_characterInputRestrictions.ContainsKey(firstPersonMover))
                _characterInputRestrictions.Add(firstPersonMover, 0);

            _characterInputRestrictions[firstPersonMover] |= inputRestriction;
        }

        /// <summary>
        /// Returns if the given <see cref="FirstPersonMover"/> has all of the input restrictions in the given <see cref="InputRestrictions"/> bitfield
        /// </summary>
        /// <param name="firstPersonMover">The <see cref="FirstPersonMover"/> to check</param>
        /// <param name="inputRestriction">A bitfield of input restrictions to check for</param>
        /// <returns></returns>
        public static bool HasRestrictions(FirstPersonMover firstPersonMover, InputRestrictions inputRestriction)
        {
            if (!_characterInputRestrictions.ContainsKey(firstPersonMover))
                return false;

            return (_characterInputRestrictions[firstPersonMover] & inputRestriction) == inputRestriction;
        }

        /// <summary>
        /// Removes all input restrictions defined in the given bitfield from a <see cref="FirstPersonMover"/>, if no input restrictions remain, the <see cref="FirstPersonMover"/> will be removed from the input restrictions dictionary
        /// </summary>
        /// <param name="firstPersonMover">The <see cref="FirstPersonMover"/> to remove the restrictions from</param>
        /// <param name="inputRestriction">A bitfield of input restrictions to remove</param>
        public static void RemoveRestriction(FirstPersonMover firstPersonMover, InputRestrictions inputRestriction)
        {
            if (!_characterInputRestrictions.ContainsKey(firstPersonMover))
                return;

            _characterInputRestrictions[firstPersonMover] &= ~inputRestriction; // Remove the restriction(s) from the bitfield

            if (_characterInputRestrictions[firstPersonMover] == 0) // If we are left with 0 (No values in the bitfield), remove it from the dictionary
                _characterInputRestrictions.Remove(firstPersonMover);
        }

        /// <summary>
        /// Returns if the given <see cref="FirstPersonMover"/> has any input restrictions
        /// </summary>
        /// <param name="firstPersonMover">The <see cref="FirstPersonMover"/> to check for</param>
        /// <returns></returns>
        public static bool HasAnyRestrictions(FirstPersonMover firstPersonMover)
        {
            return _characterInputRestrictions.ContainsKey(firstPersonMover);
        }

        /// <summary>
        /// Gets the <see cref="InputRestrictions"/> for the given <see cref="FirstPersonMover"/>, if the <see cref="FirstPersonMover"/> is not in the input restrictions dictionary, a <see cref="KeyNotFoundException"/> is thrown, to avoid this, consider calling <see cref="HasAnyRestrictions(FirstPersonMover)"/> to check if any input restrictions are defined for a specific <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="firstPersonMover">The <see cref="FirstPersonMover"/> to attempt to get the <see cref="InputRestrictions"/> of</param>
        /// <returns></returns>
        public static InputRestrictions GetInputRestrictions(FirstPersonMover firstPersonMover)
        {
            if (!_characterInputRestrictions.ContainsKey(firstPersonMover))
                throw new KeyNotFoundException("FirstPersonMover passed to CharacterInputRestrictor.GetInputRestrictions(FirstPersonMover) does not exist in the input restrictions dictionary");

            return _characterInputRestrictions[firstPersonMover];
        }
    }
}
