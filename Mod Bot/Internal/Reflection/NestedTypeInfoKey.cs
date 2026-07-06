using System;

namespace InternalModBot
{
    internal class NestedTypeInfoKey : MemberInfoKey
    {
        public NestedTypeInfoKey(Type reflectedType, string nestedTypeName) : base(reflectedType, nestedTypeName)
        {
        }

        protected override MemberInfoKey clone()
        {
            return new NestedTypeInfoKey(ReflectedType, MemberName);
        }
    }
}
