using HarmonyLib;
using ModLibrary;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using MoonSharp.Interpreter.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InternalModBot
{
    internal class LUAMod : IMod
    {
        ModInfo _modInfo;
        Script _script;

        public LUAMod(ModInfo modInfo)
        {
            _modInfo = modInfo;

            _script = new Script(CoreModules.Bit32 | CoreModules.Coroutine | CoreModules.ErrorHandling | CoreModules.Math | CoreModules.OS_Time | CoreModules.String | CoreModules.Table | CoreModules.TableIterators);
            _script.Options.ScriptLoader = new FileSystemScriptLoader();

            StaticLUACallbackFunctions.AddGlobalFunctions(_script);

            try
            {
                _script.DoFile(modInfo.MainLuaFilePath);
            }
            catch (SyntaxErrorException syntaxError)
            {
                debug.LogAndShowConsole("Syntax error when parsing lua for \"" + _modInfo.DisplayName + "\": " + syntaxError.DecoratedMessage);
            }
        }

        DynValue tryCallFunction(string name, params object[] arguments)
        {
            if (tryGetFunction(name, out Closure function))
            {
                return LUAManager.CallFunctionSafe(function, arguments);
            }
            else
            {
                return DynValue.Void;
            }
        }

        bool functionExists(string name)
        {
            return tryGetFunction(name, out _);
        }

        bool tryGetFunction(string name, out Closure function)
        {
            DynValue functionDynValue = _script.Globals.Get(name);
            if (functionDynValue != null && functionDynValue.IsNotNil() && functionDynValue.Type == DataType.Function)
            {
                function = functionDynValue.Function;
                return true;
            }

            function = null;
            return false;
        }

        public string HarmonyID => "com.Mod-Bot.Mod." + ModInfo.Type + "." + ModInfo.UniqueID;

        public Harmony HarmonyInstance => new Harmony(HarmonyID);

        public ModInfo ModInfo => _modInfo;

        public void AfterUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            tryCallFunction("afterUpgradesRefreshed", owner, upgrades);
        }

        public bool ImplementsSettingsWindow()
        {
            return functionExists("buildSettingsWindow");
        }
        public void CreateSettingsWindow(ModOptionsWindowBuilder builder)
        {
            tryCallFunction("buildSettingsWindow", builder);
        }

        public void GlobalUpdate()
        {
            tryCallFunction("update", Time.deltaTime);
        }

        public void OnCharacterKilled(Character killedCharacter, Character killerCharacter, DamageSourceType damageSourceType, int attackID)
        {
            tryCallFunction("characterKilled", killedCharacter, killerCharacter, damageSourceType, attackID);
        }

        public void OnCharacterModelCreated(FirstPersonMover owner)
        {
            tryCallFunction("characterModelCreated", owner);
        }

        public void OnCharacterSpawned(Character character)
        {
            tryCallFunction("characterSpawned", character);
        }

        public void OnCharacterUpdate(Character character)
        {
            tryCallFunction("characterUpdate", character);
        }

        public void OnClientConnectedToServer()
        {
            tryCallFunction("connectedToServer");
        }

        public void OnClientDisconnectedFromServer()
        {
            tryCallFunction("disconnectedFromServer");
        }

        public void OnCommandRan(string command)
        {
            tryCallFunction("consoleCommand", command);
        }

        public void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
            tryCallFunction("firstPersonMoverSpawned", firstPersonMover);
        }

        public void OnFirstPersonMoverUpdate(FirstPersonMover firstPersonMover)
        {
            tryCallFunction("firstPersonMoverUpdate", firstPersonMover);
        }

        public void OnLanguageChanged(string newLanguageID, Dictionary<string, string> localizationDictionary)
        {
            DynValue result = tryCallFunction("getLocalizationTable", newLanguageID);
            if (result.IsVoid()) // Function doesn't exist
                return;

            if (result.IsNotNil() && result.Type == DataType.Table)
            {
                IEnumerable<TablePair> pairs = result.Table.Pairs;
                foreach (TablePair pair in pairs)
                {
                    string key = pair.Key.CastToString();
                    string value = pair.Value.CastToString();

                    if (key != null && value != null)
                    {
                        if (!localizationDictionary.ContainsKey(key))
                        {
                            localizationDictionary.Add(key, value);
                        }
                        else
                        {
                            debug.Log("Error adding localization key \"" + key + "\" from " + _modInfo.DisplayName + ": Key already present in localization dictionary");
                        }
                    }
                    else
                    {
                        if (key == null)
                        {
                            debug.Log("Error adding localization key from " + _modInfo.DisplayName + ": Cannot convert key type \"" + pair.Key.Type + "\" to string");
                        }
                        else // if (value == null)
                        {
                            debug.Log("Error adding localization key \"" + key + "\" from " + _modInfo.DisplayName + ": Cannot convert value type \"" + pair.Value.Type + "\" to string");
                        }
                    }
                }
            }
            else
            {
                debug.Log("Error adding localization dictionary from " + _modInfo.DisplayName + ": getLocalizationTable returned type " + result.Type + ", expected " + DataType.Table);
            }
        }

        public void OnLevelEditorStarted()
        {
            tryCallFunction("levelEditorStarted");
        }

        public void OnModDeactivated()
        {
            tryCallFunction("modDeactivated");
        }

        public void OnModEnabled()
        {
            tryCallFunction("modEnabled");
        }

        public void OnModLoaded()
        {
            tryCallFunction("modLoaded");
        }

        public void OnModRefreshed()
        {
            tryCallFunction("modRefreshed");
        }

        public void OnMultiplayerEventReceived(GenericStringForModdingEvent moddedEvent)
        {
            tryCallFunction("multiplayerMessage", moddedEvent.EventData);
        }

        public UnityEngine.Object OnResourcesLoad(string path)
        {
            // TODO: Implement this
            return null;
        }

        public void OnUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            tryCallFunction("beforeUpgradesRefreshed", owner, upgrades);
        }

        public bool ShouldCursorBeEnabled()
        {
            DynValue result = tryCallFunction("enableCursor");
            return result.IsNotNil() && result.Type == DataType.Boolean && result.Boolean;
        }
    }
}
