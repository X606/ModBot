using System;
using System.Collections.Generic;
using UnityEngine;
using InternalModBot;

#pragma warning disable IDE1005

namespace ModLibrary
{
    /// <summary>
    /// Represents a generic message box popup, with any number of options
    /// </summary>
    public class SimpleMessageBox
    {
        static GenericMessageBoxUI MessageBoxUI => ModBotUIRoot.Instance.GenericMessageBoxUI;

        static bool _isAnyWindowOpen;
        static SimpleMessageBox _activeWindow;

        static Queue<SimpleMessageBox> _queuedMessages = new Queue<SimpleMessageBox>();

        internal string Message;
        internal List<MessageBoxButton> Buttons;

        /// <summary>
        /// Initializes an instance of a <see cref="SimpleMessageBox"/> with a message
        /// </summary>
        /// <param name="message">The message to display</param>
        public SimpleMessageBox(string message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));
            
            Message = message;
            Buttons = new List<MessageBoxButton>();
        }

        /// <summary>
        /// Adds a button option to this instance
        /// </summary>
        /// <param name="button">The button to add</param>
        public void AddButton(MessageBoxButton button)
        {
            if (button is null)
                throw new ArgumentNullException(nameof(button));

            // Make a copy of the button instance so the same instance can be used for several message boxes
            MessageBoxButton buttonCopy = new MessageBoxButton(button);

            // Always hide this menu when the button is pressed
            buttonCopy.PressedCallback += hide;
            Buttons.Add(buttonCopy);
        }

        void hide()
        {
            if (_queuedMessages.Count > 0)
            {
                SimpleMessageBox queuedMessageBox = _queuedMessages.Dequeue();
                queuedMessageBox.show();
            }
            else
            {
                _isAnyWindowOpen = false;
                _activeWindow = null;

                MessageBoxUI.Hide();
            }
        }

        void show()
        {
            _isAnyWindowOpen = true;
            _activeWindow = this;

            MessageBoxUI.Show(this);
        }

        /// <summary>
        /// Queue this message to appear after all the current messages have been displayed, if no messages are active, this message will be displayed instantly
        /// </summary>
        public void QueueMessage()
        {
            if (!_isAnyWindowOpen)
            {
                DisplayMessageNow();
            }
            else
            {
                _queuedMessages.Enqueue(this);
            }
        }

        /// <summary>
        /// Force this instance to display immediately, any currently active message box is added back into the queue
        /// </summary>
        public void DisplayMessageNow()
        {
            if (_isAnyWindowOpen)
            {
                _activeWindow.hide();
                _queuedMessages.Enqueue(_activeWindow);
            }

            show();
        }

        [Obsolete("Remove when Generic2ButtonDialogue is completely removed")]
        internal static bool IsAnyWindowActive()
        {
            return _isAnyWindowOpen;
        }

        [Obsolete("Remove when Generic2ButtonDialogue is completely removed")]
        internal void SetButtonColor(int buttonIndex, Color color)
        {
            if (buttonIndex >= 0 && buttonIndex < Buttons.Count)
            {
                Buttons[buttonIndex].ButtonColor = color;
                MessageBoxUI.Show(this); // Refresh message box
            }
        }

        [Obsolete("Remove when Generic2ButtonDialogue is completely removed")]
        internal void Close()
        {
            hide();
        }
    }
}
