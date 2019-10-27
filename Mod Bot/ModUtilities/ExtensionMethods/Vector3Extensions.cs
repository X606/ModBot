using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModLibrary
{
    /// <summary>
    /// Defines extension methods for <see cref="Vector3"/>
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// Gets the direction from one <see cref="Vector3"/> to another
        /// </summary>
        /// <param name="startVector"></param>
        /// <param name="destinationVector">The <see cref="Vector3"/> to get the direction to</param>
        /// <returns></returns>
        public static Vector3 GetDirectionTo(this Vector3 startVector, Vector3 destinationVector)
        {
            return destinationVector - startVector;
        }

        /// <summary>
        /// Gets the normalized direction from one <see cref="Vector3"/> to another
        /// </summary>
        /// <param name="startVector"></param>
        /// <param name="destinationVector">The <see cref="Vector3"/> to get the direction to</param>
        /// <returns></returns>
        public static Vector3 GetDirectionToNormalized(this Vector3 startVector, Vector3 destinationVector)
        {
            return startVector.GetDirectionTo(destinationVector).normalized;
        }
    }
}
