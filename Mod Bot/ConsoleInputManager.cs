using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModLibrary;
using UnityEngine;

namespace InternalModBot
{
    public static class ConsoleInputManager
    {
        public static void OnCommandRan(string command)
        {
            command = command.ToLower();
            string[] subCommands = command.Split(' ');

            if (subCommands[0] == "ignoreallcrashes")
            {
                if (subCommands.Length <= 1)
                {
                    debug.Log("usage: ignoreallcrashes <number 0-1>");
                    return;
                }
                IgnoreCrashesManager.SetIsIgnoringCrashes(subCommands[1] == "1");
            }

            if (subCommands[0] == "crash")
            {
                Delayed.TriggerAfterDelay(new fakeAction(typeof(ConsoleInputManager).GetMethod("Crash"), null), 1);
            }


        }




        public static void Crash()
        {
            throw new Exception("Boom crash");
        }
    }




    public static class IgnoreCrashesManager
    {
        private static bool isIgnoringCrashes = false;

        public static void Start()
        {
            int get = PlayerPrefs.GetInt("IgnoreCrashes", 0);
            isIgnoringCrashes = get == 1;

            if (isIgnoringCrashes)
            {
                Delayed.TriggerAfterDelay(new fakeAction(typeof(IgnoreCrashesManager).GetMethod("Alert"), null), 1);
            }
            
        }


        public static void Alert()
        {
            debug.Log("Rememberd that you wanted to ingnore crashes. If you want to be more secure turn it off at anytime by typing in \"ignoreallcrashes 0\" into the console :)", Color.red);
        }


        public static void SetIsIgnoringCrashes(bool state)
        {
            isIgnoringCrashes = state;
            if (isIgnoringCrashes)
            {
                PlayerPrefs.SetInt("IgnoreCrashes", 1);
            } else
            {
                PlayerPrefs.SetInt("IgnoreCrashes", 0);
            }
            

            if (state)
            {
                debug.Log("The game is now ignoring all crashes, this means that if something goes wrong it could REALLY go wrong. But it also means that you dont crash like ever!", Color.red);
            } else
            {
                debug.Log("The game is now NOT ignoring crashes anymore, so you should be a lot safer now!", Color.green);
            }
        }

        public static bool GetIsIgnoringCrashes()
        {
            return isIgnoringCrashes;
        }

    }
}
