using HarmonyLib;
using System;
using System.Linq;

namespace InternalModBot
{
    internal class MethodInfoKey : MemberInfoKey
    {
        public readonly MatchType[] ParameterTypes;

        public MethodInfoKey(Type reflectedType, string methodName, object[] arguments, Type[] argumentTypeOverrides) : base(reflectedType, methodName)
        {
            if (arguments == null && argumentTypeOverrides == null)
            {
                ParameterTypes = null;
            }
            else
            {
                int length = Math.Max(arguments?.Length ?? 0, argumentTypeOverrides?.Length ?? 0);

                ParameterTypes = new MatchType[length];
                for (int i = 0; i < length; i++)
                {
                    ParameterTypes[i] = ReflectionUtils.GetMatchType(arguments, argumentTypeOverrides, i);
                }
            }
        }

        protected MethodInfoKey(Type reflectedType, string methodName, MatchType[] parameterTypes) : base(reflectedType, methodName)
        {
            ParameterTypes = parameterTypes ?? new MatchType[0];
        }

        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();
            
            hashCode ^= (hashCode * -1521134295) + (ParameterTypes?.Length ?? 0);
            if (ParameterTypes != null)
            {
                foreach (MatchType parameterType in ParameterTypes)
                {
                    hashCode ^= (hashCode * -1521134295) + (parameterType?.GetHashCode() ?? 0);
                }
            }

            return hashCode;
        }

        protected override bool Equals(MemberInfoKey other)
        {
            return base.Equals(other) && other is MethodInfoKey methodInfoKey && equals(methodInfoKey);
        }

        bool equals(MethodInfoKey other)
        {
            if (ParameterTypes is null || other.ParameterTypes is null)
                return ParameterTypes is null && other.ParameterTypes is null;

            return ParameterTypes.SequenceEqual(other.ParameterTypes, MatchType.Comparer);
        }

        protected override MemberInfoKey Clone()
        {
            return new MethodInfoKey(ReflectedType, MemberName, ParameterTypes);
        }

        public override string ToString()
        {
            return $"{{ReflectedType: {ReflectedType.FullDescription()}, MemberName: {MemberName}, ParameterTypes: ({ParameterTypes.Join()})}}";
        }
    }
}
