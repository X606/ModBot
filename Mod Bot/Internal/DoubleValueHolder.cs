using System.Collections.Generic;
using System.Text;
using System;

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
        /// Initializes a new instance of the <see cref="DoubleValueHolder{T1, T2}"/> struct
        /// </summary>
        /// <param name="_first">The first value to store</param>
        /// <param name="_second">The second value to hold</param>
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
