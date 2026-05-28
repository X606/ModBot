using ModLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// The UI root for all mod-bot UI
    /// </summary>
    internal class ModBotUIRoot : Singleton<ModBotUIRoot>
    {
        /// <summary>
        /// The Console UI
        /// </summary>
        public ConsoleUI ConsoleUI;
        /// <summary>
        /// The FPS counter in the corner
        /// </summary>
        public FPSCounterUI FPSCounter;
        /// <summary>
        /// The mod suggesting UI
        /// </summary>
        public ModSuggestingUI ModSuggestingUI;
        /// <summary>
        /// The modbot sign in UI
        /// </summary>
        public ModBotSignInUI ModBotSignInUI;
        /// <summary>
        /// The mods list window UI
        /// </summary>
        public ModListWindow ModList;
        /// <summary>
        /// The generic 2 Button dialoge UI
        /// </summary>
        public Generic2ButtonDialogueUI Generic2ButtonDialogueUI;
        /// <summary>
        /// The mod options window UI
        /// </summary>
        public ModOptionsWindow ModOptionsWindow;
        /// <summary>
        /// The mods download UI
        /// </summary>
        public ModDownloadWindow DownloadWindow;
        /// <summary>
        /// The loading bar
        /// </summary>
        public GenericLoadingBar LoadingBar;
        /// <summary>
        /// The root canvas
        /// </summary>
        public Canvas Root;

        private bool _initialized;

        private void Update()
        {
            if (!_initialized) return;

            if (Input.GetKeyDown(ModBotPrefs.GetKeyCode(ModBotInputType.OpenConsole)))
                ConsoleUI.Flip();
        }

        /// <summary>
        /// Sets up the mod-bot UI
        /// </summary>
        public void Init()
        {
            Root = GetComponent<Canvas>();

            ModdedObject moddedObject = GetComponent<ModdedObject>();

            ConsoleUI = moddedObject.GetObject<GameObject>(0).AddComponent<ConsoleUI>();
            ConsoleUI.Init();

            FPSCounter = gameObject.AddComponent<FPSCounterUI>();
            FPSCounter.Init(moddedObject.GetObject<Text>(1));

            Generic2ButtonDialogueUI = gameObject.AddComponent<Generic2ButtonDialogueUI>();
            Generic2ButtonDialogueUI.Init(moddedObject.GetObject<ModdedObject>(2));

            ModList = moddedObject.GetObject<GameObject>(3).AddComponent<ModListWindow>();
            ModList.Init();

            ModOptionsWindow = gameObject.AddComponent<ModOptionsWindow>();
            ModOptionsWindow.Init(moddedObject.GetObject<ModdedObject>(4));

            DownloadWindow = moddedObject.GetObject<GameObject>(5).AddComponent<ModDownloadWindow>();
            DownloadWindow.Init();

            ModBotSignInUI = gameObject.AddComponent<ModBotSignInUI>();
            ModBotSignInUI.Init(moddedObject.GetObject<ModdedObject>(6));

            ModSuggestingUI = gameObject.AddComponent<ModSuggestingUI>();
            ModSuggestingUI.Init(moddedObject.GetObject<ModdedObject>(7));

            LoadingBar = moddedObject.GetObject<GameObject>(8).AddComponent<GenericLoadingBar>();
            LoadingBar.Init();

            _initialized = true;
        }

        public bool AreAnyMenusOpen()
        {
            return _initialized && (Generic2ButtonDialogue.IsWindowOpen ||
                ModList.gameObject.activeInHierarchy ||
                ModOptionsWindow.WindowObject.activeInHierarchy ||
                DownloadWindow.gameObject.activeInHierarchy ||
                ModBotSignInUI.WindowObject.activeInHierarchy);
        }

        public bool CloseCurrentMenu()
        {
            if (!_initialized) return false;

            if (Generic2ButtonDialogueUI.UIRoot.activeInHierarchy) // block closing other menus if dialogue is active 
            {
                return true;
            }

            if (ModBotSignInUI.WindowObject.activeInHierarchy)
            {
                ModBotSignInUI.WindowObject.SetActive(false);
                return true;
            }

            if (DownloadWindow.gameObject.activeInHierarchy)
            {
                if (DownloadWindow.IsInformationWindowActive())
                {
                    DownloadWindow.CloseInformationWindow();
                    return true;
                }
                DownloadWindow.Hide();
                return true;
            }

            if (ModOptionsWindow.WindowObject.activeInHierarchy)
            {
                if (ModOptionsWindow.Builder != null)
                {
                    ModOptionsWindow.Builder.CloseWindow();
                    return true;
                }

                ModOptionsWindow.WindowObject.gameObject.SetActive(false);
                GameUIRoot.Instance.SetEscMenuDisabled(false);
                return true;
            }

            if (ModList.gameObject.activeInHierarchy)
            {
                ModList.Hide();
                return true;
            }
            return false;
        }
    }
}