using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Doorstop
{
    internal static class Entrypoint
    {
        public static Thread MainThread;

        public static void Start()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(AppDomain.CurrentDomain.ToString());

            try
            {
                MainThread = Thread.CurrentThread;
                Thread initThread = new Thread(WaitForUnityLoad);
                initThread.Start();
            }
            catch (Exception e)
            {
                string location = Assembly.GetExecutingAssembly().Location;
                location = location.Remove(location.LastIndexOf('\\'));

                stringBuilder.AppendLine(location);
                stringBuilder.AppendLine(e.ToString());
            }
            finally
            {
                File.WriteAllText("LOG.log", stringBuilder.ToString());
            }
        }

        private static void WaitForUnityLoad()
        {
            while (true)
            {
                try
                {
                    if (AppDomain.CurrentDomain.GetAssemblies().Length > 100)
                    {
                        prefix();
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


        private static void prefix()
        {
            new test();
        }

        internal static Assembly ResolveCurrentDirectory(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);
            Assembly result;
            try
            {
                string location = Assembly.GetExecutingAssembly().Location;
                location = location.Remove(location.LastIndexOf('\\'));

                result = Assembly.LoadFile(Path.Combine(location, "Clone Drone in the Danger Zone_Data", "Managed", assemblyName.Name + ".dll"));
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        private static string getPathToAssembly(string assemblyName)
        {
            string location = Assembly.GetExecutingAssembly().Location;
            location = location.Remove(location.LastIndexOf('\\'));

            return Path.Combine(location, "Clone Drone in the Danger Zone_Data", "Managed", assemblyName + ".dll");
        }

        private class test
        {
            public test()
            {
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(getPathToAssembly("ModLibrary"));
                Type type = Assembly.Load(assemblyName).GetType("InternalModBot.ModBotHarmonyInjectionManager");
                MethodInfo method = type.GetMethod("TryInject", BindingFlags.Static | BindingFlags.Public);
                method.Invoke(null, null);

                UnityEngine.Debug.Log("Injected Mod-Bot patches!");
            }
        }
    }
}