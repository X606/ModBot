using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using InternalModBot;

namespace ModLibrary
{
    /*
     * 
     * NOTE: the reason why this class isnt called "Debug" is becuase there is already a class called that in UnityEngine and that would cause a 
     * lot problems for people making mods (most people will use both this namespace and UnityEngine)
     * 
    */

    /// <summary>
    /// Allows you to write to the in-game console (open it with F1).
    /// </summary>
    public static class debug
    {
        private const int CONSOLE_CHARACTER_LIMIT = 1000;

        /// <summary>
        /// Writes to the in-game console.
        /// </summary>
        /// <param name="_log">What to write</param>
        public static void Log(string _log)
        {
            InternalModBot.Logger.Instance.Log(_log);

            if (InternalModBot.Logger.Instance.LogText.text.Length > CONSOLE_CHARACTER_LIMIT)
            {
                string newText = InternalModBot.Logger.Instance.LogText.text.Substring(InternalModBot.Logger.Instance.LogText.text.Length - CONSOLE_CHARACTER_LIMIT);
                InternalModBot.Logger.Instance.LogText.text = newText;
            }
        }

        /// <summary>
        /// Writes the given object's ToString() value to the console
        /// </summary>
        /// <param name="_log">The object to write</param>
        public static void Log(object _log)
        {
            Log(_log.ToString());
        }

        /// <summary>
        /// Writes to the in-game console, in color.
        /// </summary>
        /// <param name="_log">What to write</param>
        /// <param name="_color">The color to write in</param>
        public static void Log(string _log, Color _color)
        {
            InternalModBot.Logger.Instance.Log(_log, _color);

            if (InternalModBot.Logger.Instance.LogText.text.Length > CONSOLE_CHARACTER_LIMIT)
            {
                string newText = InternalModBot.Logger.Instance.LogText.text.Substring(InternalModBot.Logger.Instance.LogText.text.Length - CONSOLE_CHARACTER_LIMIT);
                InternalModBot.Logger.Instance.LogText.text = newText;
            }
        }

        /// <summary>
        /// Passes every instance of the given list's 'ToString()' value to 'debug.Log()'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void PrintAll<T>(IEnumerable<T> list)
        {
            foreach (T item in list)
            {
                Log(item.ToString());
            }
        }

        /// <summary>
        /// Passes every instance of the given list's 'ToString()' value to: 'debug.Log()'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="color"></param>
        public static void PrintAll<T>(IEnumerable<T> list, Color color)
        {
            foreach (T item in list)
            {
                Log(item.ToString(), color);
            }
        }

        /// <summary>
        /// Opens a notepad window with info about the passed transfrom like components and children
        /// </summary>
        /// <param name="obj"></param>
        public static void PrintAllChildren(Transform obj) // TODO : Rewrite this function
        {
            outputText = "";
            WriteToFile(obj.ToString());
            RecursivePrintAllChildren("   ", obj);

            File.WriteAllText(Application.persistentDataPath + "/debug.txt", outputText);
            Process.Start("notepad.exe", Application.persistentDataPath + "/debug.txt");
        }

        private static void RecursivePrintAllChildren(string pre, Transform obj)
        {
            Component[] components = obj.gameObject.GetComponents(typeof(Component));

            if (components.Length != 0)
            {
                WriteToFile(pre + "Components: ");
            }

            for (int i = 0; i < components.Length; i++)
            {
                WriteToFile(pre + components[i].ToString());
            }

            if (obj.childCount != 0)
            {
                WriteToFile(pre + "Children: ");
            }

            for (int i = 0; i < obj.childCount; i++)
            {
                Transform child = obj.GetChild(i);
                WriteToFile(pre + i + ": " + child.name);
                RecursivePrintAllChildren(pre + "   ", child);
            }
        }

        private static void WriteToFile(string msg)
        {
            outputText += msg + "\n";
        }

        private static string outputText;
    }
}

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot as the low level level of the debug console system
    /// </summary>
    public class Logger : Singleton<Logger>
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Flip();
            }
            if (!Container.activeSelf)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                RunCommand(input.text);
                input.text = "";
            }
        }

        private void Flip()
        {
            if (Container.activeSelf)
            {
                animator.Play("hideConsole");
                return;
            }
            animator.Play("showConsole");
        }

        /// <summary>
        /// Writes the specified text to the console
        /// </summary>
        /// <param name="whatToLog"></param>
        public void Log(string whatToLog)
        {
            Text logText = LogText;
            logText.text = logText.text + "\n" + whatToLog;
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
        public Animator animator;
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
        public InputField input;
    }
}