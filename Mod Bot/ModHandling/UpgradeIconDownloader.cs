using System;
using System.Collections.Generic;
using System.Text;
using ModLibrary;
using UnityEngine;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;
using System.Net;
using ModLibrary.ModTools;

namespace InternalModBot
{
    public class UpgradeIconDownloader : Singleton<UpgradeIconDownloader>
    {
        private List<DoubleValueHolder<UpgradeTypeAndLevel, Sprite>> DownloadedIcons = new List<DoubleValueHolder<UpgradeTypeAndLevel, Sprite>>();
        private List<DoubleValueHolder<UpgradeTypeAndLevel, string>> IconsToDownload = new List<DoubleValueHolder<UpgradeTypeAndLevel, string>>();

        private const string UpgradeIconsFolderName = "ModdedUpgradeIcons/";
        private string UpgradeIconsFolderPath => AssetLoader.GetModsFolderDirectory() + UpgradeIconsFolderName;

        private readonly float TimeToWaitBetweenDownloadingQueue = 5f;

        private void Start()
        {
            if (!Directory.Exists(UpgradeIconsFolderPath))
            {
                Directory.CreateDirectory(UpgradeIconsFolderPath);
            }

            DownloadAllQueuedIcons();
            DownloadedIcons = GetAllSavedIcons();
            UpdateAllUpgradeIcons();
        }

        public void AddUpgradeIcon(UpgradeDescription upgradeDescription, string url)
        {
            if (UpgradeHasIconConfigured(upgradeDescription))
            {
                debug.Log("WARNING: The upgrade " + upgradeDescription.UpgradeName + " already has a custom icon registered, some icons may not look correct!", Color.yellow);
            }

            DoubleValueHolder<UpgradeTypeAndLevel, string> iconToDownload = new DoubleValueHolder<UpgradeTypeAndLevel, string>(GetUpgradeTypeAndLevel(upgradeDescription), url);
            IconsToDownload.Add(iconToDownload);
        }

        private bool UpgradeHasIconConfigured(UpgradeDescription upgradeDescription)
        {
            UpgradeTypeAndLevel upgradeTypeAndLevel = GetUpgradeTypeAndLevel(upgradeDescription);

            foreach (DoubleValueHolder<UpgradeTypeAndLevel, Sprite> downloadedIcons in DownloadedIcons)
            {
                if (downloadedIcons.FirstValue == upgradeTypeAndLevel)
                {
                    return true;
                }
            }

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
            foreach (DoubleValueHolder<UpgradeTypeAndLevel, Sprite> upgradeIconData in DownloadedIcons)
            {
                UpgradeDescription upgradeDescription = GetUpgradeDescription(upgradeIconData.FirstValue);
                Sprite icon = upgradeIconData.SecondValue;

                if (upgradeDescription == null)
                {
                    continue;
                }
                if (icon == null)
                {
                    debug.Log("Custom icon for upgrade " + upgradeDescription.UpgradeType.ToString() + " (level " + upgradeDescription.Level + ") could not be loaded! (Sprite was null)", Color.yellow);
                }

                upgradeDescription.Icon = icon;
            }
        }

        private Sprite GetIconFromPNG(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("", filePath);
            }

            byte[] rawData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(rawData);

            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.one * 0.5f);
        }

        private List<DoubleValueHolder<UpgradeTypeAndLevel, Sprite>> GetAllSavedIcons()
        {
            string[] filePaths = Directory.GetFiles(UpgradeIconsFolderPath, "*.png", SearchOption.TopDirectoryOnly);

            List<DoubleValueHolder<UpgradeTypeAndLevel, Sprite>> downloadedIcons = new List<DoubleValueHolder<UpgradeTypeAndLevel, Sprite>>();

            foreach (string filePath in filePaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                if (fileName.Split('_').Length != 2)
                {
                    throw new Exception("Unexpected amount of \"_\" characters found! (Expected 2, got " + fileName.Split('_').Length + ")");
                }

                string upgradeTypeString = fileName.Split('_')[0];
                UpgradeType upgradeType;
                if (EnumTools.GetNames<UpgradeType>().Contains(upgradeTypeString))
                {
                    upgradeType = (UpgradeType)Enum.Parse(typeof(UpgradeType), upgradeTypeString);
                }
                else
                {
                    upgradeType = (UpgradeType)Convert.ToInt32(upgradeTypeString);
                }

                string levelString = fileName.Split('_')[1];
                int level = Convert.ToInt32(levelString);

                UpgradeTypeAndLevel upgradeTypeAndLevel = new UpgradeTypeAndLevel { UpgradeType = upgradeType, Level = level };
                Sprite icon = GetIconFromPNG(filePath);

                downloadedIcons.Add(new DoubleValueHolder<UpgradeTypeAndLevel, Sprite>(upgradeTypeAndLevel, icon));
            }

            return downloadedIcons;
        }

        private void DownloadAllQueuedIcons()
        {
            if (IconsToDownload.Count > 0)
            {
                while (IconsToDownload.Count > 0)
                {
                    DoubleValueHolder<UpgradeTypeAndLevel, string> iconToDownload = IconsToDownload[0];
                    UpgradeTypeAndLevel upgradeTypeAndLevel = iconToDownload.FirstValue;
                    string fileName = upgradeTypeAndLevel.UpgradeType.ToString() + "_" + upgradeTypeAndLevel.Level.ToString();

                    string url = iconToDownload.SecondValue;

                    DownloadFileToIconsFolder(url, fileName);
                    IconsToDownload.RemoveAt(0);

                    string filePath = UpgradeIconsFolderPath + fileName + ".png";
                    Sprite icon = GetIconFromPNG(fileName);

                    DoubleValueHolder<UpgradeTypeAndLevel, Sprite> upgradeIcon = new DoubleValueHolder<UpgradeTypeAndLevel, Sprite>(upgradeTypeAndLevel, icon);
                    DownloadedIcons.Add(upgradeIcon);
                }
            }

            DelegateScheduler.Instance.Schedule(DownloadAllQueuedIcons, TimeToWaitBetweenDownloadingQueue);
        }

        private void DownloadFileToIconsFolder(string url, string fileName)
        {
            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
            byte[] downloadedData;
            WebClient webClient = new WebClient
            {
                Headers =
                {
                    "User-Agent: Other"
                }
            };
            downloadedData = webClient.DownloadData(url);

            string filePath = UpgradeIconsFolderPath + fileName + ".png";

            if (File.Exists(filePath))
            {
                return;
            }

            FileStream fileStream = File.Create(filePath);
            fileStream.Write(downloadedData, 0, downloadedData.Length);
            fileStream.Close();
        }

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
    }
}