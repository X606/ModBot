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
        const int MAX_MESSAGE_LENGTH = 512;
        const string MESSAGE_PREFIX = "[ModData]";
        const string CHARACTERS_TO_USE_IN_IDS = "qwertyuiopasdfghjklzxcvbnm1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
        const char SEPERATOR_CHAR = '█';

        Dictionary<string, string[]> _downloadingData = new Dictionary<string, string[]>();

        /// <summary>
        /// Sends a request to all other Mod-Bot clients to download the passed byte[] and load it as a mod
        /// </summary>
        /// <param name="data"></param>
        /// <param name="modName"></param>
        public static void SendModToAllModBotClients(byte[] data, string modName)
        {
            string dataAsString = data.RawBytesToString();

            int messageAmountToSend = Mathf.CeilToInt(dataAsString.Length / (float)MAX_MESSAGE_LENGTH);

            string id = generateID(4);

            for (int i = 0; i < messageAmountToSend; i++)
            {
                int startIndex = i * MAX_MESSAGE_LENGTH;
                int length = MAX_MESSAGE_LENGTH;

                string messageToSend;
                if (dataAsString.Length > startIndex + MAX_MESSAGE_LENGTH)
                {
                    messageToSend = dataAsString.Substring(startIndex, length);
                }
                else
                {
                    messageToSend = dataAsString.Substring(startIndex);
                }

                string localPlayfabID = MultiplayerLoginManager.Instance.GetLocalPlayFabID();
                messageToSend = MESSAGE_PREFIX + SEPERATOR_CHAR + messageAmountToSend + SEPERATOR_CHAR + id + SEPERATOR_CHAR + i + SEPERATOR_CHAR + localPlayfabID + SEPERATOR_CHAR + modName + SEPERATOR_CHAR + messageToSend;

                MultiplayerMessageSender.SendToAllClients(messageToSend, GlobalTargets.Others);
                
            }
            
        }

        /// <summary>
        /// Called when the client recives a GenericStringForModdingEvent event
        /// </summary>
        /// <param name="moddingEvent"></param>
        public void OnModdedEvent(GenericStringForModdingEvent moddingEvent)
        {
            string[] messageInfo = moddingEvent.EventData.Split(SEPERATOR_CHAR);
            
            if (messageInfo.Length != 7 || messageInfo[0] != MESSAGE_PREFIX)
                return;
            
            int amountOfMessagesToSend = Convert.ToInt32(messageInfo[1]);
            string id = messageInfo[2];
            int index = Convert.ToInt32(messageInfo[3]);
            string senderPlayfabId = messageInfo[4];
            string modName = messageInfo[5];
            string data = messageInfo[6];

            addToDownloadedData(id, index, amountOfMessagesToSend, data, senderPlayfabId, modName);
        }

        void addToDownloadedData(string id, int index, int amountOfMessagesToSend, string data, string senderPlayfabID, string modName)
        {
            if (!_downloadingData.ContainsKey(id))
            {
                string[] allData = new string[amountOfMessagesToSend];
                _downloadingData.Add(id, allData);
            }
            _downloadingData[id][index] = data;

            if (!hasDownloadedAllParts(_downloadingData[id]))
                return;

            string fullDataAsString = string.Concat(_downloadingData[id]);
            byte[] fullData = fullDataAsString.ToBytes();

            ModSuggestingManager.Instance.SuggestModMultiplayer(senderPlayfabID, modName, fullData);
            _downloadingData.Remove(id);
        }

        static bool hasDownloadedAllParts(string[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    return false;
                }
                    
            }
            return true;
        }

        static string generateID(int length)
        {
            string returnValue = "";
            for (int i = 0; i < length; i++)
            {
                char randomCharacter = CHARACTERS_TO_USE_IN_IDS[UnityEngine.Random.Range(0, CHARACTERS_TO_USE_IN_IDS.Length)];
                returnValue += randomCharacter;
            }
            return returnValue;
        }
    }

}
