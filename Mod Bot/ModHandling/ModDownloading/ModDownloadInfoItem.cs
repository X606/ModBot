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
        Button _loadButton;

        string _modDownloadUrl;

        ModInfo _underlyingModInfo;

        /// <summary>
        /// Initilizes the <see cref="ModDownloadInfoItem"/>
        /// </summary>
        /// <param name="holder"></param>
        public void Init(ModInfo holder)
        {
            ModdedObject moddedObject = GetComponent<ModdedObject>();
            _modImage = moddedObject.GetObject<Image>(0);
            _nameDisplay = moddedObject.GetObject<Text>(1);
            _desciptionDisplay = moddedObject.GetObject<Text>(2);
            _creatorText = moddedObject.GetObject<Text>(3);
            _downloadButton = moddedObject.GetObject<Button>(4);
            _loadButton = moddedObject.GetObject<Button>(5);

            _modDownloadUrl = "modbot.org/api?operation=downloadMod&id=" + holder.UniqueID;

            _downloadButton.onClick.AddListener(onDownloadButtonClicked);
            _loadButton.onClick.AddListener(onBrowseButtonClicked);

            _nameDisplay.text = holder.DisplayName.Replace("&lt;", "<").Replace("&#39;", "'"); // makes sure we show "<" like "<" and not "&lt;"
            _desciptionDisplay.text = holder.Description.Replace("&lt;", "<").Replace("&#39;", "'"); // makes sure we show "<" like "<" and not "&lt;"
            _creatorText.text = "by: " + holder.Author.Replace("&lt;", "<").Replace("&#39;", "'"); // makes sure we show "<" like "<" and not "&lt;";

            StartCoroutine(downloadImageAsync("https://modbot.org/api?operation=getModImage&size=108x90&id=" + holder.UniqueID));

            _underlyingModInfo = holder;
        }

        IEnumerator downloadImageAsync(string url)
        {
            UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);

            yield return webRequest.SendWebRequest();
            if(webRequest.isHttpError || webRequest.isNetworkError)
                yield break;

            Texture2D texture = (webRequest.downloadHandler as DownloadHandlerTexture).texture;

            _modImage.sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), new Vector2(texture.width/2, texture.height/2));
        }

        void onDownloadButtonClicked()
        {
            new Generic2ButtonDialogue(ModBotLocalizationManager.FormatLocalizedStringFromID("mod_download_confirm_message", _nameDisplay.text),
            LocalizationManager.Instance.GetTranslatedString("mod_download_confirm_no"), null,
            LocalizationManager.Instance.GetTranslatedString("mod_download_confirm_yes"), delegate
            {
                StartCoroutine(downloadModFileAndLoadAsync(_modDownloadUrl));
            });
        }

        void onBrowseButtonClicked()
        {
            Process.Start("https://modbot.org/modPreview.html?modID=" + _underlyingModInfo.UniqueID);
            /*
            new Generic2ButtonDialogue(ModBotLocalizationManager.FormatLocalizedStringFromID("mod_load_confirm_message", _nameDisplay.text),
            LocalizationManager.Instance.GetTranslatedString("mod_load_confirm_no"), null,
            LocalizationManager.Instance.GetTranslatedString("mod_load_confirm_yes"), delegate
            {
                StartCoroutine(downloadModBytesAndLoadAsync(_modDownloadUrl));
            });
            */
        }

        static IEnumerator downloadModFileAndLoadAsync(string url)
        {
			// yield return 0;
			
            // Reverted
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();

            if(webRequest.isHttpError || webRequest.isNetworkError)
                yield break;

            string[] subUrls = url.Split('/');
            string fileName = subUrls[subUrls.Length-1];

            string path = AssetLoader.GetModsFolderDirectory() + fileName;
            File.WriteAllBytes(path, webRequest.downloadHandler.data);

            ModsManager.Instance.ReloadMods();

            ModsPanelManager.Instance.ReloadModItems();
		}

        IEnumerator downloadModBytesAndLoadAsync(string url)
        {
			yield return 0;

            // Old pre 2.0 code code
            /*
			UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();

            if(webRequest.isHttpError || webRequest.isNetworkError)
                yield break;

            ModsManager.Instance.LoadMod(webRequest.downloadHandler.data, false, out string error);
            ModsPanelManager.Instance.ReloadModItems();
            */
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
