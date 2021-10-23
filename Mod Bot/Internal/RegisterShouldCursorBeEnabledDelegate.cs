using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalModBot
{
    /// <summary>
    /// Used internally in mod-bot for telling if the cursor should be enabled or not
    /// </summary>
    internal static class RegisterShouldCursorBeEnabledDelegate
    {
        static List<Func<bool>> _registeredHandlers = new List<Func<bool>>();

        /// <summary>
        /// Registers a new handler, if this handler returns <see langword="true"/> the cursor will be unlocked
        /// </summary>
        /// <param name="handler"></param>
        public static void Register(Func<bool> handler)
        {
            _registeredHandlers.Add(handler);
        }

        /// <summary>
        /// Removes a handler
        /// </summary>
        /// <param name="handler"></param>
        public static void UnRegister(Func<bool> handler)
        {
            _registeredHandlers.Remove(handler);
        }

        /// <summary>
        /// Returns <see langword="true"/> if any of the registerd handlers return <see langword="true"/>
        /// </summary>
        /// <returns></returns>
        public static bool ShouldMouseBeEnabled()
        {
            for(int i = 0; i < _registeredHandlers.Count; i++)
            {
                if(_registeredHandlers[i]())
                    return true;
            }

            return false;
        }

        [HarmonyPatch]
        static class Patches
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(GameUIRoot), "RefreshCursorEnabled")]
            static bool GameUIRoot_RefreshCursorEnabled_Prefix()
            {
                if (ShouldMouseBeEnabled() || ModsManager.Instance == null || ModsManager.Instance.PassOnMod.ShouldCursorBeEnabled())
                {
                    InputManager.Instance.SetCursorEnabled(true);
                    return false;
                }

                return true;
            }
        }
    }
}
