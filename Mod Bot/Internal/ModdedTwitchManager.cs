using InternalModBot;
using ModLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchChatter;
using UnityEngine;

namespace InternalModBot
{
    /// <summary>
	/// Used by Mod-Bot to intercept messages from twitch chat.
	/// </summary>
    internal class ModdedTwitchManager : Singleton<ModdedTwitchManager>
    {
        private void Start()
        {
            if (TwitchChatClient.singleton != null)
            {
                TwitchChatClient.singleton.AddChatListener(new ChatMessageNotificationDelegate(this.HandleTwitchChatMessage));
            }
        }
        private void OnDestroy()
        {
            if (TwitchChatClient.singleton != null)
            {
                TwitchChatClient.singleton.RemoveChatListener(new ChatMessageNotificationDelegate(this.HandleTwitchChatMessage));
            }
        }
        public void HandleTwitchChatMessage(ref TwitchChatMessage message)
        {
            if (GameModeManager.IsOnTitleScreen())
                return;
            try
            {
                Singleton<ModsManager>.Instance.PassOnMod.OnTwitchChatMessage(message);
            }
            catch (Exception ex)
            {
                AudioManager.Instance.PlayClipGlobal(AudioLibrary.Instance.ErrorBeep);
                debug.Log(ModBotLocalizationManager.FormatLocalizedStringFromID("command_failed_message", new object[]
                {
                    message,
                    ex.Message
                }), Color.red);
                debug.Log(ex.StackTrace, Color.red);
            }
        }
    }
}
