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

namespace InternalModBot
{
    /// <summary>
    /// Controlls a item in the get more mods menu
    /// </summary>
    public class ModDownloadInfoItem : MonoBehaviour
    {
        Image ModImage;
        Text NameDisplay;
        Text DesciptionDisplay;
        Text CreatorText;
        Button DownloadButton;
        Button LoadButton;

        string ModDownloadUrl;

        /// <summary>
        /// Initilizes the <see cref="ModDownloadInfoItem"/>
        /// </summary>
        /// <param name="holder"></param>
        public void Init(ModsHolder.ModHolder holder)
        {
            ModdedObject moddedObject = GetComponent<ModdedObject>();
            ModImage = moddedObject.GetObject<Image>(0);
            NameDisplay = moddedObject.GetObject<Text>(1);
            DesciptionDisplay = moddedObject.GetObject<Text>(2);
            CreatorText = moddedObject.GetObject<Text>(3);
            DownloadButton = moddedObject.GetObject<Button>(4);
            LoadButton = moddedObject.GetObject<Button>(5);

            ModDownloadUrl = holder.DownloadLink;

            DownloadButton.onClick.AddListener(OnDownloadButtonClicked);
            LoadButton.onClick.AddListener(OnLoadButtonClicked);

            NameDisplay.text = holder.ModName.Replace("&lt;", "<").Replace("&#39;", "'"); // makes sure we show "<" like "<" and not "&lt;"
            DesciptionDisplay.text = holder.Description.Replace("&lt;", "<").Replace("&#39;", "'"); // makes sure we show "<" like "<" and not "&lt;"
            CreatorText.text = "by: " + holder.CreatorID.Replace("&lt;", "<").Replace("&#39;", "'"); // makes sure we show "<" like "<" and not "&lt;";

            StartCoroutine(DownloadImageAsync(holder.ImageLink));
        }

        IEnumerator DownloadImageAsync(string url)
        {
            UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);

            yield return webRequest.SendWebRequest();
            if(webRequest.isHttpError || webRequest.isNetworkError)
                yield break;

            Texture2D texture = (webRequest.downloadHandler as DownloadHandlerTexture).texture;

            ModImage.sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), new Vector2(texture.width/2, texture.height/2));
        }

        void OnDownloadButtonClicked()
        {
            new Generic2ButtonDialogue("You are about to download " + NameDisplay.text + ". This will place a .dll file in your mods folder and reload all mods (this also means that any temporarily loaded mods will go away). Are you sure you want to do this?",
                "No", delegate
                {

                }
            , "Yes", delegate
            {
                StartCoroutine(DownloadModFileAndLoadAsyc(ModDownloadUrl));

            });
        }
        void OnLoadButtonClicked()
        {
            new Generic2ButtonDialogue("You are about to load in " + NameDisplay.text + " temporarily (will not place it in the mods folder. The mod will go away the next time you exit the game). Are you sure you want to do this?",
                "No", delegate
            {

            }
            , "Yes", delegate
            {
                StartCoroutine(DownloadModBytesAndLoadAsyc(ModDownloadUrl));

            });

        }
        IEnumerator DownloadModFileAndLoadAsyc(string url)
        {
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
        IEnumerator DownloadModBytesAndLoadAsyc(string url)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();

            if(webRequest.isHttpError || webRequest.isNetworkError)
                yield break;

            ModsManager.Instance.LoadMod(webRequest.downloadHandler.data, false);
            ModsPanelManager.Instance.ReloadModItems();
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
        [JsonProperty(PropertyName = "mods")]
        public ModHolder[] Mods;

        /// <summary>
        /// Used when deserilizing data from the site and represents 1 mod
        /// </summary>
        public struct ModHolder
        {
            /// <summary>
            /// Whether or not the mod is "checked", this means that its working and is checked so that it doesnt have any viruses in it.
            /// </summary>
            [JsonProperty(PropertyName = "checked")]
            public bool Checked;
            /// <summary>
            /// The name of the creator.
            /// </summary>
            [JsonProperty(PropertyName = "creatorID")]
            public string CreatorID;
            /// <summary>
            /// The description of the mod.
            /// </summary>
            [JsonProperty(PropertyName = "description")]
            public string Description;
            /// <summary>
            /// The download link for the mod.
            /// </summary>
            [JsonProperty(PropertyName = "downloadLink")]
            public string DownloadLink;
            /// <summary>
            /// The link to the image that should be displayed.
            /// </summary>
            [JsonProperty(PropertyName = "imageLink")]
            public string ImageLink;
            /// <summary>
            /// The name of the mod.
            /// </summary>
            [JsonProperty(PropertyName = "name")]
            public string ModName;
        }
    }
    
}
