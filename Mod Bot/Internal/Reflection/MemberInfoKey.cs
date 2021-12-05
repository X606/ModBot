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

        public bool IsGetMemberInfoKey { get; set; } = false;

        public bool CanStepDownInTypeHierarchy => ReflectedType.BaseType != null;

        public MemberInfoKey StepDownInTypeHierarchy()
        {
            MemberInfoKey copy = clone();
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
            int hashCode = GetType().GetHashCode();
            hashCode ^= (hashCode * -1521134295) + (ReflectedType?.GetHashCode() ?? 0);
            hashCode ^= (hashCode * -1521134295) + MemberName.GetHashCode();
            hashCode ^= (hashCode * -1521134295) + IsGetMemberInfoKey.GetHashCode();
            return hashCode;
        }

        protected abstract MemberInfoKey clone();

        protected virtual IEnumerable<string> getStringValues()
        {
            yield return nameof(ReflectedType) + ": " + (ReflectedType?.FullDescription() ?? "null");
            yield return nameof(MemberName) + ": " + MemberName;
            yield return nameof(IsGetMemberInfoKey) + ": " + IsGetMemberInfoKey;
        }

        public override string ToString()
        {
            return GetType().FullDescription() + ": {" + getStringValues().Join() + "}";
        }
    }
}
