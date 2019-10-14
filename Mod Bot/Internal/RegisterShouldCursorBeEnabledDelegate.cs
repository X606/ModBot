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
    public static class RegisterShouldCursorBeEnabledDelegate
    {
        static List<Func<bool>> registerdHandlers = new List<Func<bool>>();

        /// <summary>
        /// Registers a new handler, if this handeler returns true the cursor will be unlocked
        /// </summary>
        /// <param name="handler"></param>
        public static void Register(Func<bool> handler)
        {
            registerdHandlers.Add(handler);
        }
        /// <summary>
        /// Removes a handler
        /// </summary>
        /// <param name="handler"></param>
        public static void UnRegister(Func<bool> handler)
        {
            registerdHandlers.Remove(handler);
        }

        /// <summary>
        /// Returns true if any of the registerd handelers return true
        /// </summary>
        /// <returns></returns>
        public static bool ShouldMouseBeEnabled()
        {
            for(int i = 0; i < registerdHandlers.Count; i++)
            {
                if(registerdHandlers[i]())
                    return true;
            }

            return false;
        }

    }
}
