using ModBotWebsiteAPI;
using ModLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace InternalModBot
{
    /// <summary>
    /// Used by mod-bot to manage custom name tags in multiplayer
    /// </summary>
    internal class MultiplayerPlayerNameManager : Singleton<MultiplayerPlayerNameManager>
    {
        const string MOD_BOT_USER_KEY = "ModBotUser";

        const string DEFAULT_MOD_BOT_USER_PREFIX = "<color=#ffac00>[Mod-Bot]</color>";

        public const string REFRESH_NAME_TAGS_EVENT = "RefreshNameTags";

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

        internal void TriggerRefreshNameTagsEvent()
        {
            GlobalEventManager.Instance.Dispatch(REFRESH_NAME_TAGS_EVENT);
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
            StringBuilder stringBuilder = new StringBuilder();

            // append custom tags
            if (!ModBotPrefs.HideCustomTags && playfabIDToCustomPrefixDictionary.TryGetValue(playfabID, out string customPrefix))
            {
                stringBuilder.Append(customPrefix);
                stringBuilder.Append(' ');
            }

            // append modbot tag
            if (ModBotUserIdentifier.Instance.IsUsingModBot(playfabID))
            {
                stringBuilder.Append(DEFAULT_MOD_BOT_USER_PREFIX);
                stringBuilder.Append(' ');
            }

            return stringBuilder.ToString();
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
    }
}