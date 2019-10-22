using System;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot as the low level level of the debug console system
    /// </summary>
    public class Logger : Singleton<Logger>
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                Flip();

            if (!Container.activeSelf)
                return;

            if (Input.GetKeyDown(KeyCode.Return))
            {
                RunCommand(InputField.text);
                InputField.text = "";
            }
        }

        internal void Flip()
        {
            if (Container.activeSelf)
            {
                Animator.Play("hideConsole");
                return;
            }

            Animator.Play("showConsole");
        }

        /// <summary>
        /// Writes the specified text to the console
        /// </summary>
        /// <param name="whatToLog"></param>
        public void Log(string whatToLog)
        {
            Text logText = LogText;
            logText.text = logText.text + "\n" + whatToLog;

            Console.WriteLine(whatToLog);
        }

        /// <summary>
        /// Writes the specified text to the console, now in color!
        /// </summary>
        /// <param name="whatToLog"></param>
        /// <param name="color"></param>
        public void Log(string whatToLog, Color color)
        {
            string text = ColorUtility.ToHtmlStringRGB(color);
            Text logText = LogText;
            logText.text = logText.text + "\n<color=#" + text + ">" + whatToLog + "</color>";

            Console.WriteLine(whatToLog);
        }

        /// <summary>
        /// Gets called when the user types in a command into the input field and presses enter
        /// </summary>
        /// <param name="command"></param>
        public void RunCommand(string command)
        {
            try
            {
                ConsoleInputManager.OnCommandRan(command);
                ModsManager.Instance.PassOnMod.OnCommandRan(command);
            }
            catch (Exception ex)
            {
                Log("command '" + command + "' failed with the following error: " + ex.Message, Color.red);
                Log(ex.StackTrace, Color.red);
            }
        }

        /// <summary>
        /// The animator containing the animations for opening and closeing the console
        /// </summary>
        public Animator Animator;

        /// <summary>
        /// The complete text of the console
        /// </summary>
        public Text LogText;

        /// <summary>
        /// The GameObject thats holding the console
        /// </summary>
        public GameObject Container;

        /// <summary>
        /// the input field that commands are typed into
        /// </summary>
        public InputField InputField;
    }
}