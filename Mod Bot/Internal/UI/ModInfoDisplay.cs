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
    internal class ModInfoDisplay : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        private RawImage _thumbnail;
        private Transform _notVerifiedIcon;

        private Button _downloadButton;
        private Text _downloadCount;
        private Text _downloadedText;
        private Slider _downloadProgressBar;

        private Button _likeButton;
        private Text _likesCount;

        private Button _moreInfoButton;
        private Transform _controlsBG;

        private bool _initialized;
        private bool _isFading;
        private int _prevLikeCount = -1;

        private JsonObject _modInfoJsonObject;
        private ModInfo _remoteModInfo;
        private ModInfo _localModInfo;
        private Dictionary<string, JToken> _specialData;

        public bool IsModInstalled => _localModInfo != null;
        public string ModName => _remoteModInfo.DisplayName;
        public bool CanInteractWithSpecialData => ModBotSignInUI.HasSignedIn && _remoteModInfo != null && !string.IsNullOrEmpty(_remoteModInfo.UniqueID);

        private static ModsDownloadManager.ModDownloadInfo m_DownloadInfo;
        public static bool IsDownloadingAMod(string id) => m_DownloadInfo != null && m_DownloadInfo.ModInformation != null && id.Equals(m_DownloadInfo.ModInformation.UniqueID);

        public ModInfoDisplay Init(ModInfo info)
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            _canvasGroup = base.GetComponent<CanvasGroup>();
            _remoteModInfo = info;

            string description = info.Description;/*
            if(description.Length > 70)
            {
                description = description.Remove(70) + "...";
            }*/

            moddedObject.GetObject<Text>(1).text = "By " + info.Author;
            moddedObject.GetObject<Text>(2).text = info.DisplayName;
            moddedObject.GetObject<Text>(3).text = info.Description;
            moddedObject.GetObject<Text>(3).text = description;
            moddedObject.GetObject<Button>(8).onClick.AddListener(CopyModID);
            moddedObject.GetObject<Button>(9).onClick.AddListener(OpenModOnWebsite);
            moddedObject.GetObject<Button>(7).onClick.AddListener(ShowDetails);
            _likeButton = moddedObject.GetObject<Button>(15);
            _likeButton.onClick.AddListener(LikeTheMod);
            _likeButton.interactable = false;
            _likesCount = moddedObject.GetObject<Text>(13);
            _moreInfoButton = moddedObject.GetObject<Button>(5);
            _moreInfoButton.onClick.AddListener(ToggleControlsBGVisibility);
            _downloadProgressBar = moddedObject.GetObject<Slider>(12);
            _downloadCount = moddedObject.GetObject<Text>(11);
            _downloadButton = moddedObject.GetObject<Button>(4);
            _downloadButton.onClick.AddListener(downloadAMod);
            _downloadButton.gameObject.SetActive(false);
            _downloadedText = moddedObject.GetObject<Text>(10);
            _thumbnail = moddedObject.GetObject<RawImage>(0);
            _controlsBG = moddedObject.GetObject<Transform>(6);
            _notVerifiedIcon = moddedObject.GetObject<Transform>(16);
            _notVerifiedIcon.gameObject.SetActive(false);
            _initialized = true;

            base.gameObject.SetActive(true);
            DoAnimation();
            _ = StartCoroutine(downloadImageAsync("https://modbot.org/api?operation=getModImage&size=256x256&id=" + _remoteModInfo.UniqueID));
            _ = StartCoroutine(downloadSpecialModData());
            SetControlsBGVisible(false);
            refreshModIsInstalled();
            refreshSpecialData();
            refreshModIsBeingDownloaded();

            return this;
        }

        private void downloadAMod()
        {
            if (!_initialized || m_DownloadInfo != null)
            {
                return;
            }

            _ = new Generic2ButtonDialogue("Do you want to install this mod?\n" + _remoteModInfo.DisplayName, "Yes", delegate
            {
                ModsDownloadManager.DownloadMod(_remoteModInfo, onModDownloaded);
                m_DownloadInfo = ModsDownloadManager.GetDownloadingModInfo();
                refreshModIsBeingDownloaded();
            }, "Nevermind", null, Generic2ButtonDialogeUI.ModDeletionSizeDelta);
        }

        private void onModDownloaded()
        {
            if (!_initialized)
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
            ModBotUIRoot.Instance.ModList.ReloadList();
        }

        private void refreshModIsBeingDownloaded()
        {
            if (!_initialized || _remoteModInfo == null)
            {
                return;
            }

            _downloadProgressBar.gameObject.SetActive(false);
            if (IsDownloadingAMod(_remoteModInfo.UniqueID))
            {
                _downloadButton.gameObject.SetActive(false);
                _downloadedText.gameObject.SetActive(false);
                _downloadProgressBar.gameObject.SetActive(true);
                _downloadProgressBar.value = m_DownloadInfo.DownloadProgress;
            }
        }

        private void refreshModIsInstalled()
        {
            if (!_initialized)
            {
                return;
            }

            LoadedModInfo info = ModsManager.Instance.GetLoadedModWithID(_remoteModInfo.UniqueID);
            if (info == null)
            {
                //_downloadButton.gameObject.SetActive(true);
                _downloadedText.gameObject.SetActive(false);
                return;
            }
            _localModInfo = info.OwnerModInfo;
            _downloadButton.gameObject.SetActive(false);
            _downloadedText.gameObject.SetActive(true);
        }

        private void refreshSpecialData()
        {
            if (_specialData == null)
            {
                _downloadCount.text = "?";
                _likesCount.text = "?";
                return;
            }
            _likesCount.text = _specialData["Likes"].ToObject<string>();
            _downloadCount.text = _specialData["Downloads"].ToObject<string>();
            var isVerified = _specialData["Verified"].ToObject<bool>();
            if (!isVerified)
            {
                _notVerifiedIcon.gameObject.SetActive(true);
                _downloadButton.gameObject.SetActive(false);
                return;
            }
            if (!IsModInstalled)
                _downloadButton.gameObject.SetActive(true);
        }

        private IEnumerator downloadImageAsync(string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.isHttpError || webRequest.isNetworkError)
                    yield break;

                Texture2D texture = (webRequest.downloadHandler as DownloadHandlerTexture).texture;
                _thumbnail.color = Color.white;
                _thumbnail.texture = texture;
            }
        }

        private IEnumerator downloadSpecialModData()
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://modbot.org/api?operation=getSpecialModData&id=" + _remoteModInfo.UniqueID))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                    yield break;

                _specialData = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(webRequest.downloadHandler.text);
                refreshSpecialData();
                _likeButton.interactable = CanInteractWithSpecialData;

                if (_prevLikeCount != -1 && _prevLikeCount == _specialData["Likes"].ToObject<int>())
                {
                    _prevLikeCount = -1;
                    _ = new Generic2ButtonDialogue("It seems like you have already liked the mod.", "I want to dislike the mod", UnLikeTheMod, "OK", null, Generic2ButtonDialogeUI.ModDeletionSizeDelta);
                }

                yield break;
            }
        }

        public void DoAnimation()
        {
            _canvasGroup.alpha = 0f;
            _isFading = true;
        }

        public void ToggleControlsBGVisibility()
        {
            SetControlsBGVisible(!_controlsBG.gameObject.activeSelf);
            _moreInfoButton.OnDeselect(null);
        }
        public void SetControlsBGVisible(bool value)
        {
            _controlsBG.gameObject.SetActive(value);
        }

        public void CopyModID()
        {
            if (_remoteModInfo == null || string.IsNullOrEmpty(_remoteModInfo.UniqueID))
            {
                return;
            }

            TextEditor textEditor = new TextEditor
            {
                text = _remoteModInfo.UniqueID
            };
            textEditor.SelectAll();
            textEditor.Copy();
            SetControlsBGVisible(false);
        }

        public void OpenModOnWebsite()
        {
            if (_remoteModInfo == null)
            {
                return;
            }
            Application.OpenURL("https://modbot.org/modPreview.html?modID=" + _remoteModInfo.UniqueID);
            SetControlsBGVisible(false);
        }

        public void ShowDetails()
        {
            ModBotUIRoot.Instance.DownloadWindow.OpenInformationWindow(_remoteModInfo, _specialData, _thumbnail.texture);
            SetControlsBGVisible(false);
        }

        public void LikeTheMod()
        {
            if (!CanInteractWithSpecialData)
            {
                return;
            }
            _prevLikeCount = _specialData["Likes"].ToObject<int>();
            _likeButton.interactable = false;
            API.Like(_remoteModInfo.UniqueID, "true", onLikedTheMod);
        }

        public void UnLikeTheMod()
        {
            if (!CanInteractWithSpecialData)
            {
                return;
            }
            _prevLikeCount = _specialData["Likes"].ToObject<int>();
            _likeButton.interactable = false;
            API.Like(_remoteModInfo.UniqueID, "false", onLikedTheMod);
        }

        private void onLikedTheMod(JsonObject callback)
        {
            _modInfoJsonObject = callback;
            _ = StartCoroutine(downloadSpecialModData());
        }

        private void OnDisable()
        {
            _initialized = false;
            StopAllCoroutines();
        }

        private void Update()
        {
            if (!_initialized)
            {
                return;
            }

            if (_isFading)
            {
                _canvasGroup.alpha += Time.unscaledDeltaTime;
                if (_canvasGroup.alpha == 1f)
                {
                    _isFading = false;
                }
            }

            _downloadButton.interactable = !ModsDownloadManager.IsDownloadingAMod() || m_DownloadInfo == null;
            if (!_downloadButton.interactable && Time.frameCount % 3 == 0)
            {
                refreshModIsBeingDownloaded();
            }
        }
    }
}
