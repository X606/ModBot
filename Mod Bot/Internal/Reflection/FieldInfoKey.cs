using System;

namespace InternalModBot
{
    internal class FieldInfoKey : MemberInfoKey
    {
        public FieldInfoKey(Type reflectedType, string fieldName) : base(reflectedType, fieldName)
        {
        }

        protected override MemberInfoKey clone()
        {
            return new FieldInfoKey(ReflectedType, MemberName);
        }
    }
}
