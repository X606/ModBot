using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip;

namespace InternalModBot
{
    /// <summary>
    /// Controlls a item in the get more mods menu
    /// </summary>
    public class ModDownloadInfoItem : MonoBehaviour
    {
        Image _modImage;
        Text _nameDisplay;
        Text _desciptionDisplay;
        Text _creatorText;
        Button _downloadButton;
        Button _siteButton;

        Slider _loadProgressSlider;
        RectTransform _downloadUI;
        Button _cancelDownloadButton;

        ModInfo _underlyingModInfo;

        float _timeToNextRefresh;

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

        IEnumerator downloadImageAsync(string url)
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

        void onDownloadButtonClicked()
        {
            new Generic2ButtonDialogue(ModBotLocalizationManager.FormatLocalizedStringFromID("mod_download_confirm_message", _nameDisplay.text),
            LocalizationManager.Instance.GetTranslatedString("mod_download_confirm_no"), null,
            LocalizationManager.Instance.GetTranslatedString("mod_download_confirm_yes"), delegate
            {
                ModBotUIRoot.Instance.ModDownloadPage.XButton.interactable = false;
                ModDownloadManager.DownloadMod(_underlyingModInfo, delegate
                {
                    ModBotUIRoot.Instance.ModDownloadPage.XButton.interactable = true;
                    ModsPanelManager.Instance.ReloadModItems();
                });
                refreshDownload();
            });
        }

        void onCancelDownloadButtonClicked()
        {
            ModDownloadManager.EndDownload(ModDownloadManager.ModDownloadCancelReason.Manual);
        }

        void onBrowseButtonClicked()
        {
            Process.Start("https://modbot.org/modPreview.html?modID=" + _underlyingModInfo.UniqueID);
        }

        void refreshDownload()
        {
            if(_underlyingModInfo == null)
            {
                return;
            }

            ModDownloadManager.ModDownloadInfo newInfo = ModDownloadManager.GetDownloadingModInfo();

            _downloadUI.gameObject.SetActive(!newInfo.IsNull && newInfo.ModInformation == _underlyingModInfo && !newInfo.IsDone);

            if(_downloadUI.gameObject.activeSelf)
            {
                float percent = newInfo.DownloadProgress;
                _loadProgressSlider.value = percent;
            }
        }

        void FixedUpdate()
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
