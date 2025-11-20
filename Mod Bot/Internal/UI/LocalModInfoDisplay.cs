using ModLibrary;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InternalModBot
{
    internal class LocalModInfoDisplay : MonoBehaviour
    {
        public const string BG_ENABLED_COLOR = "#373C43";
        public const string BG_DISABLED_COLOR = "#740900";

        public const string TOGGLE_ENABLED_COLOR = "#FF6666";
        public const string TOGGLE_DISABLED_COLOR = "#34FF37";

        public const string AUTHOR_COLOR = "#5ABAFF";
        public const string VERSION_COLOR = "#FF9500";

        private Button _toggleButton;
        private Button _optionsButton;
        private Button _errorsButton;
        private Button _shareButton;

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

            canvasGroup = base.GetComponent<CanvasGroup>();
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            moddedObject.GetObject<Text>(0).text = modInfo.OwnerModInfo.DisplayName;
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
            bool exception = false;//!string.IsNullOrEmpty(loadedModInfo.OwnerModInfo?.Exception);
            bool isEnabled = loadedModInfo.IsEnabled;
            bool isMultiplayer = GameModeManager.IsMultiplayer();
            bool hasSettings = isEnabled && modInfo.ModReference != null && modInfo.ModReference.ImplementsSettingsWindow();

            SetColor(base.transform, isEnabled ? BG_ENABLED_COLOR : BG_DISABLED_COLOR);
            SetColor(_toggleButton.transform, isEnabled ? TOGGLE_ENABLED_COLOR : TOGGLE_DISABLED_COLOR);
            SetColor(_descriptionHolderTransform, isEnabled ? BG_ENABLED_COLOR : BG_DISABLED_COLOR);

            _optionsButtonText.color = hasSettings ? Color.white : Color.gray;

            //_optionsButton.gameObject.SetActive(isEnabled);
            _optionsButton.interactable = hasSettings;
            _descriptionHolderObject.SetActive(!exception);
            _errorsButton.gameObject.SetActive(exception);
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

        public static void SetColor(Transform transform, string hexColor)
        {
            ColorUtility.TryParseHtmlString(hexColor, out Color color);
            Graphic graphic = transform.GetComponent<Graphic>();
            if (graphic)
            {
                graphic.color = color;
            }
        }

        public void OnToggleButtonClicked()
        {
            if (loadedModInfo == null)
                return;

            try // ensure that the mod list will still work properly if the mod is enabled/disabled with an error
            {
                loadedModInfo.IsEnabled = !loadedModInfo.IsEnabled;
            }
            catch (System.Exception exc)
            {
                UnityEngine.Debug.LogException(exc);
            }
            finally
            {
                Refresh();
                ModBotUIRoot.Instance.ModList.RefreshModsInfoLabel();
            }
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
            /*_ = new Generic2ButtonDialogue("Mod errors:\n" + loadedModInfo.OwnerModInfo.Exception,
                "Ok",
                null,
                "Disable mod",
                delegate
                {
                    loadedModInfo.IsEnabled = false;
                    Refresh();
                },
                Generic2ButtonDialogeUI.ModErrorSizeDelta);*/
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
