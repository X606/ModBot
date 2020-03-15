using ModLibrary;
using System;
using UnityEngine;
using System.Collections.Generic;

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
                DelegateScheduler.Instance.Schedule(Crash, 1f);

            if (subCommands[0] == "unittest")
            {
                if (subCommands.Length > 1)
                {
                    bool foundUnitTest = ModBotUnitTestManager.TryRunUnitTest(subCommands[1]);
                    if (!foundUnitTest)
                    {
                        debug.Log("Unit test failed: Unit test \"" + subCommands[1] + "\" not found", Color.red);
                        return;
                    }
                }

                ModBotUnitTestManager.RunAllUnitTests();
            }

			if (subCommands[0] == "getplayfabids")
			{
				debug.Log("spawned players playfabids: ");
				List<FirstPersonMover> players = CharacterTracker.Instance.GetAllPlayers();
				foreach(FirstPersonMover player in players)
				{
					string playfabID = player.state.PlayFabID;
					string displayName = MultiplayerPlayerInfoManager.Instance.TryGetDisplayName(playfabID);
					if(displayName != null)
					{
						debug.Log(displayName + ": " + playfabID);
					}
				}
			}
			if (subCommands[0] == "redownloaddata")
			{
				StaticCoroutineRunner.StartStaticCoroutine(MultiplayerPlayerNameManager.DownloadDataFromFirebase());
				debug.Log("redownloading data...");
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
        static bool _isIgnoringCrashes = false;

        /// <summary>
        /// Starts ignoring crashes if we are currently configuerd to
        /// </summary>
        public static void Start()
        {
            int isIgnoringCrashesInt = PlayerPrefs.GetInt("IgnoreCrashes", 0);

            if (isIgnoringCrashesInt == 0)
            {
                _isIgnoringCrashes = false;
            }
            else if (isIgnoringCrashesInt == 1)
            {
                _isIgnoringCrashes = true;
            }
            else
            {
                debug.Log("IgnoreCrashes playerpref value out of range, resetting to default value");
                PlayerPrefs.SetInt("IgnoreCrashes", 0);
                _isIgnoringCrashes = false;
            }

            if (_isIgnoringCrashes)
                DelegateScheduler.Instance.Schedule(alert, 1f);
        }

        static void alert()
        {
            debug.Log(LocalizationManager.Instance.GetTranslatedString("ignoreallcrashes_savedwarning"), Color.red);
        }

        /// <summary>
        /// Sets if we should ignore crashes
        /// </summary>
        /// <param name="state"></param>
        public static void SetIsIgnoringCrashes(bool state)
        {
            _isIgnoringCrashes = state;
            int ignoreCrashesIntValue = _isIgnoringCrashes ? 1 : 0;

            PlayerPrefs.SetInt("IgnoreCrashes", ignoreCrashesIntValue);

            if (state)
            {
                debug.Log(LocalizationManager.Instance.GetTranslatedString("ignoreallcrashes_on"), Color.red);
            }
            else
            {
                debug.Log(LocalizationManager.Instance.GetTranslatedString("ignoreallcrashes_off"), Color.green);
            }
        }

        /// <summary>
        /// Gets if we are currently ignoring crashes
        /// </summary>
        /// <returns></returns>
        public static bool GetIsIgnoringCrashes()
        {
            return _isIgnoringCrashes;
        }
    }
}