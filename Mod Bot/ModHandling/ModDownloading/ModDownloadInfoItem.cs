using ModLibrary;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Controlls a item in the get more mods menu
    /// </summary>
    public class ModDownloadInfoItem : MonoBehaviour
    {
        private Image _modImage;
        private Text _nameDisplay;
        private Text _desciptionDisplay;
        private Text _creatorText;
        private Button _downloadButton;
        private Button _siteButton;
        private Slider _loadProgressSlider;
        private RectTransform _downloadUI;
        private Button _cancelDownloadButton;
        private ModInfo _underlyingModInfo;
        private float _timeToNextRefresh;

        /// <summary>
        /// Initilizes the <see cref="ModDownloadInfoItem"/>
        /// </summary>
        /// <param name="holder"></param>
        public void Init(ModInfo holder)
        {
            _underlyingModInfo = holder;

            ModdedObject moddedObject = GetComponent<ModdedObject>();
            _modImage = moddedObject.GetObject<Image>(0);
            _nameDisplay = moddedObject.GetObject<Text>(1);
            _desciptionDisplay = moddedObject.GetObject<Text>(2);
            _creatorText = moddedObject.GetObject<Text>(3);
            _downloadButton = moddedObject.GetObject<Button>(4);
            _siteButton = moddedObject.GetObject_Alt<Button>(8);
            _siteButton.onClick.AddListener(onBrowseButtonClicked);

            _loadProgressSlider = moddedObject.GetObject_Alt<Slider>(6);
            _downloadUI = moddedObject.GetObject_Alt<RectTransform>(5);
            _cancelDownloadButton = moddedObject.GetObject_Alt<Button>(7);

            _downloadButton.onClick.AddListener(onDownloadButtonClicked);

            _nameDisplay.text = holder.DisplayName.Replace("&lt;", "<").Replace("&#39;", "'"); // makes sure we show "<" like "<" and not "&lt;"
            _desciptionDisplay.text = holder.Description.Replace("&lt;", "<").Replace("&#39;", "'"); // makes sure we show "<" like "<" and not "&lt;"
            _creatorText.text = "by: " + holder.Author.Replace("&lt;", "<").Replace("&#39;", "'"); // makes sure we show "<" like "<" and not "&lt;";

            StartCoroutine(downloadImageAsync("https://modbot.org/api?operation=getModImage&size=108x90&id=" + holder.UniqueID));

            refreshDownload();
        }

        private IEnumerator downloadImageAsync(string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.isHttpError || webRequest.isNetworkError)
                    yield break;

                Texture2D texture = (webRequest.downloadHandler as DownloadHandlerTexture).texture;

                _modImage.sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), new Vector2(texture.width / 2f, texture.height / 2f));
            }
        }

        private void onDownloadButtonClicked()
        {
            new Generic2ButtonDialogue(ModBotLocalizationManager.FormatLocalizedStringFromID("mod_download_confirm_message", _nameDisplay.text),
            LocalizationManager.Instance.GetTranslatedString("mod_download_confirm_no"), null,
            LocalizationManager.Instance.GetTranslatedString("mod_download_confirm_yes"), delegate
            {
                ModBotUIRoot.Instance.ModDownloadPage.XButton.interactable = false;
                ModsDownloadManager.DownloadMod(_underlyingModInfo, delegate
                {
                    ModBotUIRoot.Instance.ModDownloadPage.XButton.interactable = true;
                    ModsPanelManager.Instance.ReloadModItems();
                });
                refreshDownload();
            });
        }

        private void onCancelDownloadButtonClicked()
        {
            ModsDownloadManager.EndDownload(ModsDownloadManager.ModDownloadCancelReason.Manual);
        }

        private void onBrowseButtonClicked()
        {
            Process.Start("https://modbot.org/modPreview.html?modID=" + _underlyingModInfo.UniqueID);
        }

        private void refreshDownload()
        {
            if (_underlyingModInfo == null)
            {
                return;
            }

            ModsDownloadManager.ModDownloadInfo newInfo = ModsDownloadManager.GetDownloadingModInfo();

            _downloadUI.gameObject.SetActive(newInfo != null && newInfo.ModInformation == _underlyingModInfo && !newInfo.IsDone);

            if (_downloadUI.gameObject.activeSelf)
            {
                float percent = newInfo.DownloadProgress;
                _loadProgressSlider.value = percent;
            }
        }

        private void FixedUpdate()
        {
            float time = Time.unscaledTime;
            if (time >= _timeToNextRefresh)
            {
                _timeToNextRefresh = time + 0.1f;
                refreshDownload();
            }
        }
    }

    /// <summary>
    /// Used when deserilizing data from the site
    /// </summary>
    public struct ModsHolder
    {
        /// <summary>
        /// A list of all the mods downloaded.
        /// </summary>
        [JsonProperty(PropertyName = "ModInfos")]
        public ModInfo[] Mods;
    }
}
