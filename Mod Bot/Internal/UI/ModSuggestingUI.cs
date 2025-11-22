using ModLibrary;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TwitchChatter;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to control the Twich mode mod suggesting
    /// </summary>
    internal class ModSuggestingUI : MonoBehaviour
    {
        /// <summary>
        /// The animator that plays the slide in and out animation
        /// </summary>
        public Animator ModSuggestionAnimator;

        /// <summary>
        /// Text text display where all info will be displayed
        /// </summary>
        public Text DisplayText;

        /// <summary>
        /// The mod ids of the mods the user has already rejected, static to persist across scene swiches
        /// </summary>
        public static HashSet<string> DeniedModIds = new HashSet<string>();

        /// <summary>
        /// Sets up the mod suggesting ui from a modded object
        /// </summary>
        /// <param name="moddedObject"></param>
        public void Init(ModdedObject moddedObject)
        {
            DisplayText = moddedObject.GetObject<Text>(0);
            ModSuggestionAnimator = moddedObject.GetObject<Animator>(1);
        }
        void Start()
        {
            TwitchChatClient.singleton.AddChatListener(OnTwitchChatMessage);
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.LevelSpawned, ShowNextInSuggestedModsQueue);
        }

        void OnDestroy()
        {
            if (TwitchChatClient.singleton != null)
                TwitchChatClient.singleton.RemoveChatListener(OnTwitchChatMessage);

            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.LevelSpawned, ShowNextInSuggestedModsQueue);
        }

        /// <summary>
        /// Shows the next in the suggested mods queue
        /// </summary>
        public void ShowNextInSuggestedModsQueue()
        {
            if (_modSuggestionQueue.Count == 0)
                return;

            ModSuggestion nextSuggestedMod = _modSuggestionQueue.Dequeue();
            StartCoroutine(suggestMod(nextSuggestedMod));
        }

        /// <summary>
        /// Brings up the suggest window for a multiplayer suggested mod
        /// </summary>
        /// <param name="suggesterPlayfabID"></param>
        /// <param name="modId"></param>
        public void SuggestModMultiplayer(string suggesterPlayfabID, string modId)
        {
            StartCoroutine(suggestModMultiplayerCoroutine(suggesterPlayfabID, modId));
        }

        IEnumerator suggestModMultiplayerCoroutine(string suggesterPlayfabID, string modId)
        {
            if (DeniedModIds.Contains(modId))
                yield break;

            if (ModsManager.Instance.GetLoadedModWithID(modId) != null)
                yield break;

            string displayName = null;

            bool hasDisplayName = false;
            MultiplayerPlayerInfoManager.Instance.TryGetDisplayName(suggesterPlayfabID, delegate (string receivedDisplayName)
            {
                displayName = receivedDisplayName;
                hasDisplayName = true;
            });

            yield return new WaitUntil(() => hasDisplayName);

            if (displayName == null)
                displayName = "";

            using (UnityWebRequest unityWebRequest = UnityWebRequest.Get("https://modbot.org/api?operation=getModData&id=" + modId))
            {
                unityWebRequest.timeout = 5;
                yield return unityWebRequest.SendWebRequest();

                if (!string.IsNullOrWhiteSpace(unityWebRequest.error))
                {
                    debug.Log("Error while trying to fetch mod data.", Color.red);
                    yield break;
                }

                try
                {
                    string response = unityWebRequest.downloadHandler.text;

                    if (response == "null")
                        yield break;

                    Dictionary<string, object> responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);


                    ModSuggestion suggestedMod = new ModSuggestion((string)responseDict["DisplayName"], (string)responseDict["Author"], displayName, modId);
                    _modSuggestionQueue.Enqueue(suggestedMod);

                    ShowNextInSuggestedModsQueue();
                }
                catch
                {
                    debug.Log("Something went wrong when trying to load the suggested mod", Color.red);
                    yield break;
                }
            }
        }

        IEnumerator suggestMod(ModSuggestion mod)
        {
            GameMode gameMode = GameFlowManager.Instance.GetCurrentGameMode();

            if (gameMode == GameMode.Twitch)
            {
                DisplayText.text = ModBotLocalizationManager.FormatLocalizedStringFromID("mod_suggested_twitch", mod.ModName, mod.SuggesterName);
            }
            else
            {
                DisplayText.text = ModBotLocalizationManager.FormatLocalizedStringFromID("mod_suggested_multiplayer", mod.SuggesterName, mod.ModName);
            }

            ModSuggestionAnimator.Play("suggestMod");

            bool accepted;
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.PageDown))
                {
                    accepted = false;
                    break;
                }
                else if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    accepted = true;
                    break;
                }

                yield return null;
            }

            if (accepted)
            {
                AudioManager.Instance.PlayClipGlobal(AudioLibrary.Instance.DogVoteUpZap);
                ModSuggestionAnimator.Play("AcceptMod");

                if (ModsManager.Instance.GetLoadedModWithID(mod.ModID) == null)
                {
                    if (gameMode == GameMode.Twitch) TwitchManager.Instance.EnqueueChatMessage("Mod accepted :)");

                    bool isDone = false;
                    ModsDownloadManager.DownloadMod(new ModsDownloadManager.ModGeneralInfo()
                    {
                        DisplayName = mod.ModName,
                        UniqueID = mod.ModID,
                        Version = 0
                    }, false, delegate
                    {
                        isDone = true;
                    });

                    while (!isDone) yield return null;
                }
                else
                {
                    if (gameMode == GameMode.Twitch) TwitchManager.Instance.EnqueueChatMessage("Mod already installed! FailFish");
                }

                ShowNextInSuggestedModsQueue();
            }
            else
            {
                AudioManager.Instance.PlayClipGlobal(AudioLibrary.Instance.DogVoteDown);
                ModSuggestionAnimator.Play("DenyMod");

                if (gameMode == GameMode.Twitch)
                    TwitchManager.Instance.EnqueueChatMessage("Mod denied :(");

                DeniedModIds.Add(mod.ModID);
            }

            yield return null;
            ShowNextInSuggestedModsQueue();
        }

        /// <summary>
        /// Gets called whenever anyone in twich chat sends a message
        /// </summary>
        /// <param name="msg"></param>
        public void OnTwitchChatMessage(ref TwitchChatMessage msg)
        {
            string lowerText = msg.chatMessagePlainText;
            if (!lowerText.StartsWith("!"))
                return;

            string[] subCommands = lowerText.Split(' ');

            if (subCommands[0].ToLower() == "!modsuggest")
            {
                if (subCommands.Length < 2)
                {
                    TwitchManager.Instance.EnqueueChatMessage("Usage: !modsuggest <modid>");
                    return;
                }
                string suggester = "<color=" + msg.userNameColor + ">" + msg.userName + "</color>";
                string modId = subCommands[1];

                StartCoroutine(onModSuggest(suggester, modId));

                return;
            }
            if (subCommands[0].ToLower() == "!mods")
            {
                List<Mod> mods = ModsManager.Instance.GetAllLoadedActiveMods();

                string allMods = "";
                for (int i = 0; i < mods.Count; i++)
                {
                    Mod mod = mods[i];

                    allMods += mod.ModInfo.DisplayName;

                    if (i != mods.Count - 1)
                    {
                        allMods += ", ";
                    }

                }

                TwitchManager.Instance.EnqueueChatMessage("MrDestructoid " + msg.userName + " Loaded mods: " + allMods + " MrDestructoid");
                return;
            }

        }

        IEnumerator onModSuggest(string suggester, string modId)
        {
            if (DeniedModIds.Contains(modId))
            {
                TwitchManager.Instance.EnqueueChatMessage("That mod has already been rejected FailFish");
                yield break;
            }
            if (ModsManager.Instance.GetLoadedModWithID(modId) != null)
            {
                TwitchManager.Instance.EnqueueChatMessage("Mod already installed! FailFish");
                yield break;
            }

            using (UnityWebRequest unityDataWebRequest = UnityWebRequest.Get("https://modbot.org/api?operation=getModData&id=" + modId))
            {
                using (UnityWebRequest unityVerifiedWebRequest = UnityWebRequest.Get("https://modbot.org/api?operation=isModVerified&modId=" + modId))
                {
                    unityDataWebRequest.timeout = 5;
                    unityVerifiedWebRequest.timeout = 5;
                    yield return unityDataWebRequest.SendWebRequest();
                    yield return unityVerifiedWebRequest.SendWebRequest();

                    if (!string.IsNullOrWhiteSpace(unityDataWebRequest.error) || !string.IsNullOrWhiteSpace(unityVerifiedWebRequest.error))
                    {
                        TwitchManager.Instance.EnqueueChatMessage("Error while trying to fetch mod data. MrDestructoid");
                        yield break;
                    }

                    try
                    {
                        if (unityVerifiedWebRequest.downloadHandler.text == "false")
                        {
                            TwitchManager.Instance.EnqueueChatMessage("That mod isn't verified SoonerLater");
                            yield break;
                        }

                        string response = unityDataWebRequest.downloadHandler.text;

                        if (response == "null")
                        {
                            TwitchManager.Instance.EnqueueChatMessage("No mod with the provided Id could be found NotLikeThis");
                            yield break;
                        }

                        Dictionary<string, object> responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);


                        ModSuggestion suggestedMod = new ModSuggestion((string)responseDict["DisplayName"], (string)responseDict["Author"], suggester, modId);
                        _modSuggestionQueue.Enqueue(suggestedMod);

                        TwitchManager.Instance.EnqueueChatMessage("Suggested the \"" + responseDict["DisplayName"] + "\" mod! BloodTrail");
                    }
                    catch
                    {
                        TwitchManager.Instance.EnqueueChatMessage("Something went wrong NotLikeThis");
                        yield break;
                    }
                }
            }
        }

        Queue<ModSuggestion> _modSuggestionQueue = new Queue<ModSuggestion>();

        struct ModSuggestion
        {
            public ModSuggestion(string modName, string modCreator, string suggesterName, string modID)
            {
                ModName = modName;
                ModCreator = modCreator;
                SuggesterName = suggesterName;
                ModID = modID;
            }

            public string ModName;
            public string ModCreator;
            public string SuggesterName;
            public string ModID;

        }

    }

}
