using ModLibrary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InternalModBot
{
    internal class LocalModInfoDisplay : MonoBehaviour
    {
        public const string BG_ENABLED_COLOR = "#373C43";
        public const string BG_DISABLED_COLOR = "#801500";

        public const string TOGGLE_ENABLED_COLOR = "#FF3333";
        public const string TOGGLE_DISABLED_COLOR = "#14CC17";

        public const string AUTHOR_COLOR = "#5ABAFF";
        public const string VERSION_COLOR = "#FF9500";

        private Button _toggleButton;
        private Button _optionsButton;
        private Button _errorsButton;
        private Button _shareButton;
        private Button _updateButton;

        private RawImage _modImage;
        private GameObject _modImageObject;
        private GameObject _placeholderModImageObject;

        private LocalizedTextField _toggleButtonText;
        private Text _optionsButtonText;

        private GameObject _descriptionHolderObject;
        private Transform _descriptionHolderTransform;
        private Image _descriptionHolderImage;

        private CanvasGroup canvasGroup;
        private bool _initialized;

        public LoadedModInfo loadedModInfo
        {
            get;
            private set;
        }

        public void Init(LoadedModInfo modInfo)
        {
            loadedModInfo = modInfo;

            ColorUtility.TryParseHtmlString(AUTHOR_COLOR, out Color authorColor);
            ColorUtility.TryParseHtmlString(VERSION_COLOR, out Color versionColor);

            string authorString = $"By {modInfo.OwnerModInfo.Author}".AddColor(authorColor);
            string versionString = $"Version {modInfo.OwnerModInfo.Version}".AddColor(versionColor);
            string idString = $"ID <size=9>{modInfo.OwnerModInfo.UniqueID}</size>";
            string displayName = modInfo.OwnerModInfo.DisplayName;
            bool notLoaded = modInfo.IsEnabled && modInfo.ModReference == null;

            canvasGroup = base.GetComponent<CanvasGroup>();
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            moddedObject.GetObject<Text>(0).text = notLoaded ? $"{displayName} (Not loaded)".AddColor(Color.yellow) : displayName;
            moddedObject.GetObject<Text>(1).text = modInfo.OwnerModInfo.Description;
            moddedObject.GetObject<Text>(9).text = $"{authorString} · {versionString} · {idString}";

            moddedObject.GetObject<Button>(7).onClick.AddListener(OnCopyIDButtonClicked);
            moddedObject.GetObject<Button>(11).onClick.AddListener(OnFolderButtonClicked);

            _toggleButton = moddedObject.GetObject<Button>(3);
            _toggleButton.onClick.AddListener(OnToggleButtonClicked);
            _optionsButton = moddedObject.GetObject<Button>(4);
            _optionsButton.onClick.AddListener(OnOptionsButtonClicked);
            _errorsButton = moddedObject.GetObject<Button>(10);
            _errorsButton.onClick.AddListener(OnErrorsButtonClicked);
            _shareButton = moddedObject.GetObject<Button>(6);
            _shareButton.onClick.AddListener(OnShareButtonClicked);
            _updateButton = moddedObject.GetObject<Button>(15);
            _updateButton.onClick.AddListener(OnUpdateButtonClicked);

            _descriptionHolderImage = moddedObject.GetObject<Image>(13);
            _descriptionHolderObject = _descriptionHolderImage.gameObject;
            _descriptionHolderTransform = _descriptionHolderImage.transform;

            _modImage = moddedObject.GetObject<RawImage>(2);
            _modImageObject = _modImage.gameObject;
            _placeholderModImageObject = moddedObject.GetObject<RawImage>(8).gameObject;

            _toggleButtonText = moddedObject.GetObject<Text>(12).GetComponent<LocalizedTextField>();
            _optionsButtonText = moddedObject.GetObject<Text>(14);

            DescriptionHolder descriptionHolder = _descriptionHolderObject.AddComponent<DescriptionHolder>();
            descriptionHolder.ContentTransform = base.transform.parent;

            Refresh();
            LoadImage();

            _initialized = true;
        }

        public void Refresh()
        {
            LoadedModInfo modInfo = loadedModInfo;
            ModListWindow modListWindow = ModBotUIRoot.Instance.ModList;
            bool isEnabled = loadedModInfo.IsEnabled;
            bool isMultiplayer = GameModeManager.IsMultiplayer();
            bool hasSettings = isEnabled && modInfo.ModReference != null && modInfo.ModReference.ImplementsSettingsWindow();

            setColor(base.transform, isEnabled ? BG_ENABLED_COLOR : BG_DISABLED_COLOR);
            setColor(_toggleButton.transform, isEnabled ? TOGGLE_ENABLED_COLOR : TOGGLE_DISABLED_COLOR);
            setColor(_descriptionHolderTransform, isEnabled ? BG_ENABLED_COLOR : BG_DISABLED_COLOR);

            _optionsButtonText.color = hasSettings ? Color.white : Color.gray;

            //_optionsButton.gameObject.SetActive(isEnabled);
            _updateButton.gameObject.SetActive(GameModeManager.IsOnTitleScreen() && modListWindow.DoesModHaveUpdate(modInfo.OwnerModInfo, out _));
            _updateButton.interactable = !modListWindow.GetIsUpdatingMods();
            _optionsButton.interactable = hasSettings;
            _descriptionHolderObject.SetActive(true);
            _errorsButton.gameObject.SetActive(false);
            _shareButton.gameObject.SetActive(isMultiplayer);
            _toggleButtonText.ChangeIDAndTryLocalize(isEnabled ? "mods_menu_disable_mod" : "mods_menu_enable_mod");
        }

        public void LoadImage()
        {
            ModInfo modInfo = loadedModInfo.OwnerModInfo;
            bool hasImage = modInfo.HasImage;

            _modImageObject.SetActive(false);
            _placeholderModImageObject.SetActive(true);

            if (hasImage) ModsManager.Instance.GetModImage(modInfo, onLoadedImage);
        }

        private void setColor(Transform transform, string hexColor)
        {
            ColorUtility.TryParseHtmlString(hexColor, out Color color);
            Graphic graphic = transform.GetComponent<Graphic>();
            if (graphic)
            {
                graphic.color = color;
            }
        }

        private void setModActive(LoadedModInfo mod, bool value)
        {
            if (mod.IsEnabled == value) return;

            try // ensure that the mod list will still work properly if the mod is enabled/disabled with an error
            {
                mod.IsEnabled = value;
            }
            catch (System.Exception exc)
            {
                UnityEngine.Debug.LogException(exc);
            }
        }

        public void OnToggleButtonClicked()
        {
            if (loadedModInfo == null)
                return;

            ModInfo modInfo = loadedModInfo.OwnerModInfo;
            if (!loadedModInfo.IsEnabled && modInfo.ModDependencies != null && modInfo.ModDependencies.Length != 0)
            {
                ModsManager modsManager = ModsManager.Instance;
                List<LoadedModInfo> dependencies = new List<LoadedModInfo>();
                addModDependenciesRecursive(loadedModInfo, modsManager.GetAllMods(), ref dependencies);

                List<LoadedModInfo> toEnable = new List<LoadedModInfo>();
                foreach (LoadedModInfo mod in dependencies)
                {
                    if (mod == loadedModInfo) continue;

                    if (string.IsNullOrEmpty(mod.OwnerModInfo.DisplayName) || !mod.IsEnabled) toEnable.Add(mod);
                }

                if (toEnable.Count != 0)
                {
                    bool hasMissingMods = false;

                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine($"Mod \"{modInfo.DisplayName}\" requires following mods to be installed and enabled:");
                    stringBuilder.AppendLine();
                    foreach (LoadedModInfo lmi in toEnable)
                    {
                        if (string.IsNullOrEmpty(lmi.OwnerModInfo.DisplayName))
                        {
                            stringBuilder.AppendLine($"A mod with id {lmi.OwnerModInfo.UniqueID} (missing)".AddColor(Color.red));
                            hasMissingMods = true;
                        }
                        else
                        {
                            stringBuilder.AppendLine($"{lmi.OwnerModInfo.DisplayName} (disabled)".AddColor(Color.yellow));
                        }
                    }
                    stringBuilder.AppendLine();

                    if (hasMissingMods)
                    {
                        stringBuilder.Append("You cannot enable this mod due to missing mods.\nCheck mod's description for information or ask around Discord how to fix this.");
                        new Generic2ButtonDialogue(stringBuilder.ToString(), "Ok", null, "Get mods", delegate
                        {
                            ModBotUIRoot.Instance.DownloadWindow.Show();
                        });
                        return;
                    }

                    stringBuilder.Append("Would you like to enable these mods?");
                    new Generic2ButtonDialogue(stringBuilder.ToString(), "Yes", delegate
                    {
                        foreach (LoadedModInfo lmi in toEnable)
                        {
                            if (string.IsNullOrEmpty(lmi.OwnerModInfo.DisplayName)) continue;

                            setModActive(lmi, true);
                        }
                        setModActive(loadedModInfo, true);

                        ModBotUIRoot.Instance.ModList.RefreshDisplays();
                    }, "No", null);

                    return;
                }
            }

            setModActive(loadedModInfo, !loadedModInfo.IsEnabled);
            Refresh();
            ModBotUIRoot.Instance.ModList.RefreshModsInfoLabel();
        }

        public void OnOptionsButtonClicked()
        {
            Mod mod = loadedModInfo.ModReference;

            ModOptionsWindowBuilder builder = new ModOptionsWindowBuilder(ModBotUIRoot.Instance.ModList.gameObject, mod);
            mod.CreateSettingsWindow(builder);
        }

        public void OnShareButtonClicked()
        {
            ModInfo modInfo = loadedModInfo.OwnerModInfo;

            new Generic2ButtonDialogue(ModBotLocalizationManager.FormatLocalizedStringFromID("mods_menu_broadcast_confirm_message", modInfo.DisplayName),
                LocalizationManager.Instance.GetTranslatedString("mods_menu_broadcast_confirm_no"), null,
                LocalizationManager.Instance.GetTranslatedString("mods_menu_broadcast_confirm_yes"), delegate
                {
                    ModSharingManager.SendModToAllModBotClients(modInfo.UniqueID);
                });
        }

        public void OnUpdateButtonClicked()
        {
            ModInfo modInfo = loadedModInfo.OwnerModInfo;
            ModListWindow modListWindow = ModBotUIRoot.Instance.ModList;
            if (!modListWindow.DoesModHaveUpdate(modInfo, out ModInfo newInfo)) return;

            ColorUtility.TryParseHtmlString(VERSION_COLOR, out Color color);

            new Generic2ButtonDialogue($"Update {newInfo.DisplayName.AddColor(color)}?", "Yes", delegate
            {
                modListWindow.SetIsUpdatingMods(true);
                ModsDownloadManager.DownloadMod(new ModsDownloadManager.ModGeneralInfo()
                {
                    DisplayName = newInfo.DisplayName,
                    UniqueID = newInfo.UniqueID,
                    Version = newInfo.Version
                }, true, delegate (ModsDownloadManager.DownloadModResult downloadModResult)
                {
                    if (!modListWindow) return;

                    modListWindow.SetIsUpdatingMods(false);

                    if (downloadModResult.HasFailed())
                    {
                        _ = new Generic2ButtonDialogue($"Failed to update {newInfo.DisplayName}.\n{downloadModResult.Error}",
                            "Ok", null,
                            "Ok", null);

                        return;
                    }

                    modListWindow.OnUpdatedMod(modInfo);
                });
            }, "No", null);
        }

        public void OnCopyIDButtonClicked()
        {
            string id = loadedModInfo?.OwnerModInfo?.UniqueID;
            if (string.IsNullOrEmpty(id))
                return;

            GUIUtility.systemCopyBuffer = id;
        }

        public void OnFolderButtonClicked()
        {
            string path = loadedModInfo?.OwnerModInfo?.FolderPath;
            if (string.IsNullOrWhiteSpace(path))
                return;

            _ = Process.Start(path);
        }

        public void OnErrorsButtonClicked()
        {
        }

        private void OnEnable()
        {
            canvasGroup.alpha = 0f;
            if (_initialized)
                Refresh();
        }

        private void Update()
        {
            float alpha = canvasGroup.alpha;
            canvasGroup.alpha = Mathf.Lerp(alpha, 1f, Time.unscaledDeltaTime * 7.5f);
        }

        private void onLoadedImage(Texture2D texture)
        {
            if (!_modImage) return;
            _modImage.texture = texture;

            _modImageObject.SetActive(true);
            _placeholderModImageObject.SetActive(false);
        }

        private void addModDependenciesRecursive(LoadedModInfo loadedModInfo, List<LoadedModInfo> modsList, ref List<LoadedModInfo> sortedList)
        {
            if (sortedList.Contains(loadedModInfo)) return;

            ModInfo modInfo = loadedModInfo.OwnerModInfo;
            if (modInfo.ModDependencies != null && modInfo.ModDependencies.Length != 0)
            {
                foreach (string dependencyId in modInfo.ModDependencies)
                {
                    if (string.IsNullOrEmpty(dependencyId)) continue;

                    LoadedModInfo dependencyInfo = null;
                    foreach (LoadedModInfo lmi in modsList)
                        if (lmi.OwnerModInfo != null && lmi.OwnerModInfo.UniqueID == dependencyId)
                        {
                            dependencyInfo = lmi;
                            break;
                        }

                    if (dependencyInfo == null)
                    {
                        dependencyInfo = new LoadedModInfo(null, new ModInfo()
                        {
                            DisplayName = null,
                            UniqueID = dependencyId
                        });
                    }

                    addModDependenciesRecursive(dependencyInfo, modsList, ref sortedList);
                }
            }

            sortedList.Add(loadedModInfo);
        }

        public class DescriptionHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
        {
            public const float DEFAULT_HEIGHT = 67f;

            private RectTransform m_rectTransform;

            private Text m_textComponent;

            private Outline m_outline;

            private bool m_isMouseIn;

            private int m_siblingIndex;

            public Transform InitialParent;

            public Transform ContentTransform;

            private void Start()
            {
                GameObject gameObject = base.gameObject;
                m_rectTransform = gameObject.transform as RectTransform;
                m_textComponent = gameObject.GetComponentInChildren<Text>();
                m_outline = gameObject.GetComponent<Outline>();
                m_outline.enabled = false;
                m_siblingIndex = gameObject.transform.GetSiblingIndex();
                InitialParent = base.transform.parent;
            }

            public void Refresh()
            {
                bool mouseIn = m_isMouseIn;
                Transform transform = base.transform;

                setHeight(mouseIn ? getTextPreferredHeight() + 5f : DEFAULT_HEIGHT);
                m_outline.enabled = mouseIn;

                transform.SetParent(mouseIn ? ContentTransform : InitialParent);
                if (mouseIn)
                    transform.SetAsLastSibling();
                else
                    transform.SetSiblingIndex(m_siblingIndex);
            }

            private void setHeight(float height)
            {
                Vector2 size = m_rectTransform.sizeDelta;
                size.y = Mathf.Max(height, DEFAULT_HEIGHT);
                m_rectTransform.sizeDelta = size;
            }

            private float getTextPreferredHeight()
            {
                return m_textComponent && m_textComponent.rectTransform ? LayoutUtility.GetPreferredHeight(m_textComponent.rectTransform) : 0f;
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                m_isMouseIn = true;
                Refresh();
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                m_isMouseIn = false;
                Refresh();
            }

            public void OnPointerUp(PointerEventData eventData)
            {
                m_isMouseIn = false;
                Refresh();
            }
        }
    }
}
