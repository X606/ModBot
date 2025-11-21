using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using InternalModBot;
using System;
using System.Text;

namespace ModLibrary
{
    /*
     * 
     * NOTE: the reason why this class isnt called "Debug" is becuase there is already a class called that in UnityEngine and that would cause a 
     * lot problems for people making mods (most people will use both this namespace and UnityEngine)
     * 
    */
#pragma warning disable IDE1006 // Disables the warning about the lower case name

    /// <summary>
    /// Allows you to write to the in-game console (open it with F1).
    /// </summary>
    public static class debug
    {
        /// <summary>
        /// Writes to the in-game console.
        /// </summary>
        /// <param name="_log">What to write</param>
        public static void Log(string _log)
        {
            if (_log == null)
                _log = "null";

            if (ModBotUIRoot.Instance != null && ModBotUIRoot.Instance.ConsoleUI != null)
            {
                ModBotUIRoot.Instance.ConsoleUI.Log(_log);
            }
            else
            {
                Console.WriteLine(_log);
            }
        }

        /// <summary>
        /// Writes the given object's <see cref="object.ToString"/> value to the console
        /// </summary>
        /// <param name="_log">The object to write</param>
        public static void Log(object _log)
        {
            if (_log == null)
                _log = "null";

            Log(_log.ToString());
        }

        /// <summary>
        /// Writes the given object's <see cref="object.ToString"/> value to the console with the specified <see cref="Color"/>
        /// </summary>
        /// <param name="_log">The <see cref="object"/> to log</param>
        /// <param name="color">The <see cref="Color"/> to write in</param>
        public static void Log(object _log, Color color)
        {
            if (_log == null)
                _log = "null";

            Log(_log.ToString(), color);
        }

        /// <summary>
        /// Writes to the in-game console, in color.
        /// </summary>
        /// <param name="_log">What to write</param>
        /// <param name="_color">The <see cref="Color"/> to write in</param>
        public static void Log(string _log, Color _color)
        {
            if (_log == null)
                _log = "null";

            if (ModBotUIRoot.Instance != null && ModBotUIRoot.Instance.ConsoleUI != null)
            {
                ModBotUIRoot.Instance.ConsoleUI.Log(_log, _color);
            }
            else
            {
                Console.WriteLine(_log);
            }
        }

        /// <summary>
        /// Passes every instance of the given <see cref="IEnumerable{T}"/>s <see cref="object.ToString"/> value to <see cref="Log(string)"/>
        /// </summary>
        /// <typeparam name="T">The type of the collection to write</typeparam>
        /// <param name="list">The <see cref="IEnumerable{T}"/> to write</param>
        public static void PrintAll<T>(IEnumerable<T> list)
        {
            foreach (T item in list)
            {
                Log(item.ToString());
            }
        }

        /// <summary>
        /// Passes every instance of the given <see cref="IEnumerable{T}"/>s <see cref="object.ToString"/> value to <see cref="debug.Log(string)"/>
        /// </summary>
        /// <typeparam name="T">The type of the collection to write</typeparam>
        /// <param name="list">The <see cref="IEnumerable{T}"/> to write</param>
        /// <param name="color">The <see cref="Color"/> to write in</param>
        public static void PrintAll<T>(IEnumerable<T> list, Color color)
        {
            foreach (T item in list)
            {
                Log(item.ToString(), color);
            }
        }

        /// <summary>
        /// Draws a line from one point to another in a specified color
        /// </summary>
        /// <param name="point1">Point to draw from</param>
        /// <param name="point2">Point to draw to</param>
        /// <param name="color">The color to draw in</param>
        /// <param name="timeToStay">The amount of unscaledTime in seconds to render the line</param>
        public static void DrawLine(Vector3 point1, Vector3 point2, Color color, float timeToStay = 0f)
        {
            DebugLineDrawingManager.Instance.AddLine(new LineInfo(point1, point2, color, Time.unscaledTime + timeToStay));
        }

        /// <summary>
        /// Draws a ray from a point in a direction. The ray will always have a length of 1000 units
        /// </summary>
        /// <param name="point">The point to draw from</param>
        /// <param name="direction">The direction to draw in</param>
        /// <param name="color">The color to draw in</param>
        /// <param name="timeToStay">The amount of unscaledTime in seconds to render the line</param>
        public static void DrawRay(Vector3 point, Vector3 direction, Color color, float timeToStay = 0f)
        {
            Vector3 otherPoint = point + (direction.normalized * 1000f);
            DebugLineDrawingManager.Instance.AddLine(new LineInfo(point, otherPoint, color, Time.unscaledTime + timeToStay));
        }

        /// <summary>
        /// Opens a notepad window with info about the passed transfrom like components and children
        /// </summary>
        /// <param name="obj"></param>
        public static void PrintAllChildren(Transform obj)
        {
            if (!obj) throw new ArgumentException($"{nameof(obj)} is destroyed or null");

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(obj.name);
            recursivePrintAllChildren(ref stringBuilder, string.Empty, obj);

            string path = Path.Combine(Application.persistentDataPath, "debug.txt");

            File.WriteAllText(path, stringBuilder.ToString());
            Process.Start(path);
        }

        static void recursivePrintAllChildren(ref StringBuilder stringBuilder, string prefix, Transform obj)
        {
            stringBuilder.Append(prefix);
            Component[] components = obj.GetComponents(typeof(Component));
            if (components != null && components.Length > 1)
            {
                stringBuilder.AppendLine($"{components.Length - 1} components: ");

                for (int i = 1; i < components.Length; i++) // skip transform component
                {
                    if (components[i] == null)
                    {
                        stringBuilder.Append(prefix);
                        stringBuilder.AppendLine("null");
                    }
                    else
                    {
                        stringBuilder.Append(prefix);
                        stringBuilder.AppendLine(components[i].GetType().FullName);
                    }
                }
            }
            else
            {
                stringBuilder.AppendLine("0 components");
            }

            stringBuilder.Append(prefix);
            if (obj.childCount != 0)
            {
                stringBuilder.AppendLine($"{obj.childCount} children: ");

                string prefixBefore = prefix;
                prefix += "  ";
                for (int i = 0; i < obj.childCount; i++)
                {
                    stringBuilder.Append(prefix);
                    stringBuilder.Append(i);
                    stringBuilder.Append(": ");

                    Transform child = obj.GetChild(i);
                    if (child == null)
                    {
                        stringBuilder.AppendLine("null");
                    }
                    else
                    {
                        stringBuilder.AppendLine(child.name);
                        recursivePrintAllChildren(ref stringBuilder, prefix, child);
                    }
                }
                prefix = prefixBefore;
            }
            else
            {
                stringBuilder.AppendLine("0 children");
            }
        }
    }
}