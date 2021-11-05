using ModLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    internal class GenericButton : MonoBehaviour
    {
		public Image ButtonImage;
		public Text ButtonTextLabel;
		public Button Button;

		void Awake()
        {
			ModdedObject moddedObject = GetComponent<ModdedObject>();

			ButtonImage = moddedObject.GetObject<Image>(0);
			Button = moddedObject.GetObject<Button>(1);
			ButtonTextLabel = moddedObject.GetObject<Text>(2);
		}
    }
}
