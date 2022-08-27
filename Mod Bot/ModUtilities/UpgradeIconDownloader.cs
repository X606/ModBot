using ModLibrary;
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
        const string UPGRADE_ICONS_FOLDER_NAME = "ModdedUpgradeIcons/";

        const char UPGRADE_AND_LEVEL_FILE_NAME_SEPARATOR = '_';

        static string upgradeIconsFolderPath => Path.Combine(Application.dataPath, UPGRADE_ICONS_FOLDER_NAME);

        void Start()
        {
            // Create icons folder if it does not exist
            if (!Directory.Exists(upgradeIconsFolderPath))
            {
                Directory.CreateDirectory(upgradeIconsFolderPath);
            }
        }

        /// <summary>
        /// Sets the icon of the upgrade to the image gotten from the url
        /// </summary>
        /// <param name="upgrade"></param>
        /// <param name="url"></param>
        public void SetIconOnUpgrade(UpgradeDescription upgrade, string url)
        {
            string fileName = getFileNameForUpgrade(upgrade);

            if (File.Exists(upgradeIconsFolderPath + fileName))
            {
                byte[] imageData = File.ReadAllBytes(upgradeIconsFolderPath + fileName);

                Texture2D texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
                texture.LoadImage(imageData);

                upgrade.Icon = getSpriteFromTexture(texture);
            }
            else
            {
                StartCoroutine(downloadImageAndSetIconOnUpgrade(upgrade, url));
            }
        }

        static IEnumerator downloadImageAndSetIconOnUpgrade(UpgradeDescription upgrade, string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    debug.Log(webRequest.error, Color.red);
                    upgrade.Icon = null;
                    yield break;
                }

                Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;

                upgrade.Icon = getSpriteFromTexture(texture);

                string fileName = getFileNameForUpgrade(upgrade);
                byte[] fileData = texture.EncodeToPNG();

                using (FileStream fileStream = File.Create(upgradeIconsFolderPath + fileName))
                {
                    fileStream.Write(fileData, 0, fileData.Length);
                }
            }
        }

        static Sprite getSpriteFromTexture(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.one * 0.5f);
        }

        static string getFileNameForUpgrade(UpgradeDescription upgrade)
        {
            string upgradeTypeName = upgrade.UpgradeType.ToString();
            string upgradeLevel = upgrade.Level.ToString();

            return upgradeTypeName + UPGRADE_AND_LEVEL_FILE_NAME_SEPARATOR + upgradeLevel + ".png";
        }
    }
}