using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using UnityEngine;
using Bolt;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to share local mods with others on the same server
    /// </summary>
    public class ModSharingManager : Singleton<ModSharingManager>
    {
        const string MESSAGE_PREFIX = "[SharedMod]";

        const char SEPERATOR_CHAR = '█';

        /// <summary>
        /// Sends a request to all other Mod-Bot clients to download the passed byte[] and load it as a mod
        /// </summary>
        /// <param name="modId"></param>
        public static void SendModToAllModBotClients(string modId)
        {
            string localPlayfabID = MultiplayerLoginManager.Instance.GetLocalPlayFabID();

            string messageToSend = MESSAGE_PREFIX + localPlayfabID + SEPERATOR_CHAR + modId;

            MultiplayerMessageSender.SendToAllClients(messageToSend, GlobalTargets.Others);
        }

        /// <summary>
        /// Called when the client recives a GenericStringForModdingEvent event
        /// </summary>
        /// <param name="moddingEvent"></param>
        public void OnModdedEvent(GenericStringForModdingEvent moddingEvent)
        {
            string messageData = moddingEvent.EventData;

            if (!messageData.StartsWith(MESSAGE_PREFIX))
                return;

            messageData = messageData.Substring(MESSAGE_PREFIX.Length);

            string[] data = messageData.Split(SEPERATOR_CHAR);

            if (data.Length != 2) // if the event is in a invalid format, skip it
                return;

            string playfabId = data[0];
            string modId = data[1];

            ModBotUIRoot.Instance.ModSuggestingUI.SuggestModMultiplayer(playfabId, modId);
        }
    }
}
