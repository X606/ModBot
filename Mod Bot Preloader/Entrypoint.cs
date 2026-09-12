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
                        AssemblyName assemblyName = AssemblyName.GetAssemblyName(getPathToAssembly("ModLibrary"));
                        Type type = Assembly.Load(assemblyName).GetType("InternalModBot.ModBotHarmonyInjectionManager");
                        MethodInfo method = type.GetMethod("TryInject", BindingFlags.Static | BindingFlags.Public);
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

        private static string getPathToAssembly(string assemblyName)
        {
            string gameDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(gameDir, "Clone Drone in the Danger Zone_Data", "Managed", assemblyName + ".dll");
        }
    }
}