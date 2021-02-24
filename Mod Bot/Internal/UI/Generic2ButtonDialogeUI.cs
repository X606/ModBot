using ModLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// The generic 2 button dialoge window Ui
    /// </summary>
    public class Generic2ButtonDialogeUI : MonoBehaviour
	{
		/// <summary>
		/// The text element in the UI
		/// </summary>
		public Text Text;
		/// <summary>
		/// The first button in the UI
		/// </summary>
		public Button Button1;
		/// <summary>
		/// The second button in the UI
		/// </summary>
		public Button Button2;
		/// <summary>
		/// The actual root of the window
		/// </summary>
		public GameObject UIRoot;


		/// <summary>
		/// Sets up the generic 2 button dialoge from a modded object
		/// </summary>
		/// <param name="moddedObject"></param>
		public void Init(ModdedObject moddedObject)
		{
			Text = moddedObject.GetObject<Text>(0);
			Button1 = moddedObject.GetObject<Button>(1);
			Button2 = moddedObject.GetObject<Button>(2);

			UIRoot = moddedObject.gameObject;
		}
	}

}
