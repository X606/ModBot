using ModLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
	/// <summary>
	/// The mods window UI
	/// </summary>
	internal class ModsWindow : MonoBehaviour
	{
		/// <summary>
		/// The holder for the content of the window
		/// </summary>
		public GameObject Content;
		/// <summary>
		/// The close button
		/// </summary>
		public Button CloseButton;
		/// <summary>
		/// The get more mods button
		/// </summary>
		public Button GetMoreModsButton;
		/// <summary>
		/// The open mods folder button
		/// </summary>
		public Button OpenModsFolderButton;
		/// <summary>
		/// The base window object
		/// </summary>
		public GameObject WindowObject;

		/// <summary>
		/// Sets up the Mods window UI from a modded object
		/// </summary>
		/// <param name="moddedObject"></param>
		public void Init(ModdedObject moddedObject)
		{
			Content = moddedObject.GetObject<GameObject>(0);
			CloseButton = moddedObject.GetObject<Button>(1);
			GetMoreModsButton = moddedObject.GetObject<Button>(2);
			OpenModsFolderButton = moddedObject.GetObject<Button>(3);
			WindowObject = moddedObject.gameObject;
		}
	}

}
