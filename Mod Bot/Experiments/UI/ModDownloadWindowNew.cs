using ModLibrary;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace InternalModBot
{
    public class ModDownloadWindowNew : MonoBehaviour
    {
        private ModdedObject m_ModdedObject;
        private UnityWebRequest m_CurrentWebRequest;
        private ModsHolder m_CurrentModsHolder;

        private ModdedObject m_ModInfoEntryPrefab;
        private Transform m_ModInfoEntriesContainer;

        private InputField m_SearchField;
        private Button m_WebsiteButton;

        private Transform m_InformationWindow;
        private RawImage m_ModPreview;
        private Text m_ModName;
        private Text m_ModDescription;
        private Text m_ModVersion;

        private readonly List<ModInfoUIVizualizator> m_ModInfos = new List<ModInfoUIVizualizator>();

        internal ModDownloadWindowNew Init()
        {
            m_ModdedObject = base.GetComponent<ModdedObject>();
            m_ModdedObject.GetObject_Alt<Button>(3).onClick.AddListener(Hide);
            m_ModInfoEntryPrefab = m_ModdedObject.GetObject_Alt<ModdedObject>(0);
            m_ModInfoEntryPrefab.gameObject.SetActive(false);
            m_ModInfoEntriesContainer = m_ModdedObject.GetObject_Alt<Transform>(2);
            m_SearchField = m_ModdedObject.GetObject_Alt<InputField>(1);
            m_SearchField.onValueChanged.AddListener(ShowModsWithMatchingNames);
            m_WebsiteButton = m_ModdedObject.GetObject_Alt<Button>(4);
            m_WebsiteButton.onClick.AddListener(OpenWebsite);
            m_InformationWindow = m_ModdedObject.GetObject_Alt<Transform>(5);
            m_ModPreview = m_ModdedObject.GetObject_Alt<RawImage>(7);
            m_ModName = m_ModdedObject.GetObject_Alt<Text>(8);
            m_ModDescription = m_ModdedObject.GetObject_Alt<Text>(9);
            m_ModVersion = m_ModdedObject.GetObject_Alt<Text>(10);
            m_ModdedObject.GetObject_Alt<Button>(6).onClick.AddListener(closeInformationWindow);
            Hide();

            return this;
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            LoadDownloadPage();
        }

        public void Hide()
        {
            StopAllCoroutines();
            closeInformationWindow();
            ModBotHUDRootNew.LoadingBar.SetActive(false);
            base.gameObject.SetActive(false);
        }

        public void ShowModsWithMatchingNames(string name)
        {
            foreach(ModInfoUIVizualizator ui in m_ModInfos)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    ui.gameObject.SetActive(true);
                    continue;
                }
                ui.gameObject.SetActive(ui.ModName.ToLower().Contains(name.ToLower()));
            }
        }

        public void PopulateModsHolder()
        {
            if (!base.gameObject.activeInHierarchy)
            {
                return;
            }
            _ = StartCoroutine(asyncPopulateModsHolder());
        }
        private IEnumerator asyncPopulateModsHolder()
        {
            float wait = 0.04f;
            int index = 1;
            foreach (ModInfo info in m_CurrentModsHolder.Mods)
            {
                yield return new WaitForSecondsRealtime(wait);

                ModInfoUIVizualizator v = Instantiate(m_ModInfoEntryPrefab, m_ModInfoEntriesContainer).gameObject.AddComponent<ModInfoUIVizualizator>().Init(info);
                m_ModInfos.Add(v);

                index++;
                if(index >= 9)
                {
                    wait = 0.1f;
                }
            }
            yield break;
        }

        internal void LoadDownloadPage()
        {
            m_ModInfos.Clear();
            TransformUtils.DestroyAllChildren(m_ModInfoEntriesContainer);
            ModBotHUDRootNew.LoadingBar.SetActive("Loading mods", 0f);
            ModBotWebsiteInteraction.RequestAllModInfos(delegate (UnityWebRequest r)
            {
                m_CurrentWebRequest = r;
            }, OnLoadedModInfos, OnFailedToLoadModInfos);
        }

        internal void OnLoadedModInfos(ModsHolder? holder)
        {
            m_CurrentModsHolder = holder.Value;
            ModBotHUDRootNew.LoadingBar.SetActive(false);
            PopulateModsHolder();
        }

        internal void OnFailedToLoadModInfos(string error)
        {
            Hide();
            ModBotHUDRootNew.LoadingBar.SetActive(false);
            if(ModBotUIRoot.Instance.ModsWindow.WindowObject.activeSelf) ModErrorManager.ShowModBotSiteError(error);
        }

        public void OpenWebsite()
        {
            Application.OpenURL("https://modbot.org/modBrowsing.html");
        }

        public void OpenInformationWindow(ModInfo info, Dictionary<string, JToken> specialData, Texture previewImage)
        {
            m_InformationWindow.gameObject.SetActive(info != null && specialData != null);
            if(info == null || specialData == null)
            {
                return;
            }

            m_ModName.text = info.DisplayName;
            m_ModDescription.text = info.Description;
            m_ModPreview.texture = previewImage;
            m_ModVersion.text = "version " + info.Version + ", " + specialData["Downloads"].ToObject<string>() + " downloads";
        }

        private void closeInformationWindow()
        {
            OpenInformationWindow(null, null, null);
        }
    }
}