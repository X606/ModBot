using System;
using System.Collections.Generic;
using System.Text;

namespace InternalModBot
{
    public struct DoubleValueHolder<T1, T2>
    {
        public DoubleValueHolder(T1 _first, T2 _second)
        {
            FirstValue = _first;
            SecondValue = _second;
        }

        public T1 FirstValue;
        public T2 SecondValue;
    }
}
