using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;

namespace ModLibrary
{
    /// <summary>
    /// Used to find out other players <see cref="BoltConnection"/> and if they are using Mod-Bot
    /// </summary>
    public class ModBotUserIdentifier : Singleton<ModBotUserIdentifier>
    {
        private List<string> PlayFabIDs = new List<string>();
        
        private const string REQUEST_MESSAGE_PREFIX = "[IDRequest]";
        private const string RESPONSE_MESSAGE_PREFIX = "[IDResponse]";
        private const char SEPARATOR_CHAR = '█';

        /// <summary>
        /// Returns if the player with the target playfabID is running Mod-Bot
        /// </summary>
        /// <param name="playfabID"></param>
        /// <returns></returns>
        public bool IsUsingModBot(string playfabID)
        {
            return PlayFabIDs.Contains(playfabID);
        }

        /// <summary>
        /// Sends a request to get some info on all players with mod-bot
        /// </summary>
        internal void RequestIds(FirstPersonMover player)
        {
            string message = REQUEST_MESSAGE_PREFIX + SEPARATOR_CHAR + player.GetPlayFabID();
            MultiplayerMessageSender.SendToAllClients(message);
        }
        private void Respond(string playfabIDTarget)
        {
            FirstPersonMover player = CharacterTracker.Instance.GetPlayer();
            if (player == null)
            {
                DelegateScheduler.Instance.Schedule(delegate { Respond(playfabIDTarget); }, 1);
                return;
            }
                

            string message = RESPONSE_MESSAGE_PREFIX + SEPARATOR_CHAR + playfabIDTarget + SEPARATOR_CHAR + player.GetPlayFabID();
            MultiplayerMessageSender.SendToAllClients(message);
        }

        /// <summary>
        /// Called when we recive a modded event
        /// </summary>
        /// <param name="moddedEvent"></param>
        internal void OnEvent(GenericStringForModdingEvent moddedEvent)
        {

            string message = moddedEvent.EventData;

            if (message.StartsWith(REQUEST_MESSAGE_PREFIX))
            {
                string[] subMessages = message.Split(SEPARATOR_CHAR);

                if (!PlayFabIDs.Contains(subMessages[1]))
                {
                    PlayFabIDs.Add(subMessages[1]);

                    Respond(subMessages[1]);

                }
            }

            if (message.StartsWith(RESPONSE_MESSAGE_PREFIX))
            {
                string[] subMessages = message.Split(SEPARATOR_CHAR);
                string playfabID = MultiplayerLoginManager.Instance.GetLocalPlayFabID();
                if (subMessages[1] != playfabID) // if the message wasnt meant for us, dont do anything
                    return;

                if (!PlayFabIDs.Contains(subMessages[2]))
                {
                    PlayFabIDs.Add(subMessages[2]);

                }
            }


        }
    }
}
