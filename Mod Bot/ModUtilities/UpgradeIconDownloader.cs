using ModLibrary;
using System.Collections;
using System.IO;
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
            const string CUSTOM_ICON_POSTFIX = "-ModdedIcon";

            string fileName = getFileNameForUpgrade(upgrade);

            // If there already is an icon with a modded icon texture, destroy both
            if (upgrade.Icon != null && upgrade.Icon.texture != null && upgrade.Icon.texture.name.EndsWith(CUSTOM_ICON_POSTFIX))
            {
                Destroy(upgrade.Icon.texture);
                Destroy(upgrade.Icon);
            }

            string textureName = fileName + CUSTOM_ICON_POSTFIX;
            string textureFilePath = Path.Combine(upgradeIconsFolderPath, fileName);
            if (File.Exists(textureFilePath))
            {
                StartCoroutine(loadImageFromDiskAndSetIconOnUpgrade(upgrade, textureFilePath, textureName));
            }
            else
            {
                StartCoroutine(downloadImageAndSetIconOnUpgrade(upgrade, url, textureName));
            }
        }

        static IEnumerator downloadImageAndSetIconOnUpgrade(UpgradeDescription upgrade, string url, string textureName)
        {
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    debug.Log($"Failed to download upgrade icon \"{textureName}\" from network. ({webRequest.error})", Color.red);
                    upgrade.Icon = null;
                    yield break;
                }

                Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                texture.name = textureName;

                upgrade.Icon = getSpriteFromTexture(texture);

                string textureFilePath = Path.Combine(upgradeIconsFolderPath, getFileNameForUpgrade(upgrade));
                File.WriteAllBytes(Path.Combine(upgradeIconsFolderPath, textureFilePath), texture.EncodeToPNG());
            }
        }

        static IEnumerator loadImageFromDiskAndSetIconOnUpgrade(UpgradeDescription upgrade, string path, string textureName)
        {
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture($"file://{path}"))
            {
                webRequest.timeout = 5;
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    debug.Log($"Failed to load upgrade icon \"{textureName}\" from disk. ({webRequest.error})", Color.red);
                    upgrade.Icon = null;
                    yield break;
                }

                Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                texture.name = textureName;

                upgrade.Icon = getSpriteFromTexture(texture);
            }
        }

        static Sprite getSpriteFromTexture(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
        }

        static string getFileNameForUpgrade(UpgradeDescription upgrade)
        {
            string upgradeTypeName = upgrade.UpgradeType.ToString();
            string upgradeLevel = upgrade.Level.ToString();

            return upgradeTypeName + UPGRADE_AND_LEVEL_FILE_NAME_SEPARATOR + upgradeLevel + ".png";
        }
    }
}