using System;
using UnityEngine;

#pragma warning disable IDE1005

namespace ModLibrary
{
    /// <summary>
    /// Represents a generic message box button
    /// </summary>
    public class MessageBoxButton
    {
        /// <summary>
        /// The default <see cref="Color"/> of buttons
        /// </summary>
        public static readonly Color DefaultColor = new Color(1f, 0.3f, 0.3f);

        internal string ButtonText;
        internal Color ButtonColor;
        internal Action PressedCallback;

        /// <summary>
        /// Initializes an instance of a <see cref="MessageBoxButton"/> with a text, color, and pressed callback
        /// </summary>
        /// <param name="buttonText">The text to display on the button</param>
        /// <param name="buttonColor">The <see cref="Color"/> of the button</param>
        /// <param name="pressedCallback">The callback to invoke when this button is pressed</param>
        public MessageBoxButton(string buttonText, Color buttonColor, Action pressedCallback)
        {
            if (buttonText == null)
                throw new ArgumentNullException(nameof(buttonText));
            
            ButtonText = buttonText;
            ButtonColor = buttonColor;
            PressedCallback = delegate { pressedCallback?.Invoke(); }; // If pressedCallback is null, the PressedCallback field should still not be null
        }

        /// <summary>
        /// Initializes an instance of a <see cref="MessageBoxButton"/> with a text, and pressed callback
        /// </summary>
        /// <param name="buttonText">The text to display on the button</param>
        /// <param name="pressedCallback">The callback to invoke when this button is pressed</param>
        public MessageBoxButton(string buttonText, Action pressedCallback) : this(buttonText, DefaultColor, pressedCallback)
        {
        }

        /// <summary>
        /// Initializes an instance of a <see cref="MessageBoxButton"/> with a text, and color
        /// </summary>
        /// <param name="buttonText">The text to display on the button</param>
        /// <param name="buttonColor">The <see cref="Color"/> of the button</param>
        public MessageBoxButton(string buttonText, Color buttonColor) : this(buttonText, buttonColor, null)
        {
        }

        /// <summary>
        /// Initializes an instance of a <see cref="MessageBoxButton"/> with a text
        /// </summary>
        /// <param name="buttonText">The text to display on the button</param>
        public MessageBoxButton(string buttonText) : this(buttonText, DefaultColor, null)
        {
        }

        /// <summary>
        /// Copies one <see cref="MessageBoxButton"/> into a new instance
        /// </summary>
        /// <param name="otherButton">The <see cref="MessageBoxButton"/> to clone</param>
        public MessageBoxButton(MessageBoxButton otherButton) : this(otherButton.ButtonText, otherButton.ButtonColor, otherButton.PressedCallback)
        {
        }

        internal void OnPressed()
        {
            PressedCallback.Invoke();
        }

        internal MessageBoxButton Clone()
        {
            return new MessageBoxButton(ButtonText, ButtonColor, PressedCallback);
        }
    }
}
