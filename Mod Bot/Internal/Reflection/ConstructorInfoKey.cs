using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InternalModBot
{
    internal class ConstructorInfoKey : MethodInfoKey
    {
        public bool IsTypeInitializer;

        public ConstructorInfoKey(Type reflectedType, bool isTypeInitializer, object[] arguments, Type[] argumentTypeOverrides) : base(reflectedType, isTypeInitializer ? ConstructorInfo.TypeConstructorName : ConstructorInfo.ConstructorName, arguments, argumentTypeOverrides)
        {
            IsTypeInitializer = isTypeInitializer;
        }

        protected ConstructorInfoKey(Type reflectedType, bool isTypeInitializer, MatchType[] parameterTypes) : base(reflectedType, isTypeInitializer ? ConstructorInfo.TypeConstructorName : ConstructorInfo.ConstructorName, parameterTypes)
        {
            IsTypeInitializer = isTypeInitializer;
        }

        protected override MemberInfoKey clone()
        {
            return new ConstructorInfoKey(ReflectedType, IsTypeInitializer, ParameterTypes);
        }

        protected override IEnumerable<string> getStringValues()
        {
            foreach (string item in base.getStringValues())
            {
                yield return item;
            }

            yield return nameof(IsTypeInitializer) + ": " + IsTypeInitializer;
        }
    }
}
