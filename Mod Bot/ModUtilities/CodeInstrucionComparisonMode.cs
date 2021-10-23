using HarmonyLib;
using System;

namespace ModLibrary
{
    /// <summary>
    /// Specifies what determines if two <see cref="CodeInstruction"/>s are considered equal
    /// </summary>
    [Flags]
    public enum CodeInstrucionComparisonMode
    {
        /// <summary>
        /// Compare the <see cref="CodeInstruction.opcode"/> field from both instances
        /// </summary>
        OpCode = 1 << 0,

        /// <summary>
        /// Compare the <see cref="CodeInstruction.operand"/> field from both instances
        /// </summary>
        Operand = 1 << 1,

        /// <summary>
        /// Compare both the <see cref="CodeInstruction.opcode"/> and <see cref="CodeInstruction.operand"/> fields from both instances
        /// </summary>
        OpCodeAndOperand = OpCode | Operand
    }
}
