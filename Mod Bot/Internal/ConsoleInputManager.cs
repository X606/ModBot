using ModLibrary;
using System;
using UnityEngine;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to define commands
    /// </summary>
    public static class ConsoleInputManager
    {
        /// <summary>
        /// The same as <see cref="Mod.OnCommandRan(string)"/>, but called in Mod-Bot
        /// </summary>
        /// <param name="command"></param>
        public static void OnCommandRan(string command)
        {
            string[] subCommands = command.ToLower().Split(' ');

            if (subCommands[0] == "ignoreallcrashes")
            {
                if (subCommands.Length < 2)
                {
                    debug.Log("Usage: ignoreallcrashes [1 - 0], [on - off], [true, false]");
                    return;
                }

                bool value;
                if (subCommands[1] == "1" || subCommands[1] == "on" || subCommands[1] == "true")
                {
                    value = true;
                }
                else if (subCommands[1] == "0" || subCommands[1] == "off" || subCommands[1] == "false")
                {
                    value = false;
                }
                else
                {
                    debug.Log("Usage: ignoreallcrashes[1 - 0], [on - off], [true, false]");
                    return;
                }

                IgnoreCrashesManager.SetIsIgnoringCrashes(value);
            }

            if (subCommands[0] == "crash")
            {
                DelegateScheduler.Instance.Schedule(Crash, 1f);
            }
        }

        /// <summary>
        /// Crashes the game
        /// </summary>
        public static void Crash()
        {
            throw new Exception("-Crashed from console-");
        }
    }
    
    /// <summary>
    /// Used by Mod-Bot to ignore crashes when the ignoreallcrashes command is active
    /// </summary>
    public static class IgnoreCrashesManager
    {
        private static bool isIgnoringCrashes = false;

        /// <summary>
        /// Starts ignoring crashes if we are currently configuerd to
        /// </summary>
        public static void Start()
        {
            int isIgnoringCrashesInt = PlayerPrefs.GetInt("IgnoreCrashes", 0);

            if (isIgnoringCrashesInt == 0)
            {
                isIgnoringCrashes = false;
            }
            else if (isIgnoringCrashesInt == 1)
            {
                isIgnoringCrashes = true;
            }
            else
            {
                debug.Log("IgnoreCrashes playerpref value out of range, resetting to default value");
                PlayerPrefs.SetInt("IgnoreCrashes", 0);
                isIgnoringCrashes = false;
            }

            if (isIgnoringCrashes)
            {
                DelegateScheduler.Instance.Schedule(Alert, 1f);
            }
        }
        
        private static void Alert()
        {
            debug.Log("Saved option message (IgnoreCrashes): All crashes are being ignored, this should only be enabled for testing purposes, turn it off by typing \"ignoreallcrashes 0\" into the console.", Color.red);
        }
        
        /// <summary>
        /// Sets if we should ignore crashes
        /// </summary>
        /// <param name="state"></param>
        public static void SetIsIgnoringCrashes(bool state)
        {
            isIgnoringCrashes = state;
            int ignoreCrashesIntValue = isIgnoringCrashes ? 1 : 0;

            PlayerPrefs.SetInt("IgnoreCrashes", ignoreCrashesIntValue);
            
            if (state)
            {
                debug.Log("The game is now ignoring all crashes, this option should only ever be enabled for testing. Having this option enabled will ignore soft crashes, but hard crashes can still occur.", Color.red);
            }
            else
            {
                debug.Log("The game is no longer ignoring crashes, this option should always be turned off for stability purposes.", Color.green);
            }
        }

        /// <summary>
        /// Gets if we are currently ignoring crashes
        /// </summary>
        /// <returns></returns>
        public static bool GetIsIgnoringCrashes()
        {
            return isIgnoringCrashes;
        }
    }
}