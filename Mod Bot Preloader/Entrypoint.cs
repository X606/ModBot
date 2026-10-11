using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Doorstop
{
    internal static class Entrypoint
    {
        /// <summary>
        /// Minimum amount of loaded assemblies to apply patches
        /// </summary>
        private const int MIN_REQUIRED_LOADED_ASSEMBLIES = 100;

        private const string MODBOT_INI_FILE = "modbot.ini";

        private const string ADDTIONAL_TARGET_ASSEMBLY_VAR = "additional_target_assembly=";

        private const string MODBOT_INJECTION_MANAGER_NAME = "InternalModBot.ModBotHarmonyInjectionManager";

        private const string MODBOT_TRY_INJECT_NAME = "TryInject";

        public static void Start()
        {
            string modbotFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string gameFolder = Path.GetDirectoryName(modbotFolder);
            string iniFilePath = Path.Combine(gameFolder, MODBOT_INI_FILE);

            // load additional Doorstop assembly
            if (File.Exists(iniFilePath) && tryParseAdditionalAssemblyPath(iniFilePath, out string additionalAssemblyPath))
            {
                string additionalPath = Path.Combine(gameFolder, additionalAssemblyPath);
                if (File.Exists(additionalPath))
                {
                    Assembly assembly = Assembly.LoadFile(additionalPath);
                    if (assembly != null)
                    {
                        Type type = assembly.GetType("Doorstop.Entrypoint");
                        if (type != null)
                        {
                            MethodInfo method = type.GetMethod("Start", BindingFlags.Static | BindingFlags.Public);
                            if (method != null)
                            {
                                Environment.SetEnvironmentVariable("DOORSTOP_INVOKE_DLL_PATH", additionalPath);
                                method.Invoke(null, null);
                            }
                        }
                    }
                }
            }

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
                        string modbotFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        Assembly assembly = Assembly.Load(AssemblyName.GetAssemblyName(Path.Combine(modbotFolder, "ModLibrary.dll")));
                        if (assembly == null) throw new Exception("Mod-Bot assembly not found!");

                        Type type = assembly.GetType(MODBOT_INJECTION_MANAGER_NAME);
                        if (type == null) throw new Exception("Injection manager not found!");

                        MethodInfo method = type.GetMethod(MODBOT_TRY_INJECT_NAME, BindingFlags.Static | BindingFlags.Public);
                        if (method == null) throw new Exception("Injection method not found!");

                        method.Invoke(null, null);

                        break;
                    }
                }
                catch (Exception exc)
                {
                    logCrash(exc);
                    break;
                }
                Thread.Sleep(100);
            }
        }

        private static bool tryParseAdditionalAssemblyPath(string iniFile, out string path)
        {
            path = null;

            try
            {
                using (FileStream fileStream = File.OpenRead(iniFile))
                using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 256))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (line.StartsWith(ADDTIONAL_TARGET_ASSEMBLY_VAR))
                        {
                            path = line.Substring(ADDTIONAL_TARGET_ASSEMBLY_VAR.Length);
                            break;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logCrash(exc);
                return false;
            }

            return !string.IsNullOrEmpty(path);
        }

        private static void logCrash(Exception exc)
        {
            File.WriteAllText($"crash {DateTime.Now:yyyy-MM-dd HHmmss}.log", exc.ToString());
        }
    }
}