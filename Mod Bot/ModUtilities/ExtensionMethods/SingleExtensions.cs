using InternalModBot;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine;

namespace ModLibrary
{
    /// <summary>
    /// Dont call these methods directly from here
    /// </summary>
    public static class SingleMethodExtensions
    {
        /// <summary>
        /// Gets the <see cref="UnityEngine.Object"/> at the specified index and casts it to type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of the object at the index</typeparam>
        /// <param name="moddedObject"></param>
        /// <param name="index">The index of the <see cref="ModdedObject.objects"/> <see cref="List{T}"/></param>
        /// <returns>The <see cref="UnityEngine.Object"/> at the specified index, casted to type <typeparamref name="T"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="moddedObject"/> is <see langword="null"/></exception>
        /// <exception cref="IndexOutOfRangeException">If the given index is outside the range of <see cref="ModdedObject.objects"/></exception>
        /// <exception cref="InvalidCastException">If the <see cref="UnityEngine.Object"/> at index <paramref name="index"/> is not of type <typeparamref name="T"/></exception>
        public static T GetObject<T>(this ModdedObject moddedObject, int index) where T : UnityEngine.Object
        {
            if (moddedObject == null)
                throw new ArgumentNullException(nameof(moddedObject));

            if (index < 0 || index >= moddedObject.objects.Count)
                throw new IndexOutOfRangeException("Given index was not in the range of the objects list:\tMin: 0 " + "Max: " + (moddedObject.objects.Count - 1) + ", Recieved: " + index);

            if (moddedObject.objects[index] is T)
                return moddedObject.objects[index] as T;

            throw new InvalidCastException("Object at index " + index + " could not be casted to type " + typeof(T).ToString());
        }

        /// <summary>
        /// Checks if the given <see cref="Mod"/> is currently activated
        /// </summary>
        /// <param name="mod"></param>
        /// <returns><see langword="true"/> of the <see cref="Mod"/> is enabled, <see langword="false"/> if it's disabled</returns>
        /// <exception cref="Exception">If the <see cref="Mod"/> has not been loaded by <see cref="ModsManager"/></exception>
        public static bool IsModEnabled(this Mod mod)
        {
            bool? isModDeactivated = ModsManager.Instance.IsModDeactivated(mod);

            if (!isModDeactivated.HasValue)
                throw new Exception("Mod \"" + mod.GetModName() + "\" with unique id \"" + mod.GetUniqueID() + "\" could not found in ModsManager's list of mods!");

            return !isModDeactivated.Value;
        }
    }
}
