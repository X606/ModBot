using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using InternalModBot;

#pragma warning disable IDE1005

namespace ModLibrary
{
    /// <summary>
    /// Used to bring up a dialoge with 2 buttons
    /// </summary>
    public class Generic2ButtonDialogue
    {
        private Generic2ButtonDialogue() // to make sure that you cant create one of these without arguments
        {
        }

        Action _onButton1ClickedCallback;
        Action _onButton2ClickedCallback;

        Text _displayText;
        Button _button1;
        Button _button2;

        ModdedObject _spawnedObject;

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
            _spawnedObject = InternalAssetBundleReferences.ModsWindow.InstantiateObject("Generic2ButtonDialoge").GetComponent<ModdedObject>();

            _displayText = _spawnedObject.GetObject<Text>(0);
            _button1 = _spawnedObject.GetObject<Button>(1);
            _button2 = _spawnedObject.GetObject<Button>(2);

            _button1.GetComponentInChildren<Text>().text = button1Text;
            _button2.GetComponentInChildren<Text>().text = button2Text;

            _displayText.text = message;
            _button1.onClick.AddListener(onButton1Clicked);
            _button2.onClick.AddListener(onButton2Clicked);

            _onButton1ClickedCallback = onPressButton1;
            _onButton2ClickedCallback = onPressButton2;

            IsWindowOpen = true;
        }

        /// <summary>
        /// Sets the color of the first button
        /// </summary>
        /// <param name="color">The color to set the button to</param>
        public void SetColorOfFirstButton(Color color)
        {
            _button1.GetComponent<Image>().color = color;
        }
        /// <summary>
        /// Sets the color of the second button
        /// </summary>
        /// <param name="color">The color to set the button to</param>
        public void SetColorOfSecondButton(Color color)
        {
            _button2.GetComponent<Image>().color = color;
        }

        /// <summary>
        /// Closes the window
        /// </summary>
        public void Close()
        {
            GameObject.Destroy(_spawnedObject.gameObject);
            IsWindowOpen = false;
        }

        void onButton1Clicked()
        {
            if (_onButton1ClickedCallback != null)
            {
                _onButton1ClickedCallback();
            }

            Close();
        }

        void onButton2Clicked()
        {
            if (_onButton2ClickedCallback != null)
            {
                _onButton2ClickedCallback();
            }

            Close();
        }

    }
}
