using HarmonyLib;
using ModLibrary;
using MoonSharp.Interpreter;
using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UdpKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace InternalModBot
{
    static class LUAManager
    {
        public static DynValue CallFunctionSafe(Closure function, params object[] args)
        {
            try
            {
                return function.Call(args);
            }
            catch (ScriptRuntimeException runtimeException)
            {
                debug.LogAndShowConsole("Lua runtime error: " + runtimeException.DecoratedMessage);
                return DynValue.Nil;
            }
        }

        public static void RegisterTypes()
        {
            foreach (Type type in typeof(Ability).Assembly.GetTypes()) // Assembly-CSharp.dll
            {
                if (type.IsPublic)
                {
                    debug.Log(type.FullName);
                    try
                    {
                        UserData.RegisterType(type);
                    }
                    catch (Exception e)
                    {
                        debug.Log("Error registering type: " + e.ToString());
                    }
                }
            }

            foreach (Type type in typeof(BetterOutline).Assembly.GetTypes()) // Assembly-CSharp-firstpass.dll
            {
                if (type.IsPublic)
                {
                    debug.Log(type.FullName);
                    UserData.RegisterType(type);
                }
            }

            foreach (Type type in typeof(BoltAssertFailedException).Assembly.GetTypes()) // bolt.dll
            {
                if (type.IsPublic)
                {
                    debug.Log(type.FullName);
                    UserData.RegisterType(type);
                }
            }

            foreach (Type type in typeof(ActivateAutomatedLaserBlastEvent).Assembly.GetTypes()) // bolt.user.dll
            {
                if (type.IsPublic)
                {
                    debug.Log(type.FullName);
                    UserData.RegisterType(type);
                }
            }

            foreach (Type type in typeof(Accessor).Assembly.GetTypes()) // ModLibrary.dll
            {
                if (type.IsPublic)
                {
                    debug.Log(type.FullName);
                    UserData.RegisterType(type);
                }
            }

            foreach (Type type in typeof(AccelerationEvent).Assembly.GetTypes()) // UnityEngine.CoreModule.dll
            {
                if (type.IsPublic)
                {
                    debug.Log(type.FullName);
                    UserData.RegisterType(type);
                }
            }

            foreach (Type type in typeof(AnimationTriggers).Assembly.GetTypes()) // UnityEngine.UI.dll
            {
                if (type.IsPublic)
                {
                    debug.Log(type.FullName);
                    UserData.RegisterType(type);
                }
            }

            foreach (Type type in typeof(Canvas).Assembly.GetTypes()) // UnityEngine.UIModule.dll
            {
                if (type.IsPublic)
                {
                    debug.Log(type.FullName);
                    UserData.RegisterType(type);
                }
            }

            foreach (Type type in typeof(UdpEndPoint).Assembly.GetTypes()) // udpkit.common.dll
            {
                if (type.IsPublic)
                {
                    debug.Log(type.FullName);
                    UserData.RegisterType(type);
                }
            }

            foreach (Type type in typeof(Player).Assembly.GetTypes()) // Rewired_Core.dll
            {
                if (type.IsPublic)
                {
                    debug.Log(type.FullName);
                    UserData.RegisterType(type);
                }
            }
        }
    }
}
