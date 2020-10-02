using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModLibrary.YieldInstructions
{
    /// <summary>
    /// Waits until the character model of the given <see cref="FirstPersonMover"/> as been initialized
    /// </summary>
    public class WaitForCharacterModelInitialization : CustomYieldInstruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForCharacterModelInitialization"/> instruction
        /// </summary>
        /// <param name="firstPersonMover">The <see cref="FirstPersonMover"/> to check</param>
        public WaitForCharacterModelInitialization(FirstPersonMover firstPersonMover)
        {
            _firstPersonMover = firstPersonMover;
        }

        /// <summary>
        /// Returns if the <see cref="Coroutine"/> should keep waiting
        /// </summary>
        public override bool keepWaiting => !_firstPersonMover.HasCharacterModel();

        FirstPersonMover _firstPersonMover;
    }
}
