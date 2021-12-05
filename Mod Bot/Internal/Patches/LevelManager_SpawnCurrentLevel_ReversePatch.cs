using HarmonyLib;
using ModLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace InternalModBot
{
    /*
    [HarmonyPatch]
    internal static class LevelManager_SpawnCurrentLevel_ReversePatch
    {
        [HarmonyTargetMethod]
        static MethodBase TargetMethod()
        {
            debug.Log("what the fuck");
            return typeof(LevelManager).GetDeclaredNestedType("<SpawnCurrentLevel>d__65").GetMethodInfo("MoveNext");
        }

        [HarmonyReversePatch(HarmonyReversePatchType.Original)]
        public static LevelEditorLevelData GetLevelEditorLevelData(LevelDescription levelDescription)
        {
            IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                Type spawnCurrentLevel_Type = typeof(LevelManager).GetDeclaredNestedType("<SpawnCurrentLevel>d__65");

                LocalBuilder local_prefabPath = generator.DeclareLocal(typeof(string));

                FieldInfo spawnCurrentLevel_this_FI = spawnCurrentLevel_Type.GetFieldInfo("<>4__this");
                PropertyInfo levelManager_Instance_PI = typeof(Singleton<LevelManager>).GetProperty(nameof(Singleton<LevelManager>.Instance));
                instructions = instructions.ReplaceInstructions(
                    new CodeInstruction[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, spawnCurrentLevel_this_FI)
                    },
                    new CodeInstruction[]
                    {
                        new CodeInstruction(OpCodes.Call, levelManager_Instance_PI.GetMethod)
                    }, CodeInstrucionComparisonMode.OpCodeAndOperand);

                FieldInfo spawnCurrentLevel_levelDescription_FI = spawnCurrentLevel_Type.GetFieldInfo("<levelDescription>5__2");
                instructions = instructions.Manipulator(
                (CodeInstruction item) => item.LoadsField(spawnCurrentLevel_levelDescription_FI),
                delegate (CodeInstruction instruction)
                {
                    instruction.opcode = OpCodes.Ldarg_0;
                    instruction.operand = null;
                });

                object operand = instructions.First(instruction => instruction.opcode == OpCodes.Switch).operand;
                debug.Log(operand.GetType() + " " + operand.ToString());

                return instructions;
            }

            _ = Transpiler(null, null);
            return null;
        }
    }
    */
}
