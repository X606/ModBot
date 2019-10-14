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
        static List<Func<bool>> registerdHandelers = new List<Func<bool>>();

        /// <summary>
        /// Registers a new handeler, if this handeler returns true the cursor will be unlocked
        /// </summary>
        /// <param name="handeler"></param>
        public static void Register(Func<bool> handeler)
        {
            registerdHandelers.Add(handeler);
        }
        /// <summary>
        /// Removes a handeler
        /// </summary>
        /// <param name="handeler"></param>
        public static void UnRegister(Func<bool> handeler)
        {
            registerdHandelers.Remove(handeler);
        }

        /// <summary>
        /// Returns true if any of the registerd handelers return true
        /// </summary>
        /// <returns></returns>
        public static bool ShouldMouseBeEnabled()
        {
            for(int i = 0; i < registerdHandelers.Count; i++)
            {
                if(registerdHandelers[i]())
                    return true;
            }

            return false;
        }

    }
}
