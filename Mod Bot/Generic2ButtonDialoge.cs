using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#pragma warning disable IDE1005

namespace ModLibrary
{
    /// <summary>
    /// Used to bring up a dialoge with 2 buttons
    /// </summary>
    public class Generic2ButtonDialogue
    {
        private readonly Action OnButton1ClickedCallback;
        private readonly Action OnButton2ClickedCallback;

        private Text DisplayText;
        private Button Button1;
        private Button Button2;

        private ModdedObject SpawnedObject;

        /// <summary>
        /// If this is <see langword="true"/> there is currently a <see cref="Generic2ButtonDialogue"/> open
        /// </summary>
        public static bool IsWindowOpen { get; private set; }

        /// <summary>
        /// Creates a dialoge where the user can select one of 2 options
        /// </summary>
        /// <param name="message">The text that will be displayed on screen</param>
        /// <param name="button1Text">The text on the first button</param>
        /// <param name="onPressButton1">When the first button is pressed, this will be called, then the window will be closed, if <see langword="null"/>, it will just close the window</param>
        /// <param name="button2Text">The text on the second button</param>
        /// <param name="onPressButton2">When the first button is pressed, this will be called, then the window will be closed, if <see langword="null"/>, it will just close the window</param>
        public Generic2ButtonDialogue(string message, string button1Text, Action onPressButton1, string button2Text, Action onPressButton2) 
        {
            GameObject prefab = AssetLoader.GetObjectFromFile<GameObject>("modswindow", "Generic2ButtonDialoge", "Clone Drone in the Danger Zone_Data/");
            SpawnedObject = GameObject.Instantiate(prefab).GetComponent<ModdedObject>();

            DisplayText = SpawnedObject.GetObject<Text>(0);
            Button1 = SpawnedObject.GetObject<Button>(1);
            Button2 = SpawnedObject.GetObject<Button>(2);

            Button1.GetComponentInChildren<Text>().text = button1Text;
            Button2.GetComponentInChildren<Text>().text = button2Text;

            DisplayText.text = message;
            Button1.onClick.AddListener(OnButton1Clicked);
            Button2.onClick.AddListener(OnButton2Clicked);

            OnButton1ClickedCallback = onPressButton1;
            OnButton2ClickedCallback = onPressButton2;

            IsWindowOpen = true;
        }

        /// <summary>
        /// Sets the color of the first button
        /// </summary>
        /// <param name="color">The color to set the button to</param>
        public void SetColorOfFirstButton(Color color)
        {
            Button1.GetComponent<Image>().color = color;
        }
        /// <summary>
        /// Sets the color of the second button
        /// </summary>
        /// <param name="color">The color to set the button to</param>
        public void SetColorOfSecondButton(Color color)
        {
            Button2.GetComponent<Image>().color = color;
        }

        /// <summary>
        /// Closes the window
        /// </summary>
        public void Close()
        {
            GameObject.Destroy(SpawnedObject.gameObject);
            IsWindowOpen = false;
        }

        private void OnButton1Clicked()
        {
            if (OnButton1ClickedCallback != null)
            {
                OnButton1ClickedCallback();
            }

            Close();
        }
        private void OnButton2Clicked()
        {
            if (OnButton2ClickedCallback != null)
            {
                OnButton2ClickedCallback();
            }

            Close();
        }


        private Generic2ButtonDialogue() // to make sure that you cant create one of these without arguments
        {
        }

    }
}
