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
        Button _loadButton;

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
                StartCoroutine(downloadModFileAndLoadAsync(() => ModBotUIRoot.Instance.ModDownloadPage.XButton.interactable = true));
            });
        }

        void onBrowseButtonClicked()
        {
            Process.Start("https://modbot.org/modPreview.html?modID=" + _underlyingModInfo.UniqueID);
        }

        IEnumerator downloadModFileAndLoadAsync(Action onComplete)
        {
            // If mod is already loaded, just cancel the download instead of throwing an exception
            if (ModsManager.Instance.GetLoadedModWithID(_underlyingModInfo.UniqueID) != null)
            {
                onComplete?.Invoke();
                yield break;
            }

            string folderName = _underlyingModInfo.DisplayName;
            foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
            {
                folderName = folderName.Replace(invalidCharacter, '_');
            }

            string targetDirectory = ModsManager.Instance.ModFolderPath + folderName;
            if (Directory.Exists(targetDirectory))
            {
                onComplete?.Invoke();
                yield break;
            }

            byte[] bytes;
            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://modbot.org/api?operation=downloadMod&id=" + _underlyingModInfo.UniqueID))
            {
                UnityWebRequestAsyncOperation request = webRequest.SendWebRequest();

                ModBotUIRoot.Instance.ModDownloadPage.ProgressBar.Show();

                while (!request.isDone)
                {
                    ModBotUIRoot.Instance.ModDownloadPage.ProgressBar.Progress = request.progress;
                    yield return 0;
                }

                ModBotUIRoot.Instance.ModDownloadPage.ProgressBar.Hide();

                bytes = webRequest.downloadHandler.data;
            }

            string tempFilePath = Path.GetTempFileName();

            yield return new WaitForTask(Task.Run(async () =>
            {
                using (FileStream tempFile = new FileStream(tempFilePath, FileMode.Create))
                {
                    tempFile.Seek(0, SeekOrigin.Begin);
                    await tempFile.WriteAsync(bytes, 0, bytes.Length);
                }
            }));

            Directory.CreateDirectory(targetDirectory);
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(tempFilePath, targetDirectory, null);

            ModsManager.Instance.ReloadMods();

            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }

            onComplete?.Invoke();
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
