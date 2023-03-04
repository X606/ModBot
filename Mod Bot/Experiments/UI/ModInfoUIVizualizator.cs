using ModBotWebsiteAPI;
using ModLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace InternalModBot
{
    internal class ModInfoUIVizualizator : MonoBehaviour
    {
        private bool m_HasInitialized;

        private int m_PrevLikeCount = -1;

        private JsonObject obj;
        private ModdedObject m_ModdedObject;
        private ModInfo m_ModInfo;
        private ModInfo m_InstalledModInfo;
        private Dictionary<string, JToken> m_SpecialData;

        private CanvasGroup m_CanvasGroup;
        private bool m_IsAnimating;

        private RawImage m_Thumbnail;
        private Transform m_NotVerifiedIcon;

        private Button m_DownloadButton;
        private Text m_DownloadCount;
        private Text m_DownloadedText;
        private Slider m_DownloadProgressBar;

        private Button m_LikeButton;
        private Text m_LikesCount;

        private Button m_MoreInfoButton;
        private Transform m_ControlsBG;

        public bool IsModInstalled => m_InstalledModInfo != null;
        public string ModName => m_ModInfo.DisplayName;
        public bool CanInteractWithSpecialData => ModBotSignInUI.HasSignedIn && m_ModInfo != null && !string.IsNullOrEmpty(m_ModInfo.UniqueID);

        private static ModsDownloadManager.ModDownloadInfo m_DownloadInfo;
        public static bool IsDownloadingAMod(string id) => m_DownloadInfo != null && m_DownloadInfo.ModInformation != null && id.Equals(m_DownloadInfo.ModInformation.UniqueID);

        public ModInfoUIVizualizator Init(ModInfo info)
        {
            m_ModdedObject = base.GetComponent<ModdedObject>();
            m_CanvasGroup = base.GetComponent<CanvasGroup>();
            m_ModInfo = info;

            string description = info.Description;/*
            if(description.Length > 70)
            {
                description = description.Remove(70) + "...";
            }*/

            m_ModdedObject.GetObject_Alt<Text>(1).text = "By " + info.Author;
            m_ModdedObject.GetObject_Alt<Text>(2).text = info.DisplayName;
            m_ModdedObject.GetObject_Alt<Text>(3).text = info.Description;
            m_ModdedObject.GetObject_Alt<Text>(3).text = description;
            m_ModdedObject.GetObject_Alt<Button>(8).onClick.AddListener(CopyModID);
            m_ModdedObject.GetObject_Alt<Button>(9).onClick.AddListener(OpenModOnWebsite);
            m_ModdedObject.GetObject_Alt<Button>(7).onClick.AddListener(ShowDetails);
            m_LikeButton = m_ModdedObject.GetObject_Alt<Button>(15);
            m_LikeButton.onClick.AddListener(LikeTheMod);
            m_LikeButton.interactable = false;
            m_LikesCount = m_ModdedObject.GetObject_Alt<Text>(13);
            m_MoreInfoButton = m_ModdedObject.GetObject_Alt<Button>(5);
            m_MoreInfoButton.onClick.AddListener(ToggleControlsBGVisibility);
            m_DownloadProgressBar = m_ModdedObject.GetObject_Alt<Slider>(12);
            m_DownloadCount = m_ModdedObject.GetObject_Alt<Text>(11);
            m_DownloadButton = m_ModdedObject.GetObject_Alt<Button>(4);
            m_DownloadButton.onClick.AddListener(downloadAMod);
            m_DownloadedText = m_ModdedObject.GetObject_Alt<Text>(10);
            m_Thumbnail = m_ModdedObject.GetObject_Alt<RawImage>(0);
            m_ControlsBG = m_ModdedObject.GetObject_Alt<Transform>(6);
            m_NotVerifiedIcon = m_ModdedObject.GetObject_Alt<Transform>(16);
            m_NotVerifiedIcon.gameObject.SetActive(false);
            m_HasInitialized = true;

            base.gameObject.SetActive(true);
            DoAnimation();
            _ = StartCoroutine(downloadImageAsync("https://modbot.org/api?operation=getModImage&size=256x256&id=" + m_ModInfo.UniqueID));
            _ = StartCoroutine(downloadSpecialModData());
            SetControlsBGVisible(false);
            refreshModIsInstalled();
            refreshSpecialData();
            refreshModIsBeingDownloaded();

            return this;
        }

        private void downloadAMod()
        {
            if(!m_HasInitialized || m_DownloadInfo != null)
            {
                return;
            }

            _ = new Generic2ButtonDialogue("Do you want to install this mod?\n" + m_ModInfo.DisplayName, "Yes", delegate
            {
                ModsDownloadManager.DownloadMod(m_ModInfo, onModDownloaded);
                m_DownloadInfo = ModsDownloadManager.GetDownloadingModInfo();
                refreshModIsBeingDownloaded();
            }, "Nevermind", null, Generic2ButtonDialogeUI.ModDeletionSizeDelta);
        }

        private void onModDownloaded()
        {
            if (!m_HasInitialized)
            {
                return;
            }

            onModDownloadedStatic();
            refreshModIsBeingDownloaded();
            refreshModIsInstalled();
        }

        private static void onModDownloadedStatic()
        {
            m_DownloadInfo = null;
            ModsPanelManager.Instance.ReloadModItems();
        }

        private void refreshModIsBeingDownloaded()
        {
            if (!m_HasInitialized || m_ModInfo == null)
            {
                return;
            }

            m_DownloadProgressBar.gameObject.SetActive(false);
            if (IsDownloadingAMod(m_ModInfo.UniqueID))
            {
                m_DownloadButton.gameObject.SetActive(false);
                m_DownloadedText.gameObject.SetActive(false);
                m_DownloadProgressBar.gameObject.SetActive(true);
                m_DownloadProgressBar.value = m_DownloadInfo.DownloadProgress;
            }
        }

        private void refreshModIsInstalled()
        {
            if (!m_HasInitialized)
            {
                return;
            }

            LoadedModInfo info = ModsManager.Instance.GetLoadedModWithID(m_ModInfo.UniqueID);
            if(info == null)
            {
                m_DownloadButton.gameObject.SetActive(true);
                m_DownloadedText.gameObject.SetActive(false);
                return;
            }
            m_InstalledModInfo = info.OwnerModInfo;
            m_DownloadButton.gameObject.SetActive(false);
            m_DownloadedText.gameObject.SetActive(true);
        }

        private void refreshSpecialData()
        {
            if(m_SpecialData == null)
            {
                m_DownloadCount.text = "?";
                m_LikesCount.text = "?";
                return;
            }
            m_LikesCount.text = m_SpecialData["Likes"].ToObject<string>();
            m_DownloadCount.text = m_SpecialData["Downloads"].ToObject<string>();
            m_NotVerifiedIcon.gameObject.SetActive(!m_SpecialData["Verified"].ToObject<bool>());
        }

        private IEnumerator downloadImageAsync(string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.isHttpError || webRequest.isNetworkError)
                    yield break;

                Texture2D texture = (webRequest.downloadHandler as DownloadHandlerTexture).texture;
                m_Thumbnail.color = Color.white;
                m_Thumbnail.texture = texture;
            }
        }

        private IEnumerator downloadSpecialModData()
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://modbot.org/api?operation=getSpecialModData&id=" + m_ModInfo.UniqueID))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                    yield break;

                m_SpecialData = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(webRequest.downloadHandler.text);
                refreshSpecialData();
                m_LikeButton.interactable = CanInteractWithSpecialData;

                if (m_PrevLikeCount != -1 && m_PrevLikeCount == m_SpecialData["Likes"].ToObject<int>())
                {
                    m_PrevLikeCount = -1;
                    _ = new Generic2ButtonDialogue("It seems like you have already liked the mod.", "I want to dislike the mod", UnLikeTheMod, "OK", null, Generic2ButtonDialogeUI.ModDeletionSizeDelta);
                }

                yield break;
            }
        }

        public void DoAnimation()
        {
            m_CanvasGroup.alpha = 0f;
            m_IsAnimating = true;
        }

        public void ToggleControlsBGVisibility()
        {
            SetControlsBGVisible(!m_ControlsBG.gameObject.activeSelf);
            m_MoreInfoButton.OnDeselect(null);
        }
        public void SetControlsBGVisible(bool value)
        {
            m_ControlsBG.gameObject.SetActive(value);
        }
        
        public void CopyModID()
        {
            if(m_ModInfo == null || string.IsNullOrEmpty(m_ModInfo.UniqueID))
            {
                return;
            }

            TextEditor textEditor = new TextEditor
            {
                text = m_ModInfo.UniqueID
            };
            textEditor.SelectAll();
            textEditor.Copy();
            SetControlsBGVisible(false);
        }

        public void OpenModOnWebsite()
        {
            if(m_ModInfo == null)
            {
                return;
            }
            Application.OpenURL("https://modbot.org/modPreview.html?modID=" + m_ModInfo.UniqueID);
            SetControlsBGVisible(false);
        }

        public void ShowDetails()
        {
            ModBotHUDRootNew.DownloadWindow.OpenInformationWindow(m_ModInfo, m_Thumbnail.texture);
            SetControlsBGVisible(false);
        }

        public void LikeTheMod()
        {
            if (!CanInteractWithSpecialData)
            {
                return;
            }
            m_PrevLikeCount = m_SpecialData["Likes"].ToObject<int>();
            m_LikeButton.interactable = false;
            API.Like(m_ModInfo.UniqueID, "true", onLikedTheMod);
        }

        public void UnLikeTheMod()
        {
            if (!CanInteractWithSpecialData)
            {
                return;
            }
            m_PrevLikeCount = m_SpecialData["Likes"].ToObject<int>();
            m_LikeButton.interactable = false;
            API.Like(m_ModInfo.UniqueID, "false", onLikedTheMod);
        }

        private void onLikedTheMod(JsonObject callback)
        {
            obj = callback;
            _ = StartCoroutine(downloadSpecialModData());
        }

        private void OnDisable()
        {
            m_HasInitialized = false;
            StopAllCoroutines();
        }

        private void Update()
        {
            if (!m_HasInitialized)
            {
                return;
            }

            if (m_IsAnimating)
            {
                m_CanvasGroup.alpha += Time.unscaledDeltaTime;
                if (m_CanvasGroup.alpha == 1f)
                {
                    m_IsAnimating = false;
                }
            }

            m_DownloadButton.interactable = !ModsDownloadManager.IsDownloadingAMod() || m_DownloadInfo == null;
            if (!m_DownloadButton.interactable && Time.frameCount % 3 == 0)
            {
                refreshModIsBeingDownloaded();
            }
        }
    }
}
