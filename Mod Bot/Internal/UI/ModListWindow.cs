using ModLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    public class ModListWindow : MonoBehaviour
    {
        private Transform _container;

        private Button _xButton;
        private Button _getMoreModsButton;
        private Button _modsFolderButton;
        private Button _reloadListButton;

        private Dropdown _sortDropdown;
        private InputField _searchBox;

        private Text _modsInfoLabel;

        private List<LocalModInfoDisplay> _instantiatedDisplays;

        private int _sortType;

        private bool _isPopulatingList;

        private bool _hasEverPopulatedList;

        public void Init()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();

            _xButton = moddedObject.GetObject<Button>(4);
            _xButton.onClick.AddListener(Hide);
            _getMoreModsButton = moddedObject.GetObject<Button>(0);
            _getMoreModsButton.onClick.AddListener(OnGetMoreModsButtonClicked);
            _modsFolderButton = moddedObject.GetObject<Button>(1);
            _modsFolderButton.onClick.AddListener(OnModsFolderButtonClicked);
            _reloadListButton = moddedObject.GetObject<Button>(3);
            _reloadListButton.onClick.AddListener(OnReloadListButtonClicked);

            _searchBox = moddedObject.GetObject<InputField>(2);
            _searchBox.onValueChanged.AddListener(OnSearchBoxValueChanged);
            _sortDropdown = moddedObject.GetObject<Dropdown>(6);
            _sortDropdown.value = 0;
            _sortDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            _container = moddedObject.GetObject<Transform>(5);

            _modsInfoLabel = moddedObject.GetObject<Text>(11);

            _instantiatedDisplays = new List<LocalModInfoDisplay>();

            base.gameObject.SetActive(false);
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            GameUIRoot.Instance.SetEscMenuDisabled(true);

            if (!_hasEverPopulatedList)
            {
                ReloadList();
                _hasEverPopulatedList = true;
            }
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
            GameUIRoot.Instance.SetEscMenuDisabled(false);

            ModBotUIRoot.Instance.LoadingBar.SetActive(false);

            _isPopulatingList = false;
            _hasEverPopulatedList = !_isPopulatingList;
            setElementsInteractable(true);
            if (!string.IsNullOrEmpty(_searchBox.text)) _searchBox.text = string.Empty;
        }

        public void ReloadList()
        {
            if (!base.gameObject.activeInHierarchy || _isPopulatingList)
                return;

            _instantiatedDisplays.Clear();
            TransformUtils.DestroyAllChildren(_container);
            _ = base.StartCoroutine(reloadListCoroutine());
        }

        private IEnumerator reloadListCoroutine()
        {
            _isPopulatingList = true;
            setElementsInteractable(false);
            RefreshModsInfoLabel();

            GenericLoadingBar loadingBar = ModBotUIRoot.Instance.LoadingBar;
            loadingBar.SetActive("Loading mod list...", 0f);

            yield return null;

            int index = 0;
            List<LoadedModInfo> mods = ModsManager.Instance.GetAllMods();
            List<LoadedModInfo> orderedMods;
            switch (_sortType)
            {
                case 0:
                    orderedMods = mods.OrderBy(mod => mod.OwnerModInfo.DisplayName).ToList();
                    break;
                case 1:
                    orderedMods = mods.OrderByDescending(mod => mod.OwnerModInfo.DisplayName).ToList();
                    break;
                case 2:
                    orderedMods = mods.OrderBy(mod => !mod.IsEnabled).ToList();
                    break;
                case 3:
                    orderedMods = mods.OrderBy(mod => mod.OwnerModInfo.Author).ToList();
                    break;
                case 4:
                    orderedMods = mods.OrderBy(mod => -mod.OwnerModInfo.Version).ToList();
                    break;
                default:
                    orderedMods = mods;
                    break;
            }

            long totalMs = 0;
            Stopwatch stopwatch = new Stopwatch();
            foreach (LoadedModInfo info in orderedMods)
            {
                stopwatch.Restart();

                GameObject modItem = InternalAssetBundleReferences.ModBot.InstantiateObject("ModItemPrefab");
                modItem.transform.SetParent(_container, false);
                modItem.SetActive(false);

                LocalModInfoDisplay component = modItem.AddComponent<LocalModInfoDisplay>();
                component.Init(info);
                _instantiatedDisplays.Add(component);

                stopwatch.Stop();
                totalMs += stopwatch.ElapsedMilliseconds;

                if (totalMs > 50)
                {
                    totalMs = 0;
                    yield return null;
                }

                loadingBar.SetProgress(index, mods.Count);
                index++;
            }

            foreach (LocalModInfoDisplay element in _instantiatedDisplays)
            {
                element.gameObject.SetActive(true);
            }

            _isPopulatingList = false;
            _hasEverPopulatedList = true;
            setElementsInteractable(true);
            RefreshModsInfoLabel();
            loadingBar.SetActive(false);
            yield break;
        }

        public void RefreshModsInfoLabel()
        {
            Text label = _modsInfoLabel;
            if (_isPopulatingList)
            {
                label.text = "Loading...";
                return;
            }

            if (!ModsManager.Instance)
            {
                label.text = "ERROR, ModsManager is null";
                return;
            }

            List<LoadedModInfo> list = ModsManager.Instance.GetAllMods();
            if (list == null)
            {
                label.text = "ERROR, Mod list is null";
                return;
            }

            label.text = $"{list.Count} mods installed, {ModsManager.Instance.GetAllLoadedActiveMods()?.Count} enabled";
        }

        private void setElementsInteractable(bool value)
        {
            _getMoreModsButton.interactable = value;
            _reloadListButton.interactable = value;
            _searchBox.interactable = value;
            _sortDropdown.interactable = value;
        }

        public void OnGetMoreModsButtonClicked()
        {
            ModBotUIRoot.Instance.DownloadWindow.Show();
        }

        public void OnModsFolderButtonClicked()
        {
            _ = Process.Start(ModsManager.Instance.ModFolderPath);
        }

        public void OnReloadListButtonClicked()
        {
            ReloadList();
        }

        public void OnSearchBoxValueChanged(string text)
        {
            if (_isPopulatingList)
                return;

            text = text.ToLower();
            bool isEmpty = string.IsNullOrWhiteSpace(text);
            foreach (LocalModInfoDisplay display in _instantiatedDisplays)
            {
                string displayText = display.loadedModInfo?.OwnerModInfo?.DisplayName;
                if (isEmpty || string.IsNullOrEmpty(displayText))
                {
                    display.gameObject.SetActive(true);
                    continue;
                }
                display.gameObject.SetActive(displayText.ToLower().Contains(text));
            }
        }

        public void OnDropdownValueChanged(int value)
        {
            _sortType = value;
            ReloadList();
        }
    }
}
