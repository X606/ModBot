using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using InternalModBot;

namespace ModLibrary
{
    /// <summary>
    /// Used to find out other players <see cref="BoltConnection"/> and if they are using Mod-Bot
    /// </summary>
    public class ModBotUserIdentifier : Singleton<ModBotUserIdentifier>
    {
        List<string> _playFabIDs = new List<string>();

		const string CLIENT_CONNECTED_PREFIX = "[ClientConnected]";
		const string BROADCAST_PLAYFAB_ID_PREFIX = "[PlayfabIDBroadcast]";

        /// <summary>
        /// Returns if the player with the target playfabID is running Mod-Bot
        /// </summary>
        /// <param name="playfabID"></param>
        /// <returns></returns>
        public bool IsUsingModBot(string playfabID)
        {
            return _playFabIDs.Contains(playfabID);
        }

        internal void OnLocalClientConnected()
		{
			string localPlayfabID = MultiplayerLoginManager.Instance.GetLocalPlayFabID();

			MultiplayerMessageSender.SendToAllClients(CLIENT_CONNECTED_PREFIX + localPlayfabID);
		}

		void onClientConnectedMessageRecived(string playfabID)
		{
			if (!_playFabIDs.Contains(playfabID))
			{
				_playFabIDs.Add(playfabID);
			}

			string localPlayfabID = MultiplayerLoginManager.Instance.GetLocalPlayFabID();
			MultiplayerMessageSender.SendToAllClients(BROADCAST_PLAYFAB_ID_PREFIX + localPlayfabID);
		}
		void onPlayfabIDBroadcastMessageRecived(string playfabID)
		{
			if(!_playFabIDs.Contains(playfabID))
			{
				_playFabIDs.Add(playfabID);
			}
			MultiplayerPlayerNameManager.Instance.TriggerRefreshNameTagsEvent();
		}

        /// <summary>
        /// Called when we recive a modded event
        /// </summary>
        /// <param name="moddedEvent"></param>
        internal void OnEvent(GenericStringForModdingEvent moddedEvent)
        {
			
			string message = moddedEvent.EventData;

			if (message.StartsWith(CLIENT_CONNECTED_PREFIX))
			{
				string playfabID = message.Substring(CLIENT_CONNECTED_PREFIX.Length);
				onClientConnectedMessageRecived(playfabID);
			}
			else if(message.StartsWith(BROADCAST_PLAYFAB_ID_PREFIX))
			{
				string playfabID = message.Substring(BROADCAST_PLAYFAB_ID_PREFIX.Length);
				onPlayfabIDBroadcastMessageRecived(playfabID);
			}

		}

    }
}
