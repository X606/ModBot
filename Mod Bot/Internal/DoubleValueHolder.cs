using System;
using System.Collections.Generic;
using System.Text;

namespace InternalModBot
{
    /// <summary>
    /// Used to hold 2 values in 1 object
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public struct DoubleValueHolder<T1, T2>
    {
        /// <summary>
        /// Sets the first value to the first argument and the second value to the second argument
        /// </summary>
        /// <param name="_first"></param>
        /// <param name="_second"></param>
        public DoubleValueHolder(T1 _first, T2 _second)
        {
            FirstValue = _first;
            SecondValue = _second;
        }
        /// <summary>
        /// The first value held by this object
        /// </summary>
        public T1 FirstValue;
        /// <summary>
        /// The second value held by this object
        /// </summary>
        public T2 SecondValue;
    }
}
