using ModLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
	/// <summary>
	/// The mod download page UI
	/// </summary>
	internal class ModDownloadPage : MonoBehaviour
	{
		/// <summary>
		/// The content holder of the download page
		/// </summary>
		public GameObject Content;
		/// <summary>
		/// The close button of the window
		/// </summary>
		public Button XButton;

		/// <summary>
		/// The base object of the page
		/// </summary>
		public GameObject WindowObject;

		public ProgressBar ProgressBar;

		/// <summary>
		/// Sets up the mod download page from a moddedobject
		/// </summary>
		/// <param name="moddedObject"></param>
		public void Init(ModdedObject moddedObject)
		{
			Content = moddedObject.GetObject<GameObject>(0);
			XButton = moddedObject.GetObject<Button>(1);

			GameObject loadingScreen = moddedObject.GetObject<GameObject>(2);
			ProgressBar = loadingScreen.AddComponent<ProgressBar>();
			ProgressBar.Initialize(loadingScreen.GetComponent<ModdedObject>().GetObject<Image>(0));

            WindowObject = moddedObject.gameObject;
		}
	}

}
