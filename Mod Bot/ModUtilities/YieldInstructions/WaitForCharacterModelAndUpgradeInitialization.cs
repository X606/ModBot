using UnityEngine;

namespace ModLibrary.YieldInstructions
{
    /// <summary>
    /// Waits until both the upgrades and character model has been initialized on the tracking <see cref="FirstPersonMover"/>
    /// </summary>
    public class WaitForCharacterModelAndUpgradeInitialization : CustomYieldInstruction
    {
        readonly FirstPersonMover _firstPersonMover;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForCharacterModelAndUpgradeInitialization"/> instruction
        /// </summary>
        /// <param name="firstPersonMover">The <see cref="FirstPersonMover"/> to check</param>
        public WaitForCharacterModelAndUpgradeInitialization(FirstPersonMover firstPersonMover)
        {
            _firstPersonMover = firstPersonMover;
        }

        /// <summary>
        /// Returns if the <see cref="Coroutine"/> should keep waiting
        /// </summary>
        public override bool keepWaiting => !_firstPersonMover._hasInitializedCharacter;
    }
}