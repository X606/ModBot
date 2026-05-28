using ModBotWebsiteAPI;
using ModLibrary;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace InternalModBot
{
    internal class ModInfoCard : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private CanvasRenderer _bgRenderer;

        private RawImage _thumbnail;
        private Transform _notVerifiedIcon;

        private Button _downloadButton;
        private Text _downloadCount;
        private Text _downloadedText;
        private Slider _downloadProgressBar;

        private Button _likeButton;
        private Text _likesCount;

        private Button _moreInfoButton;

        private bool _initialized;
        private bool _isFading;
        private int _prevLikeCount = -1;

        private ModInfo _remoteModInfo;
        private ModInfo _localModInfo;
        private ModSpecialData _specialData;

        private bool _isDestroyed;

        public bool IsModInstalled => _localModInfo != null;
        public string ModName => _remoteModInfo.DisplayName;
        public bool CanInteractWithSpecialData => API.HasSession && _remoteModInfo != null && !string.IsNullOrEmpty(_remoteModInfo.UniqueID);

        private static ModsDownloadManager.ModDownloadInfo _downloadInfo;
        public static bool IsDownloadingAMod(string id) => _downloadInfo != null && _downloadInfo.Info != null && id.Equals(_downloadInfo.Info.UniqueID);

        private void OnDestroy()
        {
            _isDestroyed = true;
        }

        public void Init(ModInfo info, ModSpecialData specialData)
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            _canvasGroup = base.GetComponent<CanvasGroup>();
            _remoteModInfo = info;
            _specialData = specialData;

            moddedObject.GetObject<Text>(1).text = "By " + info.Author;
            moddedObject.GetObject<Text>(2).text = info.DisplayName;
            moddedObject.GetObject<Text>(3).text = info.Description;
            _likeButton = moddedObject.GetObject<Button>(10);
            _likeButton.onClick.AddListener(LikeTheMod);
            _likeButton.interactable = true;
            _likesCount = moddedObject.GetObject<Text>(9);
            _moreInfoButton = moddedObject.GetObject<Button>(5);
            _moreInfoButton.onClick.AddListener(OnModInfoButtonClicked);
            _downloadProgressBar = moddedObject.GetObject<Slider>(8);
            _downloadCount = moddedObject.GetObject<Text>(7);
            _downloadButton = moddedObject.GetObject<Button>(4);
            _downloadButton.onClick.AddListener(downloadMod);
            _downloadButton.gameObject.SetActive(false);
            _downloadedText = moddedObject.GetObject<Text>(6);
            _thumbnail = moddedObject.GetObject<RawImage>(0);
            _notVerifiedIcon = moddedObject.GetObject<Transform>(11);
            _notVerifiedIcon.gameObject.SetActive(false);
            _bgRenderer = moddedObject.GetObject<CanvasRenderer>(12);
            _initialized = true;

            base.gameObject.SetActive(true);
            StartCoroutine(downloadImageAsync("https://modbot.org/api?operation=getModImage&size=256x256&id=" + _remoteModInfo.UniqueID));
            refreshModIsInstalled();
            refreshSpecialData();
            refreshModIsBeingDownloaded();
        }

        public bool IsOffScreen() => _bgRenderer.cull;

        private void downloadMod()
        {
            if (!_initialized || _downloadInfo != null)
            {
                return;
            }

            ColorUtility.TryParseHtmlString(LocalModInfoDisplay.VERSION_COLOR, out Color color);
            _ = new Generic2ButtonDialogue($"Install {_remoteModInfo.DisplayName.AddColor(color)}?", "Yes", delegate
            {
                ModsDownloadManager.DownloadMod(new ModsDownloadManager.ModGeneralInfo()
                {
                    DisplayName = _remoteModInfo.DisplayName,
                    UniqueID = _remoteModInfo.UniqueID,
                    Version = _remoteModInfo.Version,
                }, false, onModDownloaded);
                _downloadInfo = ModsDownloadManager.GetDownloadingModInfo();
                refreshModIsBeingDownloaded();
            }, "Nevermind", null, Generic2ButtonDialogueUI.ModDeletionSizeDelta);
        }

        private void onModDownloaded(ModsDownloadManager.DownloadModResult result)
        {
            if (!_initialized)
            {
                return;
            }

            if (result.HasFailed())
            {
                ColorUtility.TryParseHtmlString(LocalModInfoDisplay.VERSION_COLOR, out Color color);
                ModsDownloadManager.ModGeneralInfo modInfo = result.Info;
                _ = new Generic2ButtonDialogue($"Failed to download {modInfo.DisplayName.AddColor(color)}.\n{result.Error}",
                    "Ok", null,
                    "Ok", null);

                return;
            }

            onModDownloadedStatic();
            refreshModIsBeingDownloaded();
            refreshModIsInstalled();
        }

        private static void onModDownloadedStatic()
        {
            _downloadInfo = null;
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
                _downloadProgressBar.value = _downloadInfo.DownloadProgress;
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

            _likesCount.text = _specialData.Likes.ToString();

            int downloadCount = _specialData.Downloads;
            if (downloadCount < 1000)
            {
                _downloadCount.text = downloadCount.ToString();
            }
            else
            {
                _downloadCount.text = $"{Mathf.FloorToInt(downloadCount / 1000f)}K";
            }

            bool isVerified = _specialData.Verified;
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

                if (webRequest.result != UnityWebRequest.Result.Success)
                    yield break;

                Texture2D texture = (webRequest.downloadHandler as DownloadHandlerTexture).texture;
                if (_isDestroyed)
                {
                    Destroy(texture);
                    yield break;
                }

                _thumbnail.color = Color.white;
                _thumbnail.texture = texture;
            }
        }

        private void updateSpecialData()
        {
            ModsDownloadManager.UpdateSpecialModData(_remoteModInfo.UniqueID, delegate (ModSpecialData modSpecialData)
            {
                if (modSpecialData == null || _isDestroyed) return;

                _likeButton.interactable = true;

                _specialData = modSpecialData;
                refreshSpecialData();

                if (_prevLikeCount != -1 && _prevLikeCount == _specialData.Likes)
                {
                    _prevLikeCount = -1;
                    _ = new Generic2ButtonDialogue("You have already liked the mod.", "Take my like!", UnLikeTheMod, "OK", null, Generic2ButtonDialogueUI.ModDeletionSizeDelta);
                }
            });
        }

        public void MakeInvisible()
        {
            _canvasGroup.alpha = 0f;
        }

        public void FadeOut()
        {
            _isFading = true;
        }

        public void OnModInfoButtonClicked()
        {
            ModBotUIRoot.Instance.DownloadWindow.OpenInformationWindow(_remoteModInfo, _specialData, _thumbnail.texture);
        }

        public void LikeTheMod()
        {
            if (!CanInteractWithSpecialData)
            {
                if (ModBotUIRoot.Instance.ModBotSignInUI.WindowObject.activeInHierarchy) return;

                _ = new Generic2ButtonDialogue("You have to be signed in to like mods", "Ok", null, "Sign in", delegate
                {
                    ModBotUIRoot.Instance.ModBotSignInUI.OpenSignInForm();
                }, Generic2ButtonDialogueUI.ModDeletionSizeDelta);
                return;
            }

            _prevLikeCount = _specialData.Likes;
            _likeButton.interactable = false;
            API.Like(_remoteModInfo.UniqueID, "true", onLikedTheMod);
        }

        public void UnLikeTheMod()
        {
            if (!CanInteractWithSpecialData) return;

            _prevLikeCount = _specialData.Likes;
            _likeButton.interactable = false;
            API.Like(_remoteModInfo.UniqueID, "false", onLikedTheMod);
        }

        private void onLikedTheMod(JsonObject callback)
        {
            updateSpecialData();
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

            _downloadButton.interactable = !ModsDownloadManager.IsDownloadingAMod() || _downloadInfo == null;
            if (!_downloadButton.interactable)
            {
                refreshModIsBeingDownloaded();
            }
        }
    }
}
