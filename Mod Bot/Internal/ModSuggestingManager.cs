using ModLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TwitchChatter;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to control the Twich mode mod suggesting
    /// </summary>
    public class ModSuggestingManager : Singleton<ModSuggestingManager>
    {
        private void Start()
        {
            TwitchChatClient.singleton.AddChatListener(new ChatMessageNotificationDelegate(OnTwitchChatMessage));
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.LevelSpawned, new Action(ShowNextInSuggestedModsQueue));
        }

        /// <summary>
        /// Shows the next in the suggested mods queue
        /// </summary>
        public void ShowNextInSuggestedModsQueue()
        {
            if (ModSuggestionQueue.Count == 0)
            {
                return;
            }

            ModSuggestion nextSuggestedMod = ModSuggestionQueue.Dequeue();
            StartCoroutine(SuggestMod(nextSuggestedMod));
        }
        
        /// <summary>
        /// Brings up the suggest window for a multiplayer suggested mod
        /// </summary>
        /// <param name="suggesterPlayfabID"></param>
        /// <param name="modName"></param>
        /// <param name="data"></param>
        public void SuggestModMultiplayer(string suggesterPlayfabID, string modName, byte[] data)
        {
            StartCoroutine(SuggestModMultiplayerIEnumerator(suggesterPlayfabID, modName, data));
        }

        private IEnumerator SuggestModMultiplayerIEnumerator(string suggesterPlayfabID, string modName, byte[] data)
        {
            string displayName = MultiplayerPlayerInfoManager.Instance.TryGetDisplayName(suggesterPlayfabID);

            if (displayName == null)
            {
                displayName = "";
            }

            displayText.text = "Mod download request!\n" +
                displayName + " wants to share a mod with you!\n" +
                "Mod name: \"" + modName + "\"";

            ModSuggestionAnimator.Play("suggestMod");

            KeyCode clickedKey;
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.PageDown))
                {
                    clickedKey = KeyCode.PageDown;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    clickedKey = KeyCode.PageUp;
                    break;
                }

                yield return 0;
            }

            if (clickedKey == KeyCode.PageUp)
            {
                ModSuggestionAnimator.Play("AcceptMod");
                try
                {
                    ModsManager.Instance.LoadMod(data, false);
                    debug.Log("Mod loaded from suggestion!", Color.green);
                }
                catch
                {
                    debug.Log("ERROR: Could not load mod loaded from multiplayer network");
                }
            }
            else if (clickedKey == KeyCode.PageDown)
            {
                ModSuggestionAnimator.Play("DenyMod");
            }

        }

        private IEnumerator SuggestMod(ModSuggestion mod)
        {
            displayText.text = "Mod suggested!\n"
                + mod.ModName + "\n" 
                + "Suggested by: " + mod.SuggesterName;
            ModSuggestionAnimator.Play("suggestMod");

            KeyCode clickedKey;
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.PageDown))
                {
                    clickedKey = KeyCode.PageDown;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    clickedKey = KeyCode.PageUp;
                    break;
                }

                yield return 0;
            }

            if (clickedKey == KeyCode.PageUp)
            {
                ModSuggestionAnimator.Play("AcceptMod");
                TwitchManager.Instance.EnqueueChatMessage("Mod accepted :)");
                UnityWebRequest webRequest = UnityWebRequest.Get(mod.Url);

                yield return webRequest.SendWebRequest();

                byte[] data = webRequest.downloadHandler.data;
                
                try
                {
                    ModsManager.Instance.LoadMod(data, false);
                }
                catch
                {
                    debug.Log("Suggested mod failed to load", Color.red);
                    TwitchManager.Instance.EnqueueChatMessage("Suggested mod \"" + mod.ModName + "\" failed to load, the link may be incorrect or the mod could be outdated.");
                }
            }
            if (clickedKey == KeyCode.PageDown)
            {
                ModSuggestionAnimator.Play("DenyMod");
                TwitchManager.Instance.EnqueueChatMessage("Mod denied :(");
            }

        }

        /// <summary>
        /// Gets called whenever anyone in twich chat sends a message
        /// </summary>
        /// <param name="msg"></param>
        public void OnTwitchChatMessage(ref TwitchChatMessage msg)
        {
            string lowerText = msg.chatMessagePlainText;
            if (!lowerText.StartsWith("!"))
            {
                return;
            }

            string[] subCommands = lowerText.Split(' ');

            if (subCommands[0].ToLower() == "!modsuggest")
            {
                if (subCommands.Length < 3)
                {
                    TwitchManager.Instance.EnqueueChatMessage("Usage: !modsuggest <mod_name> <mod_link>");
                    return;
                }

                string url = subCommands[2];
                string suggester = "<color=" + msg.userNameColor + ">" + msg.userName + "</color>";
                string modName = subCommands[1];
                ModSuggestion suggestedMod = new ModSuggestion(modName, suggester, url);
                ModSuggestionQueue.Enqueue(suggestedMod);

                if (!GameFlowManager.Instance.IsInCombat())
                {
                    ModSuggestion modSuggestion = ModSuggestionQueue.Dequeue();
                    StartCoroutine(SuggestMod(modSuggestion));
                }

                TwitchManager.Instance.EnqueueChatMessage("Mod suggested!");
            }

        }

        private byte[] DownloadData(string url)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(MyRemoteCertificateValidationCallback);
                byte[] response = new WebClient
                {
                    Headers =
                    {
                        "User-Agent: Other"
                    }
                }.DownloadData(url);

                return response;
            }
            catch (Exception e)
            {
                debug.Log(e.Message, Color.red);
            }

            return null;
        }

        private bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        if (!chain.Build((X509Certificate2)certificate))
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }

        /// <summary>
        /// The animator that plays the slide in and out animation
        /// </summary>
        public Animator ModSuggestionAnimator;

        /// <summary>
        /// Text text display where all info will be displayed
        /// </summary>
        public Text displayText;

        private Queue<ModSuggestion> ModSuggestionQueue = new Queue<ModSuggestion>();

        private struct ModSuggestion
        {
            public ModSuggestion(string modName, string suggesterName, string url)
            {
                ModName = modName;
                SuggesterName = suggesterName;
                Url = url;
            }

            public string ModName;
            public string SuggesterName;
            public string Url;

        }
    }

}
