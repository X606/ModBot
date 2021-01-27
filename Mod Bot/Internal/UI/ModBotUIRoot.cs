using ModLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
	public class ModBotUIRoot : Singleton<ModBotUIRoot>
	{
		public ConsoleUI ConsoleUI;
		public FPSCounterUI FPSCounter;
		public ModSuggestingUI ModSuggestingUI;
		public ModBotSignInUI ModBotSignInUI;
		public ModsWindow ModsWindow;
		public Generic2ButtonDialogeUI Generic2ButtonDialogeUI;
		public ModOptionsWindow ModOptionsWindow;
		public ModDownloadPage ModDownloadPage;

		public void Init(ModdedObject moddedObject)
		{
			ConsoleUI = gameObject.AddComponent<ConsoleUI>();
			ConsoleUI.Init(moddedObject.GetObject<Animator>(0), moddedObject.GetObject<Text>(1), moddedObject.GetObject<GameObject>(2), moddedObject.GetObject<InputField>(3));

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

			ModDownloadPage = gameObject.AddComponent<ModDownloadPage>();
			ModDownloadPage.Init(moddedObject.GetObject<ModdedObject>(10));
		}
	}

	public class ModsWindow : MonoBehaviour
	{
		public GameObject Content;
		public Button CloseButton;
		public Button GetMoreModsButton;
		public Button OpenModsFolderButton;
		public GameObject WindowObject;

		public void Init(ModdedObject moddedObject)
		{
			Content = moddedObject.GetObject<GameObject>(0);
			CloseButton = moddedObject.GetObject<Button>(1);
			GetMoreModsButton = moddedObject.GetObject<Button>(2);
			OpenModsFolderButton = moddedObject.GetObject<Button>(3);
			WindowObject = moddedObject.gameObject;
		}
	}

	public class ModOptionsWindow : MonoBehaviour
	{
		public GameObject Content;
		public Button XButton;
		public GameObject PageButtonsHolder;

		public GameObject WindowObject;

		internal void Init(ModdedObject moddedObject)
		{
			Content = moddedObject.GetObject<GameObject>(0);
			XButton = moddedObject.GetObject<Button>(1);
			PageButtonsHolder = moddedObject.GetObject<GameObject>(2);

			WindowObject = moddedObject.gameObject;
		}
	}

	public class ModDownloadPage : MonoBehaviour
	{
		public GameObject Content;
		public Button XButton;

		public GameObject WindowObject;
		public void Init(ModdedObject moddedObject)
		{
			Content = moddedObject.GetObject<GameObject>(0);
			XButton = moddedObject.GetObject<Button>(1);

			WindowObject = moddedObject.gameObject;
		}
	}

	public class Generic2ButtonDialogeUI : MonoBehaviour
	{
		public Text Text;
		public Button Button1;
		public Button Button2;
		public GameObject UIRoot;

		public void Init(ModdedObject moddedObject)
		{
			Text = moddedObject.GetObject<Text>(0);
			Button1 = moddedObject.GetObject<Button>(1);
			Button2 = moddedObject.GetObject<Button>(2);

			UIRoot = moddedObject.gameObject;
		}
	}

}
