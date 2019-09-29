using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModLibrary.YieldInstructions
{
    /// <summary>
    /// Waits until the given delegate method returns <see langword="true"/>
    /// </summary>
    public class WaitUntilTrue : CustomYieldInstruction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitUntilTrue"/> instruction
        /// </summary>
        /// <param name="func"></param>
        public WaitUntilTrue(Func<bool> func)
        {
            target = func;
        }

        /// <summary>
        /// Returns if the <see cref="Coroutine"/> should keep waiting
        /// </summary>
        public override bool keepWaiting
        {
            get
            {
                return !target();
            }
        }

        Func<bool> target;
    }
}
