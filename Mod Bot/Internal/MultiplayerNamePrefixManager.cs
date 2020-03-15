using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace InternalModBot
{
	/// <summary>
	/// Used by mod-bot to manage custom name tags in multiplayer
	/// </summary>
	public static class MultiplayerPlayerNameManager
	{
		const string MOD_BOT_USER_KEY = "ModBotUser";

		const string DEFUALT_MOD_BOT_USER_PREFIX = "<color=#ffac00>[Mod-Bot]</color>";
		const string PREFIXES_URL = "https://modbot-d8a58.firebaseio.com/playerPrefixes/.json";
		const string NAME_OVERRIDE_URL = "https://modbot-d8a58.firebaseio.com/playerNameOverrides/.json";

		static Dictionary<string, string> playfabIDToCustomPrefixDictionary = new Dictionary<string, string>();
		static Dictionary<string, string> playfabIDToOverridenameDictionary = new Dictionary<string, string>();

		internal static Dictionary<string, Action> OnRefreshedListeners = new Dictionary<string, Action>();

		/// <summary>
		/// Will be called when we want to refresh name tags
		/// </summary>
		public static event Action RefreshNameTags;

		internal static void TriggerRefreshNameTagsEvent()
		{
			if(RefreshNameTags != null)
				RefreshNameTags();
			
		}

		/// <summary>
		/// Gets the full prefix for a player from their playfabID
		/// </summary>
		/// <param name="playfabID"></param>
		/// <returns></returns>
		public static string GetFullPrefixForPlayfabID(string playfabID)
		{
			string prefix = "";
			
			if(playfabIDToCustomPrefixDictionary.ContainsKey(playfabID))
				prefix += playfabIDToCustomPrefixDictionary[playfabID] + " ";

			if(ModBotUserIdentifier.Instance.IsUsingModBot(playfabID))
			{
				if(playfabIDToCustomPrefixDictionary.ContainsKey(MOD_BOT_USER_KEY))
				{
					prefix += playfabIDToCustomPrefixDictionary[MOD_BOT_USER_KEY] + " ";
				}
				else
				{
					prefix += DEFUALT_MOD_BOT_USER_PREFIX + " ";
				}
			}

			return prefix;
		}

		/// <summary>
		/// If there is a name override this will return said override, otherwise it will just return defualtName
		/// </summary>
		/// <param name="playfabID"></param>
		/// <param name="defaultName"></param>
		/// <returns></returns>
		public static string GetNameForPlayfabID(string playfabID, string defaultName)
		{
			if(!playfabIDToOverridenameDictionary.ContainsKey(playfabID))
				return defaultName;

			return playfabIDToOverridenameDictionary[playfabID];
		}
		
		internal static IEnumerator DownloadDataFromFirebase()
		{
			UnityWebRequest prefixesReqeust = UnityWebRequest.Get(PREFIXES_URL);
			yield return prefixesReqeust.SendWebRequest();
			
			string prefixJson = prefixesReqeust.downloadHandler.text;
			playfabIDToCustomPrefixDictionary = JsonConvert.DeserializeObject<Dictionary<string,string>>(prefixJson);

			UnityWebRequest nameOverrideRequest = UnityWebRequest.Get(NAME_OVERRIDE_URL);
			yield return nameOverrideRequest.SendWebRequest();

			string nameOverrideJson = nameOverrideRequest.downloadHandler.text;
			playfabIDToOverridenameDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(nameOverrideJson);

			TriggerRefreshNameTagsEvent();
		}
		internal static void OnNameTagRefreshed(EnemyNameTag nameTag, string ownerPlayfabID)
		{
			nameTag.NameText.text = MultiplayerPlayerInfoManager.Instance.TryGetDisplayName(ownerPlayfabID);
		}
	}
}
