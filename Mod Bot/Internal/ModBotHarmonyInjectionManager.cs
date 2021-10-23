using HarmonyLib;
using System.Linq;
using System.Reflection;

namespace InternalModBot
{
    /// <summary>
    /// Handles all of Mod-Bots runtils patching
    /// </summary>
    internal static class ModBotHarmonyInjectionManager
    {
        /// <summary>
        /// Injects all patches if it is not already done
        /// </summary>
        public static void TryInject()
        {
            Harmony harmony = new Harmony("com.Mod-Bot.Internal");
            if (!harmony.GetPatchedMethods().Any())
                harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}