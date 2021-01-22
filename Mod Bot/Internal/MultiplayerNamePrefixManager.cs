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
using ModBotWebsiteAPI;

namespace InternalModBot
{
	/// <summary>
	/// Used by mod-bot to manage custom name tags in multiplayer
	/// </summary>
	public class MultiplayerPlayerNameManager : Singleton<MultiplayerPlayerNameManager>
	{
		const string MOD_BOT_USER_KEY = "ModBotUser";

		const string DEFUALT_MOD_BOT_USER_PREFIX = "<color=#ffac00>[Mod-Bot]</color>";
		const string PREFIXES_URL = "https://modbot-d8a58.firebaseio.com/playerPrefixes/.json";
		const string NAME_OVERRIDE_URL = "https://modbot-d8a58.firebaseio.com/playerNameOverrides/.json";

		Dictionary<string, string> playfabIDToCustomPrefixDictionary = new Dictionary<string, string>();
		Dictionary<string, string> playfabIDToOverrideNameDictionary = new Dictionary<string, string>();

		void Start()
		{
			GlobalEventManager.Instance.AddEventListener(GlobalEvents.MultiplayerPlayerInfoStateAttached, new Action<IPlayerInfoState>(onPlayerInfoStateAttachced));
		}
		void OnDestroy()
		{
			GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.MultiplayerPlayerInfoStateAttached, new Action<IPlayerInfoState>(onPlayerInfoStateAttachced));
		}

		void onPlayerInfoStateAttachced(IPlayerInfoState playerInfoState)
		{
			string playfabID = playerInfoState.PlayFabID;
			API.GetPlayerPrefix(playfabID, delegate (JsonObject json)
			{
				string nameOverride = Convert.ToString(json["nameOverride"]);
				string prefix = Convert.ToString(json["prefix"]);

				bool useOverrideName = !string.IsNullOrEmpty(nameOverride);
				bool usePrefix = !string.IsNullOrEmpty(prefix);

				if (useOverrideName)
				{
					playfabIDToOverrideNameDictionary[playfabID] = nameOverride;
				}
				if (usePrefix)
				{
					playfabIDToCustomPrefixDictionary[playfabID] = prefix;
				}

				if (useOverrideName || usePrefix)
				{
					TriggerRefreshNameTagsEvent();
				}
				
			});

		}

		/// <summary>
		/// Will be called when we want to refresh name tags
		/// </summary>
		public event Action RefreshNameTags;

		internal void TriggerRefreshNameTagsEvent()
		{
			if(RefreshNameTags != null)
				RefreshNameTags();
			
		}

		/// <summary>
		/// Gets the full prefix for a player from their playfabID
		/// </summary>
		/// <param name="playfabID"></param>
		/// <returns></returns>
		public string GetFullPrefixForPlayfabID(string playfabID)
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
		public string GetNameForPlayfabID(string playfabID, string defaultName)
		{
			if (playfabIDToOverrideNameDictionary != null && playfabIDToOverrideNameDictionary.Count != 0 && playfabIDToOverrideNameDictionary.TryGetValue(playfabID, out string overrideName))
				return overrideName;

			return defaultName;
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
