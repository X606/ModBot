using System;
using System.Collections.Generic;

namespace InternalModBot
{
    internal class EventInfoKey : MemberInfoKey
    {
        public readonly MatchType DelegateType;

        public EventInfoKey(Type reflectedType, string eventName, Type delegateType) : this(reflectedType, eventName, delegateType != null ? new MatchType(delegateType, false) : null)
        {
        }

        EventInfoKey(Type reflectedType, string eventName, MatchType delegateType) : base(reflectedType, eventName)
        {
            DelegateType = delegateType;
        }

        protected override MemberInfoKey clone()
        {
            return new EventInfoKey(ReflectedType, MemberName, DelegateType);
        }

        protected override IEnumerable<string> getStringValues()
        {
            foreach (string item in base.getStringValues())
            {
                yield return item;
            }

            yield return nameof(DelegateType) + ": " + (DelegateType?.ToString() ?? "null");
        }
    }
}
