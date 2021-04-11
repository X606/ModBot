using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalModBot.Proxies
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    sealed class ProxyAttribute : Attribute
    {
        public ProxyAttribute(Type targetType)
        {
            TargetType = targetType;
        }

        public Type TargetType { get; }
    }
}
