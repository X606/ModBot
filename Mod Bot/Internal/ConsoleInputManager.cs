using ModLibrary;
using System;
using UnityEngine;
using System.Collections.Generic;
using ModBotWebsiteAPI;
using InternalModBot.Scripting;
using HarmonyLib;
using System.Reflection;
using System.Linq;
using System.Text;

namespace InternalModBot
{
    /// <summary>
    /// Used by Mod-Bot to define commands
    /// </summary>
    internal static class ConsoleInputManager
    {
        /// <summary>
        /// The same as <see cref="Mod.OnCommandRan(string)"/>, but called in Mod-Bot
        /// </summary>
        /// <param name="command"></param>
        public static void OnCommandRan(string command)
        {
            string[] subCommands = command.ToLower().Split(' ');

            switch (subCommands[0])
            {
                case "ignoreallcrashes":
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
                        break;
                    }
                case "crash":
                    DelegateScheduler.Instance.Schedule(Crash, 1f);
                    break;
                case "clearcache":
                    ModsManager.ClearCache();
                    break;
                case "listpatches":
                    {
                        foreach (MethodBase method in Harmony.GetAllPatchedMethods())
                        {
                            Patches patchInfo = Harmony.GetPatchInfo(method);

                            bool hasPrefix = patchInfo.Prefixes.Any();
                            bool hasPostfix = patchInfo.Postfixes.Any();
                            bool hasTranspiler = patchInfo.Transpilers.Any();
                            bool hasFinalizer = patchInfo.Finalizers.Any();

                            if (!hasPrefix && !hasPostfix && !hasTranspiler && !hasFinalizer)
                                continue;

                            debug.Log($"{method.FullDescription()}: ");
                            if (hasPrefix)
                            {
                                debug.Log("\tPrefixes: ");
                                foreach (Patch prefix in patchInfo.Prefixes)
                                {
                                    debug.Log($"\t\t{prefix.owner}: {prefix.PatchMethod.FullDescription()}");
                                }
                            }

                            if (hasPostfix)
                            {
                                debug.Log("\tPostfixes: ");
                                foreach (Patch postfix in patchInfo.Postfixes)
                                {
                                    debug.Log($"\t\t{postfix.owner}: {postfix.PatchMethod.FullDescription()}");
                                }
                            }

                            if (hasTranspiler)
                            {
                                debug.Log("\tTranspilers: ");
                                foreach (Patch transpiler in patchInfo.Transpilers)
                                {
                                    debug.Log($"\t\t{transpiler.owner}: {transpiler.PatchMethod.FullDescription()}");
                                }
                            }

                            if (hasFinalizer)
                            {
                                debug.Log("\tFinalizers: ");
                                foreach (Patch finalizer in patchInfo.Finalizers)
                                {
                                    debug.Log($"\t\t{finalizer.owner}: {finalizer.PatchMethod.FullDescription()}");
                                }
                            }

                            debug.Log(""); // Blank line
                        }

                        break;
                    }
                case "help":
                    {
                        debug.Log("Avaliable mod-bot commands (not including commands from mods):\n" +
                           "ignoreallcrashes [1 - 0], [on - off], [true, false]\n" +
                           "crash\n" +
                           "clearcache\n" +
                           "listpatches\n" +
                           "help\n" +
                           "getplayfabids [copy ids: true, false]"

                           , Color.yellow);
                        break;
                    }
                case "getplayfabids":
                    {
                        var usage = "Usage: getplayfabids [true, false] \ntrue - will copy the results into clipboard, false - won't";
                        bool? shouldCopy = null;
                        if (subCommands.Length < 2 || subCommands.Length > 2)
                        {
                            debug.Log(usage);
                            return;
                        }
                        if (subCommands[1] == "true")
                            shouldCopy = true;
                        if (subCommands[1] == "false")
                            shouldCopy = false;
                        if (!shouldCopy.HasValue)
                        {
                            debug.Log(usage);
                            return;
                        }
                        if (!GameModeManager.IsMultiplayer())
                        {
                            debug.Log("this command is only usable in multiplayer");
                            return;
                        }

                        var players = CharacterTracker.Instance.GetAllPlayers();
                        var namesAndIds = new StringBuilder();
                        debug.Log("\n");
                        for (int i = 0; i < players.Count; i++)
                        {
                            FirstPersonMover player = players[i];
                            var playfabID = player.GetPlayFabID();

                            MultiplayerPlayerInfoManager.Instance.GetPlayerInfoState(playfabID).GetOrPrepareSafeDisplayName(delegate (string displayName)
                            {
                                namesAndIds.Append($"{displayName} : {playfabID}");

                                if (i == players.Count - 1)//check if this player is the last one
                                {
                                    debug.Log(namesAndIds.ToString());
                                    if (shouldCopy.Value)
                                    {
                                        GUIUtility.systemCopyBuffer = namesAndIds.ToString();
                                        debug.Log("Successfully copied all playfab ids", Color.green);
                                    }
                                }
                                else
                                {
                                    namesAndIds.Append('\n');
                                }
                            });
                        }
                        break;
                    }
#if DEBUG
                case "debug":
                    DelegateScheduler.Instance.Schedule(delegate
                    {
                        ScriptObject scriptObject = new JavascriptScriptObject();
                        scriptObject.OnError += delegate (ScriptErrorType type, string message)
                        {
                            debug.Log("ERROR (" + type + "): " + message);
                        };

                        scriptObject.RunCode(@"
                        i = 25
                        j = 26
                        a = i + j
                        debug.log('aaaaaa')
                        ");

                    }, -1f);
                    break;
#endif
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

        [HarmonyPatch]
        static class Patches
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(ErrorManager), "HandleLog")]
            static bool ErrorManager_HandleLog_Prefix()
            {
                return !GetIsIgnoringCrashes();
            }
        }
    }
}