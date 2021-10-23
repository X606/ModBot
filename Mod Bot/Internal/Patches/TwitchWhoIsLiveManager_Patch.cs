using HarmonyLib;
using System.Collections.Generic;

namespace InternalModBot
{
    [HarmonyPatch(typeof(TwitchWhoIsLiveManager))]
    static class TwitchWhoIsLiveManager_Patch
    {
        [HarmonyTranspiler]
        [HarmonyPatch("refreshGameLiveStreams")]
        static IEnumerable<CodeInstruction> refreshGameLiveStreams_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                // Remove hard-coded "ban"
                if (instruction.LoadsConstant(42863452))
                    instruction.operand = -1;

                yield return instruction;
            }
        }
    }
}