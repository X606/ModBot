using ModLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    public class ModDownloadWindow : MonoBehaviour
    {
        private Button _xButton;

        private ModdedObject _modCardPrefab;
        private Transform _container;

        private InputField _searchField;
        private Button _websiteButton;

        private Transform _informationWindow;
        private RawImage _modPreview;
        private Text _modName;
        private Text _modDescription;
        private Text _modVersion;

        private readonly List<ModInfoCard> _cards = new List<ModInfoCard>();

        private ModsHolder _modsHolder;

        internal void Init()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            _xButton = moddedObject.GetObject<Button>(3);
            _xButton.onClick.AddListener(Hide);
            _modCardPrefab = moddedObject.GetObject<ModdedObject>(0);
            _modCardPrefab.gameObject.SetActive(false);
            _container = moddedObject.GetObject<Transform>(2);
            _searchField = moddedObject.GetObject<InputField>(1);
            _searchField.onValueChanged.AddListener(ShowModsWithMatchingNames);
            _websiteButton = moddedObject.GetObject<Button>(4);
            _websiteButton.onClick.AddListener(OpenWebsite);
            _informationWindow = moddedObject.GetObject<Transform>(5);
            _informationWindow.gameObject.SetActive(false);
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
            foreach (ModInfoCard ui in _cards)
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
            if (!base.gameObject.activeInHierarchy) return;

            StopAllCoroutines();

            List<Tuple<ModInfo, int>> modsAndLikes = new List<Tuple<ModInfo, int>>();
            foreach (ModInfo info in _modsHolder.Mods)
            {
                ModSpecialData specialData = ModsDownloadManager.GetSpecialDataFor(info.UniqueID);
                if (specialData == null)
                {
                    modsAndLikes.Add(new Tuple<ModInfo, int>(info, -1));
                    continue;
                }

                modsAndLikes.Add(new Tuple<ModInfo, int>(info, -specialData.Likes));
            }

            foreach (Tuple<ModInfo, int> tuple in modsAndLikes.OrderBy(t => t.Item2))
            {
                ModInfo info = tuple.Item1;
                ModSpecialData specialData = ModsDownloadManager.GetSpecialDataFor(info.UniqueID);

                if (info.Tags != null && info.Tags.Contains("vr")) continue;

                ModInfoCard infoCard = Instantiate(_modCardPrefab, _container).gameObject.AddComponent<ModInfoCard>();
                infoCard.Init(info, specialData);
                infoCard.MakeInvisible();
                _cards.Add(infoCard);
            }

            StartCoroutine(fadeOutModCards());
        }

        private IEnumerator fadeOutModCards()
        {
            bool hasToWait = true;
            foreach (ModInfoCard card in _cards)
            {
                if (card.IsOffScreen()) hasToWait = false; // fade out only first couple of entries as they're the first thing the use sees

                if(hasToWait) yield return new WaitForSecondsRealtime(0.05f);

                card.FadeOut();
            }
            yield break;
        }

        internal void LoadDownloadPage()
        {
            _cards.Clear();
            TransformUtils.DestroyAllChildren(_container);
            ModBotUIRoot.Instance.LoadingBar.SetActive("Loading mods", 0f);
            ModsDownloadManager.GetModInfos(onGotModInfos, onReqestProgress);
        }

        private void onReqestProgress(ModsDownloadManager.GetModInfosProgress progress)
        {
            if (!base.gameObject.activeInHierarchy) return;

            float totalProgress = (progress.GettingSpecialData ? 0.5f : 0f) + (progress.Progress * 0.5f);
            ModBotUIRoot.Instance.LoadingBar.SetProgress(totalProgress);
        }

        private void onGotModInfos(ModsDownloadManager.GetModInfosResult getModInfosResult)
        {
            if (getModInfosResult.HasFailed())
            {
                Hide();
                ModBotUIRoot.Instance.LoadingBar.SetActive(false);
                if (ModBotUIRoot.Instance.ModList.gameObject.activeInHierarchy) _ = new Generic2ButtonDialogue(getModInfosResult.Error, "Ok", null, "Visit Website", ModBotUIRoot.Instance.DownloadWindow.OpenWebsite);
                return;
            }

            _modsHolder = getModInfosResult.Holder.Value;
            ModBotUIRoot.Instance.LoadingBar.SetActive(false);
            PopulateModsHolder();
        }

        public void OpenWebsite()
        {
            Application.OpenURL("https://modbot.org/modBrowsing.html");
        }

        internal void OpenInformationWindow(ModInfo info, ModSpecialData specialData, Texture previewImage)
        {
            _informationWindow.gameObject.SetActive(info != null && specialData != null);
            if (info == null || specialData == null)
            {
                return;
            }

            _modName.text = info.DisplayName;
            _modDescription.text = info.Description;
            _modPreview.texture = previewImage;
            _modVersion.text = $"VERSION {info.Version}\n{specialData.Downloads} DOWNLOADS";
        }

        private void closeInformationWindow()
        {
            OpenInformationWindow(null, null, null);
        }
    }
}