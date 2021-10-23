using HarmonyLib;
using InternalModBot;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace ModLibrary
{
    internal class InjectionInfo
    {
		public InjectionTargetAttribute InjectionTargetAttribute;
		public MethodInfo PatchMethod;

        public InjectionInfo(InjectionTargetAttribute injectionTargetAttribute, MethodInfo patchMethod)
        {
            InjectionTargetAttribute = injectionTargetAttribute;
            PatchMethod = patchMethod;
        }

		public void Patch(Harmony harmonyInstance)
        {
            debug.Log("Patching " + InjectionTargetAttribute.TargetMethod.FullDescription() + " with " + InjectionTargetAttribute.PatchType + " " + PatchMethod.FullDescription() + " with harmony ID " + harmonyInstance.Id);

            switch (InjectionTargetAttribute.PatchType)
            {
                case HarmonyPatchType.Prefix:
                    harmonyInstance.Patch(InjectionTargetAttribute.TargetMethod, new HarmonyMethod(PatchMethod));
                    break;
                case HarmonyPatchType.Postfix:
                    harmonyInstance.Patch(InjectionTargetAttribute.TargetMethod, null, new HarmonyMethod(PatchMethod));
                    break;
                case HarmonyPatchType.Transpiler:
                case HarmonyPatchType.Finalizer:
                case HarmonyPatchType.ReversePatch:
                case HarmonyPatchType.All:
                default:
                    throw new NotImplementedException("HarmonyPatchType " + InjectionTargetAttribute.PatchType + " is not implemented");
            }
        }
    }
}
