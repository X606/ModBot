using InternalModBot;
using ModBotWebsiteAPI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ModLibrary
{
    /// <summary>
    /// Handles singing into Mod-Bot and used to find out if other players are using Mod-Bot
    /// </summary>
    public class ModBotUserIdentifier : Singleton<ModBotUserIdentifier>
    {
        private const string CLIENT_CONNECTED_PREFIX = "[ClientConnected]";
        private const string BROADCAST_PLAYFAB_ID_PREFIX = "[PlayfabIDBroadcast]";

        public const string USER_SIGN_IN_ATTEMPT_EVENT = "ModBotUserSignInAttempt";

        private static readonly string s_sessionIdFilePath = Application.persistentDataPath + "/SessionID.txt";

        private static string _signInError;

        private static string _userName = string.Empty;
        /// <summary>
        /// The name of local user
        /// </summary>
        public static string UserName
        {
            get => _userName;
        }

        private static string _userNameColored = string.Empty;
        /// <summary>
        /// The name of local user with color
        /// </summary>
        public static string UserNameColored
        {
            get => _userNameColored;
        }

        private static string _userId = string.Empty;
        /// <summary>
        /// The ID of local user
        /// </summary>
        public static string UserID
        {
            get => _userId;
        }

        private static SignInStatuses _signInStatus;
        /// <summary>
        /// Current status of user authorization
        /// </summary>
        public static SignInStatuses SignInStatus
        {
            get => _signInStatus;
        }

        private static readonly List<string> s_playFabIDs = new List<string>();

        private static int s_signInAttempts;

        private void Start()
        {
            TrySingIn();
        }

        /// <summary>
        /// Sets the current session in the API
        /// </summary>
        /// <param name="sessionId"></param>
        internal void SetSession(string sessionId)
        {
            API.SetSessionID(sessionId);
            File.WriteAllText(s_sessionIdFilePath, sessionId);
        }

        internal bool CanSignIn() => MultiplayerLoginManager.Instance.IsLoggedIntoPlayfab();

        internal void TrySingIn()
        {
            if (SignInStatus == SignInStatuses.Success) return;

            if (!File.Exists(s_sessionIdFilePath))
            {
                setSignInStatus(SignInStatuses.None);
                return;
            }
            setSignInStatus(SignInStatuses.InProgress);

            string sessionId = File.ReadAllText(s_sessionIdFilePath);
            API.SetSessionID(sessionId);

            API.IsValidSession(sessionId, delegate (string data)
            {
                if (data == "false")
                {
                    File.Delete(s_sessionIdFilePath);
                    SignOut();
                    return;
                }
                onSignedIn();
            });
        }

        internal void TrySignInWithCredentials(string username, string password)
        {
            _signInError = null;
            setSignInStatus(SignInStatuses.InProgress);
            API.SignInFromGame(username, password, MultiplayerLoginManager.Instance.GetLocalPlayFabID(), onSignInInfoReceived);
        }

        internal void SignOut()
        {
            SetSession(string.Empty);
            setSignInStatus(SignInStatuses.None);
        }

        internal bool HasFailedToSignIn() => !string.IsNullOrEmpty(_signInError);

        internal string GetSignInError() => _signInError;

        internal void OnLocalClientConnectedToMultiplayer()
        {
            string localPlayfabID = MultiplayerLoginManager.Instance.GetLocalPlayFabID();
            MultiplayerMessageSender.SendToAllClients(CLIENT_CONNECTED_PREFIX + localPlayfabID);
        }

        /// <summary>
        /// Called when we recive a modded event
        /// </summary>
        /// <param name="moddedEvent"></param>
        internal void OnMultiplayerEvent(GenericStringForModdingEvent moddedEvent)
        {
            string message = moddedEvent.EventData;

            if (message.StartsWith(CLIENT_CONNECTED_PREFIX))
            {
                string playfabID = message.Substring(CLIENT_CONNECTED_PREFIX.Length);
                onClientConnectedMessageRecived(playfabID);
            }
            else if (message.StartsWith(BROADCAST_PLAYFAB_ID_PREFIX))
            {
                string playfabID = message.Substring(BROADCAST_PLAYFAB_ID_PREFIX.Length);
                onPlayfabIDBroadcastMessageRecived(playfabID);
            }
        }

        /// <summary>
        /// Returns if the player with the target playfabID is running Mod-Bot
        /// </summary>
        /// <param name="playfabID"></param>
        /// <returns></returns>
        public bool IsUsingModBot(string playfabID) => s_playFabIDs.Contains(playfabID);

        private void onSignInInfoReceived(JsonObject json)
        {
            string error;
            try
            {
                error = Convert.ToString(json["Error"]);
            }
            catch (Exception)
            {
                setSignInStatus(SignInStatuses.Failed);
                _signInError = "Could not connect to server";
                return;
            }

            if (!string.IsNullOrEmpty(error) && error != "null")
            {
                setSignInStatus(SignInStatuses.Failed);
                _signInError = error;
                return;
            }

            _signInError = null;

            string sessionID = Convert.ToString(json["sessionID"]).Trim('\"');
            SetSession(sessionID);

            onSignedIn();
        }

        void onSignedIn()
        {
            s_signInAttempts++;
            setSignInStatus(SignInStatuses.GettingUserInfo);
            API.GetCurrentUser(delegate (string userId)
            {
                API.GetUser(userId, delegate (JsonObject json)
                {
                    string username;
                    string usernameColored;
                    try
                    {
                        username = Convert.ToString(json["username"]).Trim('\"');
                        usernameColored = "<color=" + Convert.ToString(json["color"]) + ">" + username + "</color>";
                    }
                    catch (Exception)
                    {
                        if (s_signInAttempts >= 3)
                        {
                            setSignInStatus(SignInStatuses.Failed);
                            GlobalEventManager.Instance.Dispatch(USER_SIGN_IN_ATTEMPT_EVENT);
                            return;
                        }

                        DelegateScheduler.Instance.Schedule(onSignedIn, 2f);
                        return;
                    }

                    debug.Log("Logged in as " + username.Trim('\"'));

                    _userId = userId;
                    _userName = username;
                    _userNameColored = usernameColored;
                    setSignInStatus(SignInStatuses.Success);
                    GlobalEventManager.Instance.Dispatch(USER_SIGN_IN_ATTEMPT_EVENT);
                });
            });
        }

        private void onClientConnectedMessageRecived(string playfabID)
        {
            if (!s_playFabIDs.Contains(playfabID)) s_playFabIDs.Add(playfabID);

            string localPlayfabID = MultiplayerLoginManager.Instance.GetLocalPlayFabID();
            MultiplayerMessageSender.SendToAllClients(BROADCAST_PLAYFAB_ID_PREFIX + localPlayfabID);
        }

        private void onPlayfabIDBroadcastMessageRecived(string playfabID)
        {
            if (!s_playFabIDs.Contains(playfabID)) s_playFabIDs.Add(playfabID);
            MultiplayerPlayerNameManager.Instance.TriggerRefreshNameTagsEvent();
        }

        private static void setSignInStatus(SignInStatuses status)
        {
            _signInStatus = status;

            VersionLabelManager versionLabelManager = VersionLabelManager.Instance;
            if (!versionLabelManager) return;

            switch (status)
            {
                case SignInStatuses.None:
                    versionLabelManager.SetLine(2, "Not signed in");
                    break;
                case SignInStatuses.InProgress:
                    versionLabelManager.SetLine(2, "Signing in...");
                    break;
                case SignInStatuses.GettingUserInfo:
                    versionLabelManager.SetLine(2, "Getting user info...");
                    break;
                case SignInStatuses.Success:
                    versionLabelManager.SetLine(2, $"Signed in as: {UserNameColored}");
                    break;
                case SignInStatuses.Failed:
                    versionLabelManager.SetLine(2, "<color=#990000>Authorization failed</color>");
                    break;
                default:
                    versionLabelManager.SetLine(2, "hello");
                    break;
            }
        }

        public enum SignInStatuses
        {
            None,
            InProgress,
            GettingUserInfo,
            Success,
            Failed,
        }
    }
}