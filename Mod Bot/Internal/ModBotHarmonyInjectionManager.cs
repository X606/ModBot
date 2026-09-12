using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace InternalModBot
{
    /// <summary>
    /// Handles all Mod-Bot injections
    /// </summary>
    public static class ModBotHarmonyInjectionManager
    {
        /// <summary>
        /// Injects all patches. Called from preloader
        /// </summary>
        public static void TryInject()
        {
            try
            {
                Harmony harmony = new Harmony("com.Mod-Bot.Internal");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (System.Exception exc)
            {
                throw new System.Exception("Failed to inject Mod-Bot patches", exc); // throw exception so preloader can catch it and write to CRASH.log file
            }

            Debug.Log("Successfully injected Mod-Bot patches!");
        }
    }
}