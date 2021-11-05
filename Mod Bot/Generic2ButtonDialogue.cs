using System;
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
    [Obsolete("Use the SimpleMessageBox class instead")]
    public class Generic2ButtonDialogue
    {
        private Generic2ButtonDialogue() // to make sure that you cant create one of these without arguments
        {
        }

        /// <summary>
        /// If this is <see langword="true"/> there is currently a <see cref="Generic2ButtonDialogue"/> open
        /// </summary>
        public static bool IsWindowOpen => SimpleMessageBox.IsAnyWindowActive();

        SimpleMessageBox _messageBox;

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
            _messageBox = new SimpleMessageBox(message);
            _messageBox.AddButton(new MessageBoxButton(button1Text, onPressButton1));
            _messageBox.AddButton(new MessageBoxButton(button2Text, onPressButton2));
            _messageBox.QueueMessage();
        }

        /// <summary>
        /// Sets the color of the first button
        /// </summary>
        /// <param name="color">The color to set the button to</param>
        public void SetColorOfFirstButton(Color color)
        {
            _messageBox.SetButtonColor(0, color);
        }
        /// <summary>
        /// Sets the color of the second button
        /// </summary>
        /// <param name="color">The color to set the button to</param>
        public void SetColorOfSecondButton(Color color)
        {
            _messageBox.SetButtonColor(1, color);
        }

        /// <summary>
        /// Closes the window
        /// </summary>
        public void Close()
        {
            _messageBox.Close();
        }
    }
}
