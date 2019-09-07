using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using UnityEngine;
using Bolt;

namespace InternalModBot {

    /// <summary>
    /// Used by Mod-Bot to share local mods with others on the same server
    /// </summary>
    public class ModSharingManager : Singleton<ModSharingManager>
    {
        private const int MAX_MESSAGE_LENGTH = 512;
        private const string MESSAGE_PREFIX = "[ModData]";
        private const string CHARACTERS_TO_USE_IN_IDS = "qwertyuiopasdfghjklzxcvbnm1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
        private const char SEPERATOR_CHAR = '█';
        
        /// <summary>
        /// Sends a request to all other Mod-Bot clients to download the passed byte[] and load it as a mod
        /// </summary>
        /// <param name="data"></param>
        /// <param name="modName"></param>
        public void SendModToAllModBotClients(byte[] data, string modName)
        {
            string dataAsString = data.RawBytesToString();

            int messageAmountToSend = Mathf.CeilToInt(dataAsString.Length / (float)MAX_MESSAGE_LENGTH);

            string id = GenerateID(4);

            for (int i = 0; i < messageAmountToSend; i++)
            {
                string messageToSend = null;

                int startIndex = i * MAX_MESSAGE_LENGTH;
                int length = MAX_MESSAGE_LENGTH;
                if (dataAsString.Length > startIndex + MAX_MESSAGE_LENGTH)
                {
                    messageToSend = dataAsString.Substring(startIndex, length);
                } else
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

            AddToDownloadedData(id, index, amountOfMessagesToSend, data, senderPlayfabId, modName);
        }

        private void AddToDownloadedData(string id, int index, int amountOfMessagesToSend, string data, string senderPlayfabID, string modName)
        {
            if (!DownloadingData.ContainsKey(id))
            {
                string[] allData = new string[amountOfMessagesToSend];
                DownloadingData.Add(id, allData);
            }
            DownloadingData[id][index] = data;

            if (!HasDownloadedAllParts(DownloadingData[id]))
                return;

            string fullDataAsString = string.Concat(DownloadingData[id]);
            byte[] fullData = fullDataAsString.ToBytes();

            ModSuggestingManager.Instance.SuggestModMultiplayer(senderPlayfabID, modName, fullData);
            DownloadingData.Remove(id);
        }

        private bool HasDownloadedAllParts(string[] data)
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

        /// <summary>
        /// Used to store the incomplete downloaded data
        /// </summary>
        public Dictionary<string, string[]> DownloadingData = new Dictionary<string, string[]>();

        private string GenerateID(int length)
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


    /// <summary>
    /// Adds the ToBytes method to <see langword="string"/> and the RawBytesToString method to <see langword="byte"/>[]
    /// </summary>
    public static class StringAndByteArrayConverterExtensions
    {
        /// <summary>
        /// Converts each <see langword="char"/> in this <see langword="string"/> to a <see langword="byte"/>, and puts them in a <see langword="bye"/>[] in order
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string me)
        {
            byte[] bytes = new byte[me.Length];
            for (int i = 0; i < me.Length; i++)
            {
                byte byteValue = (byte)me[i];
                bytes[i] = byteValue;
            }

            return bytes;
        }
        /// <summary>
        /// Converts each <see langword="byte"/> in the <see langword="byte"/>[] to a <see langword="char"/>, then combines them to a <see langword="string"/> in order
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static string RawBytesToString(this byte[] me)
        {
            char[] characters = new char[me.Length];
            for (int i = 0; i < me.Length; i++)
            {
                char character = (char)me[i];
                characters[i] = character;
            }

            return new string(characters);
        }
    }

}
