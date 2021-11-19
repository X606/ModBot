using HarmonyLib;
using System;
using System.Collections.Generic;

namespace InternalModBot
{
    internal abstract class MemberInfoKey
    {
        class MemberInfoEqualityComparer : IEqualityComparer<MemberInfoKey>
        {
            bool IEqualityComparer<MemberInfoKey>.Equals(MemberInfoKey x, MemberInfoKey y)
            {
                if (x is null || y is null)
                    return x is null && y is null;

                return x.Equals(y);
            }

            int IEqualityComparer<MemberInfoKey>.GetHashCode(MemberInfoKey obj) => obj.GetHashCode();
        }

        static MemberInfoEqualityComparer _equalityComparer;
        public static IEqualityComparer<MemberInfoKey> EqualityComparer
        {
            get
            {
                if (_equalityComparer == null)
                    _equalityComparer = new MemberInfoEqualityComparer();

                return _equalityComparer;
            }
        }

        public bool CanStepDownInTypeHierarchy => ReflectedType.BaseType != null;

        public MemberInfoKey StepDownInTypeHierarchy()
        {
            MemberInfoKey copy = Clone();
            copy.ReflectedType = ReflectedType.BaseType;
            return copy;
        }

        public Type ReflectedType { get; private set; }
        public readonly string MemberName;

        protected MemberInfoKey(Type reflectedType, string memberName)
        {
            if (string.IsNullOrEmpty(memberName))
                throw new ArgumentException($"'{nameof(memberName)}' cannot be null or empty.", nameof(memberName));

            ReflectedType = reflectedType ?? throw new ArgumentNullException(nameof(reflectedType));
            MemberName = memberName;
        }

        protected virtual bool Equals(MemberInfoKey other)
        {
            return GetType() == other.GetType() && ReflectedType == other.ReflectedType && MemberName == other.MemberName;
        }

        public override int GetHashCode()
        {
            int hashCode = -1874024137;
            hashCode ^= (hashCode * -1521134295) + ReflectedType.GetHashCode();
            hashCode ^= (hashCode * -1521134295) + MemberName.GetHashCode();
            return hashCode;
        }

        protected abstract MemberInfoKey Clone();

        public override string ToString()
        {
            return $"{{ReflectedType: {ReflectedType.FullDescription()}, MemberName: {MemberName}}}";
        }
    }
}
