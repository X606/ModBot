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

        /// <summary>
        /// Compares 2 instances of the <see cref="DoubleValueHolder{T1, T2}"/> struct
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(DoubleValueHolder<T1, T2> left, DoubleValueHolder<T1, T2> right)
        {
            bool firstIsNull = false;
            bool secondIsNull = false;

            if (left.FirstValue == null || right.FirstValue == null)
            {
                firstIsNull = left.FirstValue == null && right.FirstValue == null;
                if (!firstIsNull)
                    return false;
            }
            if (left.SecondValue == null || right.SecondValue == null)
            {
                secondIsNull = left.SecondValue == null && right.SecondValue == null;
                if (!secondIsNull)
                    return false;
            }

            if (firstIsNull && secondIsNull)
                return true;

            return left.FirstValue.Equals(right.FirstValue) && left.SecondValue.Equals(right.SecondValue);
        }

        /// <summary>
        /// Compares 2 instances of the <see cref="DoubleValueHolder{T1, T2}"/> struct
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(DoubleValueHolder<T1, T2> left, DoubleValueHolder<T1, T2> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Checks if the current instance has the same values as the given object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is DoubleValueHolder<T1, T2>)
                return this == (DoubleValueHolder<T1, T2>)obj;

            return false;
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets info about the current value and places it in a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "DoubleValueHolder(" + FirstValue.ToString() + ", " + SecondValue.ToString() + ")";
        }

    }
}
