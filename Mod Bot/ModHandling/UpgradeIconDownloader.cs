using ModLibrary;
using ModLibrary.ModTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace InternalModBot
{
    public class UpgradeIconDownloader : Singleton<UpgradeIconDownloader>
    {
        private List<DoubleValueHolder<UpgradeTypeAndLevel, Sprite>> DownloadedIcons = new List<DoubleValueHolder<UpgradeTypeAndLevel, Sprite>>();
        private List<DoubleValueHolder<UpgradeTypeAndLevel, string>> IconsToDownload = new List<DoubleValueHolder<UpgradeTypeAndLevel, string>>();

        private const string UpgradeIconsFolderName = "ModdedUpgradeIcons/";
        private string UpgradeIconsFolderPath => AssetLoader.GetModsFolderDirectory() + UpgradeIconsFolderName;

        private readonly float TimeToWaitBetweenRefreshingDownloadQueue = 5f;

        private const char UpgradeAndLevelFileNameSeparator = '_';

        private void Start()
        {
            // Create icons folder if it does not exist
            if (!Directory.Exists(UpgradeIconsFolderPath))
            {
                Directory.CreateDirectory(UpgradeIconsFolderPath);
            }

            DownloadAllQueuedIcons(); // There will never be any icons queued at this time, all we do here is start the loop that checks the queue every x seconds
            DownloadedIcons = GetAllSavedIcons(); // Get all icons in icons folder
            UpdateAllUpgradeIcons(); // Set all custom upgrade icons
        }

        public void AddUpgradeIcon(UpgradeDescription upgradeDescription, string url)
        {
            // Display warning message if two icons for the same upgrade and level are registered
            if (UpgradeHasIconConfigured(upgradeDescription))
            {
                debug.Log("WARNING: The upgrade " + upgradeDescription.UpgradeName + " already has a custom icon registered, some icons may not look correct!", Color.yellow);
            }

            // Add icon to download queue
            DoubleValueHolder<UpgradeTypeAndLevel, string> iconToDownload = new DoubleValueHolder<UpgradeTypeAndLevel, string>(GetUpgradeTypeAndLevel(upgradeDescription), url);
            IconsToDownload.Add(iconToDownload);
        }

        private bool UpgradeHasIconConfigured(UpgradeDescription upgradeDescription)
        {
            UpgradeTypeAndLevel upgradeTypeAndLevel = GetUpgradeTypeAndLevel(upgradeDescription);

            // Loop through all cached icons
            foreach (DoubleValueHolder<UpgradeTypeAndLevel, Sprite> downloadedIcons in DownloadedIcons)
            {
                if (downloadedIcons.FirstValue == upgradeTypeAndLevel)
                {
                    return true;
                }
            }

            // Loop through all queued icons
            foreach (DoubleValueHolder<UpgradeTypeAndLevel, string> iconsToDownload in IconsToDownload)
            {
                if (iconsToDownload.FirstValue == upgradeTypeAndLevel)
                {
                    return true;
                }
            }

            return false;
        }

        private UpgradeDescription GetUpgradeDescription(UpgradeTypeAndLevel upgradeTypeAndLevel)
        {
            return UpgradeManager.Instance.GetUpgrade(upgradeTypeAndLevel.UpgradeType, upgradeTypeAndLevel.Level);
        }

        private UpgradeTypeAndLevel GetUpgradeTypeAndLevel(UpgradeDescription upgradeDescription)
        {
            return new UpgradeTypeAndLevel
            {
                UpgradeType = upgradeDescription.UpgradeType,
                Level = upgradeDescription.Level
            };
        }

        private void UpdateAllUpgradeIcons()
        {
            // Loop through all cached icons
            foreach (DoubleValueHolder<UpgradeTypeAndLevel, Sprite> upgradeIconData in DownloadedIcons)
            {
                // Get values from member
                UpgradeDescription upgradeDescription = GetUpgradeDescription(upgradeIconData.FirstValue);
                Sprite icon = upgradeIconData.SecondValue;

                if (upgradeDescription == null)
                {
                    continue; // In this case, we have an icon for an upgrade that doesnt exist, so we want to ignore it
                }
                if (icon == null)
                {
                    debug.Log("Custom icon for upgrade " + upgradeDescription.UpgradeType.ToString() + " (level " + upgradeDescription.Level + ") could not be loaded! (Sprite was null)", Color.yellow);
                }

                upgradeDescription.Icon = icon;
            }
        }

        private Sprite GetIconFromImagePath(string filePath)
        {
            // Throw an exception if the file cant be found
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("", filePath);
            }

            byte[] rawData = File.ReadAllBytes(filePath); // Get all bytes from file
            Texture2D texture = new Texture2D(0, 0); // Create new Texture2D
            texture.LoadImage(rawData); // Load the Texture2D with the raw bytes

            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.one * 0.5f); // Create a sprite and return it
        }

        private UpgradeTypeAndLevel GetUpgradeTypeAndLevelFromFileName(string fileName)
        {
            // Check if the file name can be processed properly, if not, throw an exception
            CheckFileName(fileName);

            // Get UpgradeType
            string upgradeTypeString = fileName.Split(UpgradeAndLevelFileNameSeparator)[0];
            UpgradeType upgradeType = UpgradeTypeParseSafe(upgradeTypeString);

            // Get level
            string levelString = fileName.Split(UpgradeAndLevelFileNameSeparator)[1];
            int level = Convert.ToInt32(levelString);

            // Create new UpgradeTypeAndLevel and return it
            return new UpgradeTypeAndLevel
            {
                UpgradeType = upgradeType,
                Level = level
            };
        }

        private List<DoubleValueHolder<UpgradeTypeAndLevel, Sprite>> GetAllSavedIcons()
        {
            // Get all image files in the icons directory
            string[] filePaths = Directory.GetFiles(UpgradeIconsFolderPath, "*.png", SearchOption.TopDirectoryOnly);

            // Create new list to all all found files into
            List<DoubleValueHolder<UpgradeTypeAndLevel, Sprite>> downloadedIcons = new List<DoubleValueHolder<UpgradeTypeAndLevel, Sprite>>();

            // Loop through all file paths
            foreach (string filePath in filePaths)
            {
                // Get the file name
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                // Check if the file name can be processed properly, if not, throw an exception
                CheckFileName(fileName);

                // Get UpgradeTypeAndLevel and icon from path
                UpgradeTypeAndLevel upgradeTypeAndLevel = GetUpgradeTypeAndLevelFromFileName(fileName);
                Sprite icon = GetIconFromImagePath(filePath);

                // Add file to list
                downloadedIcons.Add(new DoubleValueHolder<UpgradeTypeAndLevel, Sprite>(upgradeTypeAndLevel, icon));
            }

            // Return all found icons
            return downloadedIcons;
        }

        private void AddAndUpdateIcon(DoubleValueHolder<UpgradeTypeAndLevel, Sprite> upgradeIcon)
        {
            // Add to cache
            DownloadedIcons.Add(upgradeIcon);

            // Get UpgradeDescription
            UpgradeTypeAndLevel upgradeTypeAndLevel = upgradeIcon.FirstValue;
            UpgradeDescription upgradeDescription = GetUpgradeDescription(upgradeTypeAndLevel);
            if (upgradeDescription == null)
            {
                return; // In this case, we have an icon for an upgrade that doesnt exist, so we want to ignore it
            }

            // Get icon
            Sprite icon = upgradeIcon.SecondValue;
            if (icon == null)
            {
                debug.Log("Custom icon for upgrade " + upgradeDescription.UpgradeType.ToString() + " (level " + upgradeDescription.Level + ") could not be loaded! (Sprite was null)", Color.yellow);
            }

            // Set icon
            upgradeDescription.Icon = icon;
        }

        private void DownloadAllQueuedIcons()
        {
            // If there are no icons in the queue, skip the loop
            if (IconsToDownload.Count > 0)
            {
                // Handle all items in the queue
                while (IconsToDownload.Count > 0)
                {
                    // Get UpgradeTypeAndLevel
                    DoubleValueHolder<UpgradeTypeAndLevel, string> iconToDownload = IconsToDownload[0];
                    UpgradeTypeAndLevel upgradeTypeAndLevel = iconToDownload.FirstValue;

                    // Generate a file name
                    string fileName = upgradeTypeAndLevel.UpgradeType.ToString() + UpgradeAndLevelFileNameSeparator + upgradeTypeAndLevel.Level.ToString();

                    // Get the URL
                    string url = iconToDownload.SecondValue;

                    // Download file
                    DownloadFileToIconsFolder(url, fileName);

                    // Remove item from queue
                    IconsToDownload.RemoveAt(0);
                }
            }

            DelegateScheduler.Instance.Schedule(DownloadAllQueuedIcons, TimeToWaitBetweenRefreshingDownloadQueue);
        }

        private void DownloadFileToIconsFolder(string url, string fileName)
        {
            // Generate full path to file
            string filePath = UpgradeIconsFolderPath + fileName + ".png";

            if (File.Exists(filePath))
            {
                return; // If the file already exists, an icon of the same upgrade and level has been downloaded
            }

            // ======== Standard file downloading code ========
            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
            WebClient webClient = new WebClient();
            if (webClient.IsBusy) // If WebClient is busy
            {
                debug.Log("WebClient busy! Trying again in a few seconds...");
                DelegateScheduler.Instance.Schedule(delegate { DownloadFileToIconsFolder(url, fileName); }, 2f); // Try again after 2 seconds
                return;
            }
            // ======== Standard file downloading code ========

            // Download data
            byte[] data = webClient.DownloadData(url);

            // Create and write to file
            FileStream fileStream = File.Create(filePath);
            fileStream.Write(data, 0, data.Length);
            fileStream.Close();

            // Get UpgradeTypeAndLevel and icon from file
            UpgradeTypeAndLevel upgradeTypeAndLevel = GetUpgradeTypeAndLevelFromFileName(fileName);
            Sprite icon = GetIconFromImagePath(filePath);

            // Add to cache and update
            DoubleValueHolder<UpgradeTypeAndLevel, Sprite> upgradeIcon = new DoubleValueHolder<UpgradeTypeAndLevel, Sprite>(upgradeTypeAndLevel, icon);
            AddAndUpdateIcon(upgradeIcon);
        }

        // Standard method to use in callback for file downloading
        private bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool result = true;
            if (sslPolicyErrors > SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        if (!chain.Build((X509Certificate2)certificate))
                        {
                            result = false;
                        }
                    }
                }
            }
            return result;
        }

        private void CheckFileName(string fileName)
        {
            if (fileName.Split(UpgradeAndLevelFileNameSeparator).Length < 2) // If the UpgradeType or level is missing from the name
            {
                throw new Exception("Unexpected amount of \"" + UpgradeAndLevelFileNameSeparator + "\" characters found! (Expected 1, got " + (fileName.Split(UpgradeAndLevelFileNameSeparator).Length - 1) + ")");
            }
        }

        private UpgradeType UpgradeTypeParseSafe(string name)
        {
            if (EnumTools.GetNames<UpgradeType>().Contains(name)) // If the value is in the enum we can parse normally
            {
                return (UpgradeType)Enum.Parse(typeof(UpgradeType), name);
            }

            return (UpgradeType)Convert.ToInt32(name); // If the value is not in the enum we can't parse it, so we have to convert it to and integer and then cast it
        }
    }
}