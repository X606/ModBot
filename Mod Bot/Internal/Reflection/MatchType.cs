using HarmonyLib;
using System;
using System.Collections.Generic;

namespace InternalModBot
{
    internal class MatchType
    {
        class MatchTypeComparer : IEqualityComparer<MatchType>
        {
            bool IEqualityComparer<MatchType>.Equals(MatchType x, MatchType y)
            {
                return (x is null && y is null) || x.Equals(y);
            }

            int IEqualityComparer<MatchType>.GetHashCode(MatchType obj) => obj.GetHashCode();
        }

        static MatchTypeComparer _comparer;
        public static IEqualityComparer<MatchType> Comparer
        {
            get
            {
                if (_comparer == null)
                    _comparer = new MatchTypeComparer();

                return _comparer;
            }
        }

        public readonly Type Type;
        public readonly bool AllowInheritance;

        public MatchType(Type type) : this(type, true)
        {
        }

        public MatchType(Type type, bool allowInheritance)
        {
            Type = type;
            AllowInheritance = allowInheritance;
        }

        public bool IsAssignableTo(Type otherType)
        {
            return Type == otherType || (AllowInheritance && otherType.IsAssignableFrom(Type));
        }

        public bool IsAssignableFrom(Type otherType)
        {
            return Type == otherType || (AllowInheritance && Type.IsAssignableFrom(otherType));
        }

        public static MatchType[] FromTypeArray(Type[] types, bool allowInheritance = true)
        {
            MatchType[] result = new MatchType[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                result[i] = types[i] != null ? new MatchType(types[i], allowInheritance) : null;
            }

            return result;
        }

        public override int GetHashCode()
        {
            int hashCode = -1940753017;
            hashCode = (hashCode * -1521134295) + Type.GetHashCode();
            hashCode = (hashCode * -1521134295) + AllowInheritance.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is MatchType other)
                return Type == other.Type && AllowInheritance == other.AllowInheritance;

            return false;
        }

        public override string ToString()
        {
            return Type.FullDescription() + " [" + nameof(AllowInheritance) + ": " + AllowInheritance + "]";
        }
    }
}
