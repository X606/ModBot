using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
