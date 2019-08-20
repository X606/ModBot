using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TwitchChatter;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    public class ModSuggestingManager : Singleton<ModSuggestingManager>
    {
        private void Start()
        {
            TwitchChatClient.singleton.AddChatListener(new ChatMessageNotificationDelegate(OnTwitchChatMessage));
        }

        private void Update()
        {
            if (!IsInSuggestMode)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                Accept();
                IsInSuggestMode = false;
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                Deny();
                IsInSuggestMode = false;
            }
        }

        public void SuggestMod()
        {
            Ani.Play("suggestMod");
            CreatorName.text = "Suggested By: " + Suggester;
            ModName.text = ModNameString;
            IsInSuggestMode = true;
        }

        public void Accept()
        {
            TwitchManager.Instance.EnqueueChatMessage("Mod accepted. :)");
            Ani.Play("AcceptMod");
            IsInSuggestMode = false;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(MyRemoteCertificateValidationCallback);
                byte[] response = new WebClient
                {
                    Headers =
                {
                    "User-Agent: Other"
                }
                }.DownloadData(Url);
                Singleton<ModsManager>.Instance.LoadMod(response);
            }
            catch (Exception e)
            {
                Singleton<Logger>.Instance.Log(e.Message, Color.red);
            }
        }

        public void Deny()
        {
            TwitchManager.Instance.EnqueueChatMessage("Mod denied. :(");
            Ani.Play("DenyMod");
            IsInSuggestMode = false;
        }

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
                if (subCommands.Length >= 3)
                {
                    Url = subCommands[2];
                    Suggester = "<color=" + msg.userNameColor + ">" + msg.userName + "</color>";
                    ModNameString = subCommands[1];
                    SuggestMod();
                    TwitchManager.Instance.EnqueueChatMessage("Mod suggested!");
                }
                else
                {
                    TwitchManager.Instance.EnqueueChatMessage("Usage: !modsuggest <mod_name> <mod_link>");
                }
            }

        }

        public bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
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

        public Animator Ani;

        public bool IsInSuggestMode;

        public Text ModName;

        public Text CreatorName;

        public string Url;

        public string Suggester;

        public string ModNameString;
    }

}
