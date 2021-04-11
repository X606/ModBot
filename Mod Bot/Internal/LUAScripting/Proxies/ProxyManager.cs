using ModLibrary;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InternalModBot.Proxies
{
    static class ProxyManager
    {
        public static void RegisterProxies()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types)
            {
                ProxyAttribute proxyAttribute = type.GetCustomAttribute<ProxyAttribute>();
                if (proxyAttribute != null)
                    UserData.RegisterProxyType(new GenericProxyFactory(type, proxyAttribute.TargetType), InteropAccessMode.Default, type.FullName);
            }
        }
    }
}
