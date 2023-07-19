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

        public GameObject ErrorWindow;
        public Text ErrorText;

        public GameObject LoadingPopup;
        public Slider ProgressBarSlider;

        /// <summary>
        /// Sets up the mod download page from a moddedobject
        /// </summary>
        /// <param name="moddedObject"></param>
        public void Init(ModdedObject moddedObject)
        {
            Content = moddedObject.GetObject<GameObject>(0);
            XButton = moddedObject.GetObject<Button>(1);

            LoadingPopup = moddedObject.GetObject_Alt<Transform>(4).gameObject;
            ProgressBarSlider = LoadingPopup.GetComponent<ModdedObject>().GetObject_Alt<Slider>(1);

            ErrorWindow = moddedObject.GetObject_Alt<Transform>(3).gameObject;
            ErrorText = ErrorWindow.GetComponent<ModdedObject>().GetObject_Alt<Text>(1);

            WindowObject = moddedObject.gameObject;
            WindowObject.gameObject.SetActive(false);
        }
    }

}
