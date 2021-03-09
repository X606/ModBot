using ModLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
	/// <summary>
	/// The mod options window UI
	/// </summary>
	internal class ModOptionsWindow : MonoBehaviour
	{
		/// <summary>
		/// The actual content holder of the window
		/// </summary>
		public GameObject Content;
		/// <summary>
		/// The close button of the window
		/// </summary>
		public Button XButton;
		/// <summary>
		/// The page buttons holder
		/// </summary>
		public GameObject PageButtonsHolder;


		/// <summary>
		/// The actual window object
		/// </summary>
		public GameObject WindowObject;

		internal void Init(ModdedObject moddedObject)
		{
			Content = moddedObject.GetObject<GameObject>(0);
			XButton = moddedObject.GetObject<Button>(1);
			PageButtonsHolder = moddedObject.GetObject<GameObject>(2);

			WindowObject = moddedObject.gameObject;
		}
	}

}
