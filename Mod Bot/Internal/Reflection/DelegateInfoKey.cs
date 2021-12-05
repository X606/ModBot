using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalModBot
{
    internal class DelegateInfoKey : MemberInfoKey
    {
        public readonly MatchType[] ArgumentTypes;
        public readonly MatchType ReturnType;

        public DelegateInfoKey(Type reflectedType, string memberName, MatchType[] argumentTypes, MatchType returnType) : base(reflectedType, memberName)
        {
            ArgumentTypes = argumentTypes;
            ReturnType = returnType;
        }

        public override int GetHashCode()
        {
            int hashCode = -918583468;

            hashCode = (hashCode * -1521134295) + base.GetHashCode();

            hashCode = (hashCode * -1521134295) + (ArgumentTypes?.Length ?? 0);
            if (ArgumentTypes != null)
            {
                foreach (MatchType item in ArgumentTypes)
                {
                    hashCode = (hashCode * -1521134295) + (item?.GetHashCode() ?? 0);
                }
            }

            hashCode = (hashCode * -1521134295) + (ReturnType?.GetHashCode() ?? 0);
            
            return hashCode;
        }

        protected override MemberInfoKey clone()
        {
            return new DelegateInfoKey(ReflectedType, MemberName, ArgumentTypes, ReturnType);
        }

        protected override IEnumerable<string> getStringValues()
        {
            foreach (string item in base.getStringValues())
            {
                yield return item;
            }

            yield return $"{nameof(ArgumentTypes)}: {(ArgumentTypes != null ? $"({ArgumentTypes.Join()})" : "null")}";
            yield return $"{nameof(ReturnType)}: {ReturnType?.ToString() ?? "null"}";
        }
    }
}
