using ModLibrary;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace InternalModBot
{
    public class ModDownloadWindow : MonoBehaviour
    {
        private Button _xButton;

        private ModdedObject _modInfoEntryPrefab;
        private Transform _modInfoEntriesContainer;

        private InputField _searchField;
        private Button _websiteButton;

        private Transform _informationWindow;
        private RawImage _modPreview;
        private Text _modName;
        private Text _modDescription;
        private Text _modVersion;

        private readonly List<ModInfoDisplay> _displays = new List<ModInfoDisplay>();

        private UnityWebRequest _webRequest;
        private ModsHolder _modsHolder;

        internal void Init()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            _xButton = moddedObject.GetObject<Button>(3);
            _xButton.onClick.AddListener(Hide);
            _modInfoEntryPrefab = moddedObject.GetObject<ModdedObject>(0);
            _modInfoEntryPrefab.gameObject.SetActive(false);
            _modInfoEntriesContainer = moddedObject.GetObject<Transform>(2);
            _searchField = moddedObject.GetObject<InputField>(1);
            _searchField.onValueChanged.AddListener(ShowModsWithMatchingNames);
            _websiteButton = moddedObject.GetObject<Button>(4);
            _websiteButton.onClick.AddListener(OpenWebsite);
            _informationWindow = moddedObject.GetObject<Transform>(5);
            _modPreview = moddedObject.GetObject<RawImage>(7);
            _modName = moddedObject.GetObject<Text>(8);
            _modDescription = moddedObject.GetObject<Text>(9);
            _modVersion = moddedObject.GetObject<Text>(10);
            moddedObject.GetObject<Button>(6).onClick.AddListener(closeInformationWindow);

            base.gameObject.SetActive(false);
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
            ModBotUIRoot.Instance.LoadingBar.SetActive(false);
            base.gameObject.SetActive(false);
        }

        public void ShowModsWithMatchingNames(string name)
        {
            foreach(ModInfoDisplay ui in _displays)
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
            foreach (ModInfo info in _modsHolder.Mods)
            {
                yield return new WaitForSecondsRealtime(wait);

                ModInfoDisplay v = Instantiate(_modInfoEntryPrefab, _modInfoEntriesContainer).gameObject.AddComponent<ModInfoDisplay>().Init(info);
                _displays.Add(v);

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
            _displays.Clear();
            TransformUtils.DestroyAllChildren(_modInfoEntriesContainer);
            ModBotUIRoot.Instance.LoadingBar.SetActive("Loading mods", 0f);
            ModBotWebsiteInteraction.RequestAllModInfos(delegate (UnityWebRequest r)
            {
                _webRequest = r;
            }, OnLoadedModInfos, OnFailedToLoadModInfos);
        }

        internal void OnLoadedModInfos(ModsHolder? holder)
        {
            _modsHolder = holder.Value;
            ModBotUIRoot.Instance.LoadingBar.SetActive(false);
            PopulateModsHolder();
        }

        internal void OnFailedToLoadModInfos(string error)
        {
            Hide();
            ModBotUIRoot.Instance.LoadingBar.SetActive(false);
            if(ModBotUIRoot.Instance.ModList.gameObject.activeInHierarchy) _ = new Generic2ButtonDialogue(error, "Ok", null, "Visit Website", ModBotUIRoot.Instance.DownloadWindow.OpenWebsite);
        }

        public void OpenWebsite()
        {
            Application.OpenURL("https://modbot.org/modBrowsing.html");
        }

        public void OpenInformationWindow(ModInfo info, Dictionary<string, JToken> specialData, Texture previewImage)
        {
            _informationWindow.gameObject.SetActive(info != null && specialData != null);
            if(info == null || specialData == null)
            {
                return;
            }

            _modName.text = info.DisplayName;
            _modDescription.text = info.Description;
            _modPreview.texture = previewImage;
            _modVersion.text = "version " + info.Version + ", " + specialData["Downloads"].ToObject<string>() + " downloads";
        }

        private void closeInformationWindow()
        {
            OpenInformationWindow(null, null, null);
        }
    }
}