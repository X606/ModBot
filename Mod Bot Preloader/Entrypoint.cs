using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Doorstop
{
    internal static class Entrypoint
    {
        /// <summary>
        /// Minimum amount of loaded assemblies to apply patches
        /// </summary>
        private const int MIN_REQUIRED_LOADED_ASSEMBLIES = 100;

        private const string GAME_DATA_DIRECTORY_NAME = "Clone Drone in the Danger Zone_Data";

        private const string MODBOT_INJECTION_MANAGER_NAME = "InternalModBot.ModBotHarmonyInjectionManager";

        private const string MODBOT_TRY_INJECT_NAME = "TryInject";

        public static void Start()
        {
            // todo: add compatibility with other mod loaders that use doorstop (like bepinex)
            // probably read a file with paths to their .dll files

            new Thread(WaitForGameInitialization).Start();
        }

        /// <summary>
        /// Waits until specific amount of assemblies is loaded then patches the game
        /// </summary>
        private static void WaitForGameInitialization()
        {
            while (true)
            {
                try
                {
                    if (AppDomain.CurrentDomain.GetAssemblies().Length > MIN_REQUIRED_LOADED_ASSEMBLIES)
                    {
                        Assembly assembly = Assembly.Load(AssemblyName.GetAssemblyName(getPathToAssembly("ModLibrary.dll")));
                        if (assembly == null) throw new Exception("Mod-Bot assembly not found!");

                        Type type = assembly.GetType(MODBOT_INJECTION_MANAGER_NAME);
                        if(type == null) throw new Exception("Injection manager not found!");

                        MethodInfo method = type.GetMethod(MODBOT_TRY_INJECT_NAME, BindingFlags.Static | BindingFlags.Public);
                        if (method == null) throw new Exception("Injection method not found!");

                        method.Invoke(null, null);

                        break;
                    }
                }
                catch (Exception exc)
                {
                    File.WriteAllText("CRASH.log", exc.ToString());
                    break;
                }
                Thread.Sleep(100);
            }
        }

        private static string getPathToAssembly(string assemblyFileName)
        {
            string gameDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(gameDir, GAME_DATA_DIRECTORY_NAME, "Managed", assemblyFileName);
        }
    }
}