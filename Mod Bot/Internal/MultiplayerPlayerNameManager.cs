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
using HarmonyLib;

namespace InternalModBot
{
	/// <summary>
	/// Used by mod-bot to manage custom name tags in multiplayer
	/// </summary>
	internal class MultiplayerPlayerNameManager : Singleton<MultiplayerPlayerNameManager>
	{
		const string MOD_BOT_USER_KEY = "ModBotUser";

		const string DEFAULT_MOD_BOT_USER_PREFIX = "<color=#ffac00>[Mod-Bot]</color>";

		Dictionary<string, string> playfabIDToCustomPrefixDictionary = new Dictionary<string, string>();
		Dictionary<string, string> playfabIDToOverrideNameDictionary = new Dictionary<string, string>();

		void Start()
		{
			GlobalEventManager.Instance.AddEventListener<IPlayerInfoState>(GlobalEvents.MultiplayerPlayerInfoStateAttached, onPlayerInfoStateAttached);
		}
		void OnDestroy()
		{
			GlobalEventManager.Instance.RemoveEventListener<IPlayerInfoState>(GlobalEvents.MultiplayerPlayerInfoStateAttached, onPlayerInfoStateAttached);
		}

		void onPlayerInfoStateAttached(IPlayerInfoState playerInfoState)
        {
            if (playerInfoState == null) return;
			StartCoroutine(waitForPlayerInfoStateToInitialize(playerInfoState));
		}

		IEnumerator waitForPlayerInfoStateToInitialize(IPlayerInfoState playerInfoState)
		{
			float timeout = Time.unscaledTime + 5f;
			while (Time.unscaledTime < timeout && !playerInfoState.IsDisconnected && string.IsNullOrWhiteSpace(playerInfoState.PlayFabID))
				yield return null;

			string playFabId = playerInfoState.PlayFabID;
            if (string.IsNullOrWhiteSpace(playFabId)) yield break;

            API.GetPlayerPrefix(playFabId, json => onPlayerNameDataReceived(json, playFabId));
            yield break;
		}

		void onPlayerNameDataReceived(JsonObject json, string playfabID)
        {
			string nameOverride;
			string prefix;
			try
			{
				nameOverride = Convert.ToString(json["nameOverride"]);
				prefix = Convert.ToString(json["prefix"]);
			}
			catch (NullReferenceException)
			{
				DelegateScheduler.Instance.Schedule(() => API.GetPlayerPrefix(playfabID, j => onPlayerNameDataReceived(j, playfabID)), 2f);
				return;
			}

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

		public string GetCompleteNameForPlayer(MultiplayerPlayerInfoState playerInfoState, string normalDisplayName)
        {
			return getFullPrefixForPlayfabID(playerInfoState.state.PlayFabID) + getNameForPlayfabID(playerInfoState.state.PlayFabID, normalDisplayName);
		}

		/// <summary>
		/// Gets the full prefix for a player from their playfabID
		/// </summary>
		/// <param name="playfabID"></param>
		/// <returns></returns>
		string getFullPrefixForPlayfabID(string playfabID)
		{
			string prefix = "";
			
			if (playfabIDToCustomPrefixDictionary.TryGetValue(playfabID, out string customPrefix))
				prefix += customPrefix + " ";

			if (ModBotUserIdentifier.Instance != null && ModBotUserIdentifier.Instance.IsUsingModBot(playfabID))
			{
				if (playfabIDToCustomPrefixDictionary.TryGetValue(MOD_BOT_USER_KEY, out string modBotUserPrefix))
				{
					prefix += modBotUserPrefix + " ";
				}
				else
				{
					prefix += DEFAULT_MOD_BOT_USER_PREFIX + " ";
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
		string getNameForPlayfabID(string playfabID, string defaultName)
		{
            return playfabIDToOverrideNameDictionary.TryGetValue(playfabID, out string overrideName) ? overrideName : defaultName;
        }

        internal static void OnNameTagRefreshed(EnemyNameTag nameTag, string ownerPlayfabID)
		{
			MultiplayerPlayerInfoManager.Instance.TryGetDisplayName(ownerPlayfabID, delegate (string displayName)
			{
				nameTag.NameText.text = displayName;
			});
		}

		[HarmonyPatch]
		static class Patches
		{
			[HarmonyPostfix]
			[HarmonyPatch(typeof(CurrentlySpectatingUI), "Show")]
			static void CurrentlySpectatingUI_Show_Postfix(CurrentlySpectatingUI __instance)
			{
				__instance.CurrentPlayerText.supportRichText = true;
			}

			[HarmonyPostfix]
			[HarmonyPatch(typeof(EnemyNameTag), "Initialize")]
			static void EnemyNameTag_Initialize_Postfix(EnemyNameTag __instance, Character character)
			{
				if (MultiplayerPlayerInfoManager.Instance != null && MultiplayerPlayerInfoManager.Instance.GetPlayerInfoState(character.state.PlayFabID) != null)
				{
					__instance.NameText.supportRichText = true;
					__instance.gameObject.AddComponent<NameTagRefreshListener>().Init(character, __instance);
				}
			}

			[HarmonyPostfix]
			[HarmonyPatch(typeof(MultiplayerPlayerInfoLabel), "Initialize")]
			static void MultiplayerPlayerInfoLabel_Initialize_Postfix(MultiplayerPlayerInfoLabel __instance)
			{
				__instance.PlayerNameLabel.supportRichText = true; // Support custom colors and bold/italic text
				__instance.PlayerNameLabel.horizontalOverflow = HorizontalWrapMode.Overflow;
			}

			[HarmonyPrefix]
			[HarmonyPatch(typeof(MultiplayerPlayerInfoState), "GetOrPrepareSafeDisplayName")]
			static void MultiplayerPlayerInfoState_GetOrPrepareSafeDisplayName_Prefix(MultiplayerPlayerInfoState __instance, ref Action<string> onSafeDisplayNameReceived)
			{
				if (onSafeDisplayNameReceived != null)
				{
					// Create new callback delegate that invokes the original with the name given by MultiplayerPlayerNameManager
					Action<string> callbackCopy = onSafeDisplayNameReceived;
					onSafeDisplayNameReceived = delegate (string safeDisplayName)
					{
						string name;
						if (MultiplayerPlayerNameManager.Instance != null)
						{
							name = MultiplayerPlayerNameManager.Instance.GetCompleteNameForPlayer(__instance, safeDisplayName);
						}
						else
						{
							name = safeDisplayName;
						}

						callbackCopy(name);
					};
				}
			}

			[HarmonyPrefix]
			[HarmonyPatch(typeof(BlockListMultiplayerEntryUI), nameof(BlockListMultiplayerEntryUI.Initialize))]
			static void BlockListMultiplayerEntryUI_Initialize_Prefix(BlockListMultiplayerEntryUI __instance)
            {
				__instance.DisplayNameText.supportRichText = true;
            }
		}
	}
}
