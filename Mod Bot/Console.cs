using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Logger.Instance.log(_log);
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
    }
}
