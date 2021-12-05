using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InternalModBot
{
    internal class PropertyInfoKey : MemberInfoKey
    {
        public readonly MatchType[] IndexTypes;
        public readonly MatchType ReturnType;

        public PropertyInfoKey(Type reflectedType, string propertyName, Type returnType, object[] indexerArguments, Type[] argumentTypeOverrides) : base(reflectedType, propertyName)
        {
            if (returnType != null)
            {
                ReturnType = new MatchType(returnType);
            }
            else
            {
                ReturnType = null;
            }

            if (indexerArguments == null && argumentTypeOverrides == null)
            {
                IndexTypes = null;
            }
            else
            {
                int length = Math.Max(indexerArguments?.Length ?? 0, argumentTypeOverrides?.Length ?? 0);

                IndexTypes = new MatchType[length];
                for (int i = 0; i < length; i++)
                {
                    IndexTypes[i] = ReflectionUtils.GetMatchType(indexerArguments, argumentTypeOverrides, i);
                }
            }
        }

        protected PropertyInfoKey(Type reflectedType, string propertyName, MatchType returnType, MatchType[] indexTypes) : base(reflectedType, propertyName)
        {
            ReturnType = returnType;
            IndexTypes = indexTypes;
        }

        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();
            
            hashCode ^= (hashCode * -1521134295) + (IndexTypes?.Length ?? 0);
            if (IndexTypes != null)
            {
                foreach (MatchType indexType in IndexTypes)
                {
                    hashCode ^= (hashCode * -1521134295) + (indexType?.GetHashCode() ?? 0);
                }
            }

            hashCode ^= (hashCode * -1521134295) + (ReturnType?.GetHashCode() ?? 0);

            return hashCode;
        }

        protected override bool Equals(MemberInfoKey other)
        {
            return base.Equals(other) && other is PropertyInfoKey propertyInfoKey && equals(propertyInfoKey);
        }

        bool equals(PropertyInfoKey other)
        {
            bool indexTypes;
            if (IndexTypes is null || other.IndexTypes is null)
            {
                indexTypes = IndexTypes is null && other.IndexTypes is null;
            }
            else
            {
                indexTypes = IndexTypes.SequenceEqual(other.IndexTypes, MatchType.Comparer);
            }

            bool returnType;
            if (ReturnType is null || other.ReturnType is null)
            {
                returnType = ReturnType is null && other.ReturnType is null;
            }
            else
            {
                returnType = MatchType.Comparer.Equals(ReturnType, other.ReturnType);
            }

            return indexTypes && returnType;
        }

        protected override MemberInfoKey clone()
        {
            return new PropertyInfoKey(ReflectedType, MemberName, ReturnType, IndexTypes);
        }

        protected override IEnumerable<string> getStringValues()
        {
            foreach (string item in base.getStringValues())
            {
                yield return item;
            }

            yield return nameof(ReturnType) + ": " + (ReturnType?.ToString() ?? "null");
            yield return nameof(IndexTypes) + ": " + (IndexTypes != null ? ("[" + IndexTypes.Join() + "]") : "null");
        }
    }
}
