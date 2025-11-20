using ModLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		/// The mods window UI
		/// </summary>
		public ModsWindow ModsWindow;
		/// <summary>
		/// The generic 2 Button dialoge UI
		/// </summary>
		public Generic2ButtonDialogeUI Generic2ButtonDialogeUI;
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

        /// <summary>
        /// Sets up the mod-bot UI
        /// </summary>
        public void Init()
        {
			ModdedObject moddedObject = base.GetComponent<ModdedObject>();

            Root = moddedObject.GetComponent<Canvas>();

			ConsoleUI = gameObject.AddComponent<ConsoleUI>();
			ConsoleUI.Init(moddedObject.GetObject<Animator>(0), moddedObject.GetObject<GameObject>(1), moddedObject.GetObject<GameObject>(2), moddedObject.GetObject<InputField>(3));

			FPSCounter = gameObject.AddComponent<FPSCounterUI>();
			FPSCounter.Init(moddedObject.GetObject<Text>(4));

			ModSuggestingUI = gameObject.AddComponent<ModSuggestingUI>();
			ModSuggestingUI.Init(moddedObject.GetObject<ModdedObject>(5));

			ModBotSignInUI = gameObject.AddComponent<ModBotSignInUI>();
			ModBotSignInUI.Init(moddedObject.GetObject<ModdedObject>(6));

			ModsWindow = gameObject.AddComponent<ModsWindow>();
			ModsWindow.Init(moddedObject.GetObject<ModdedObject>(7));

			Generic2ButtonDialogeUI = gameObject.AddComponent<Generic2ButtonDialogeUI>();
			Generic2ButtonDialogeUI.Init(moddedObject.GetObject<ModdedObject>(8));

			ModOptionsWindow = gameObject.AddComponent<ModOptionsWindow>();
			ModOptionsWindow.Init(moddedObject.GetObject<ModdedObject>(9));

            DownloadWindow = moddedObject.GetObject<ModdedObject>(10).gameObject.AddComponent<ModDownloadWindow>();
            DownloadWindow.Init();

            LoadingBar = moddedObject.GetObject<ModdedObject>(11).gameObject.AddComponent<GenericLoadingBar>();
            LoadingBar.Init();
        }
	}
}
