using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bolt;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to catch events
    /// </summary>
    public class ModdedMultiplayerEventListener : GlobalEventListener
    {
        /// <summary>
        /// Called when we get a <see cref="GenericStringForModdingEvent"/> event
        /// </summary>
        /// <param name="evnt"></param>
        public override void OnEvent(GenericStringForModdingEvent evnt)
        {
            ModsManager.Instance.PassOnMod.OnMultiplayerEventReceived(evnt);
        }
    }
}
