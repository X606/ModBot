using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        /// <summary>
        /// Writes to the in-game console.
        /// </summary>
        /// <param name="_log"></param>
        public static void Log(string _log)
        {
            //Console.WriteLine(_log);
            if (Logger.Instance.LogText.text.Length > 200) {
                string newText = Logger.Instance.LogText.text.Substring(Logger.Instance.LogText.text.Length - 200);
                Logger.Instance.LogText.text = newText;
            }

            Logger.Instance.log(_log);
            
        }


        public static void Log(object _log)
        {
            //Console.WriteLine(_log);
            Logger.Instance.log(_log.ToString());
            
        }

        /// <summary>
        /// Writes to the in-game console, in color.
        /// </summary>
        /// <param name="_log"></param>
        /// <param name="_color"></param>
        public static void Log(string _log, Color _color)
        {
            Logger.Instance.log(_log, _color);
        }

        /// <summary>
        /// Passes every instance of the given list's 'ToString()' value to: 'debug.Log()'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void PrintAll<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Logger.Instance.log(list[i].ToString());
            }
        }

        /// <summary>
        /// Passes every instance of the given list's 'ToString()' value to: 'debug.Log()'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void PrintAll<T>(List<T> list, Color _color)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Logger.Instance.log(list[i].ToString(), _color);
            }
        }


        public static void PrintAllChildren(Transform obj)
        {
            outputText = "";
            WriteToFile(obj.ToString());
            RecursivePrintAllChildren("   ", obj);

            File.WriteAllText(Application.persistentDataPath + "/debug.txt",outputText);
            System.Diagnostics.Process.Start("notepad.exe", Application.persistentDataPath + "/debug.txt");

        }
        static void RecursivePrintAllChildren(string pre, Transform obj)
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
        static void WriteToFile(string msg)
        {
            outputText += msg + "\n";
        }
        static string outputText;
    }
}
