using ModLibrary;
using ModLibrary.ModTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to download icons and put them in the upgrade icons when done
    /// </summary>
    public class UpgradeIconDownloader : Singleton<UpgradeIconDownloader>
    {
        private const string UpgradeIconsFolderName = "ModdedUpgradeIcons/";
        private string UpgradeIconsFolderPath
        {
            get
            {
                return Path.Combine(Application.dataPath, UpgradeIconsFolderName);
            }
        }

        private const char UpgradeAndLevelFileNameSeparator = '_';

        private void Start()
        {
            // Create icons folder if it does not exist
            if (!Directory.Exists(UpgradeIconsFolderPath))
            {
                Directory.CreateDirectory(UpgradeIconsFolderPath);
            }
        }

        /// <summary>
        /// Sets the icon of the upgrade to the image gotten from the url
        /// </summary>
        /// <param name="upgrade"></param>
        /// <param name="url"></param>
        public void SetIconOnUpgrade(UpgradeDescription upgrade, string url)
        {
            string fileName = GetFileNameForUpgrade(upgrade);

            if (File.Exists(UpgradeIconsFolderPath + fileName))
            {
                byte[] imageData = File.ReadAllBytes(UpgradeIconsFolderPath + fileName);

                Texture2D texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
                texture.LoadImage(imageData);

                upgrade.Icon = GetSpriteFromTexture(texture);
            }
            else
            {
                StartCoroutine(DownloadImageAndSetIconOnUpgrade(upgrade, url));
            }
        }

        private IEnumerator DownloadImageAndSetIconOnUpgrade(UpgradeDescription upgrade, string url)
        {
            UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                debug.Log(webRequest.error, Color.red);
                upgrade.Icon = null;
                yield break;
            }

            Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;

            upgrade.Icon = GetSpriteFromTexture(texture);

            string fileName = GetFileNameForUpgrade(upgrade);
            byte[] fileData = texture.EncodeToPNG();

            FileStream fileStream = File.Create(UpgradeIconsFolderPath + fileName);
            fileStream.Write(fileData, 0, fileData.Length);
            fileStream.Close();
        }

        private Sprite GetSpriteFromTexture(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.one * 0.5f);
        }

        private string GetFileNameForUpgrade(UpgradeDescription upgrade)
        {
            string upgradeTypeName = upgrade.UpgradeType.ToString();
            string upgradeLevel = upgrade.Level.ToString();

            return upgradeTypeName + UpgradeAndLevelFileNameSeparator + upgradeLevel + ".png";
        }
    }
}