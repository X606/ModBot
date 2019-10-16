using Steamworks;
using System;
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
            validateInputRestrictions(inputRestriction);

            if (!_characterInputRestrictions.ContainsKey(firstPersonMover))
                _characterInputRestrictions.Add(firstPersonMover, 0);

            _characterInputRestrictions[firstPersonMover] |= inputRestriction;

            firstPersonMover.AddDeathListener(filterDictionary);
        }

        /// <summary>
        /// Returns if the given <see cref="FirstPersonMover"/> has all of the input restrictions in the given <see cref="InputRestrictions"/> bitfield
        /// </summary>
        /// <param name="firstPersonMover">The <see cref="FirstPersonMover"/> to check</param>
        /// <param name="inputRestriction">A bitfield of input restrictions to check for</param>
        /// <returns></returns>
        public static bool HasRestrictions(FirstPersonMover firstPersonMover, InputRestrictions inputRestriction)
        {
            validateInputRestrictions(inputRestriction);

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
            validateHasRestrictionsForCharacter(firstPersonMover);
            validateInputRestrictions(inputRestriction);

            _characterInputRestrictions[firstPersonMover] &= ~inputRestriction; // Remove the restriction(s) from the bitfield

            filterDictionary();
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
            validateHasRestrictionsForCharacter(firstPersonMover);

            return _characterInputRestrictions[firstPersonMover];
        }

        /// <summary>
        /// Sets the <see cref="InputRestrictions.AttackKeyDown"/> on the given <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="firstPersonMover">The <see cref="FirstPersonMover"/> to set the input restrictions of</param>
        /// <param name="inputRestrictions">The <see cref="InputRestrictions"/> to set</param>
        public static void SetInputRestrictions(FirstPersonMover firstPersonMover, InputRestrictions inputRestrictions)
        {
            validateInputRestrictions(inputRestrictions);

            _characterInputRestrictions[firstPersonMover] = inputRestrictions;

            filterDictionary();
        }

        /// <summary>
        /// Removes all input restrictions for the given <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="firstPersonMover"></param>
        public static void RemoveAllRestrictions(FirstPersonMover firstPersonMover)
        {
            _characterInputRestrictions.Remove(firstPersonMover);
        }

        static void validateHasRestrictionsForCharacter(FirstPersonMover firstPersonMover)
        {
            if (!_characterInputRestrictions.ContainsKey(firstPersonMover))
                throw new KeyNotFoundException("The given FirstPersonMover does not exist in the input restrictions dictionary, check if a character is defined with the HasAnyRestrictions(FirstPersonMover) method");
        }

        static void validateInputRestrictions(InputRestrictions inputRestrictions)
        {
            if (!isValidRange(inputRestrictions))
                throw new ArgumentOutOfRangeException("Input restrictions out of range, the value must be greater than 0");
        }

        static void filterDictionary()
        {
            List<FirstPersonMover> firstPersonMovers = _characterInputRestrictions.Keys.ToList();
            foreach (FirstPersonMover firstPersonMover in firstPersonMovers)
            {
                if (firstPersonMover == null || !firstPersonMover.IsAttachedAndAlive() || !isValidRange(_characterInputRestrictions[firstPersonMover]))
                    _characterInputRestrictions.Remove(firstPersonMover);
            }
        }

        static bool isValidRange(InputRestrictions inputRestrictions)
        {
            return inputRestrictions > 0;
        }
    }
}
