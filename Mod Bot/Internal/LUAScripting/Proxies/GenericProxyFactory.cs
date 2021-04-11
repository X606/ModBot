using MoonSharp.Interpreter.Interop;
using System;

namespace InternalModBot.Proxies
{
    class GenericProxyFactory : IProxyFactory
    {
        public GenericProxyFactory(Type proxyType, Type targetType)
        {
            ProxyType = proxyType;
            TargetType = targetType;
        }

        public Type TargetType { get; }

        public Type ProxyType { get; }

        public object CreateProxyObject(object o)
        {
            return Activator.CreateInstance(ProxyType, o);
        }
    }
}
