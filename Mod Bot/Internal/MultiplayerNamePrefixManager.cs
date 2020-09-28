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
		static Dictionary<string, string> playfabIDToOverrideNameDictionary = new Dictionary<string, string>();

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
			bool isDictionaryNullOrEmpty = false;
			if (playfabIDToCustomPrefixDictionary == null || playfabIDToCustomPrefixDictionary.Count == 0)
				isDictionaryNullOrEmpty = true;

			string prefix = "";
			
			if(!isDictionaryNullOrEmpty && playfabIDToCustomPrefixDictionary.ContainsKey(playfabID))
				prefix += playfabIDToCustomPrefixDictionary[playfabID] + " ";

			if (ModBotUserIdentifier.Instance != null && ModBotUserIdentifier.Instance.IsUsingModBot(playfabID))
			{
				if (!isDictionaryNullOrEmpty && playfabIDToCustomPrefixDictionary.TryGetValue(MOD_BOT_USER_KEY, out string modBotUserPrefix))
				{
					prefix += modBotUserPrefix + " ";
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
			if (playfabIDToOverrideNameDictionary != null && playfabIDToOverrideNameDictionary.Count != 0 && playfabIDToOverrideNameDictionary.TryGetValue(playfabID, out string overrideName))
				return overrideName;

			return defaultName;
		}
		
		internal static IEnumerator DownloadDataFromFirebase()
		{
			UnityWebRequest prefixesReqeust = UnityWebRequest.Get(PREFIXES_URL);
			yield return prefixesReqeust.SendWebRequest();
			
			string prefixJson = prefixesReqeust.downloadHandler.text;
			playfabIDToCustomPrefixDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(prefixJson);

			UnityWebRequest nameOverrideRequest = UnityWebRequest.Get(NAME_OVERRIDE_URL);
			yield return nameOverrideRequest.SendWebRequest();

			string nameOverrideJson = nameOverrideRequest.downloadHandler.text;
			playfabIDToOverrideNameDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(nameOverrideJson);

			TriggerRefreshNameTagsEvent();
		}
		internal static void OnNameTagRefreshed(EnemyNameTag nameTag, string ownerPlayfabID)
		{
			MultiplayerPlayerInfoManager.Instance.TryGetDisplayName(ownerPlayfabID, delegate (string displayName)
			{
				nameTag.NameText.text = displayName;
			});
		}
	}
}
