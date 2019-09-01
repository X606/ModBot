using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InternalModBot;

namespace ModLibrary
{
    /// <summary>
    /// Dont call these methods directly from here
    /// </summary>
    public static class SingleMethodExtensions
    {
        /// <summary>
        /// Instead of having to filter the object array yourself you can use this method to get the object at a specific index in a much safer way
        /// </summary>
        /// <typeparam name="T">The type of the object at the index</typeparam>
        /// <param name="moddedObject"></param>
        /// <param name="index">The index of the object you want to get</param>
        /// <returns>The object at the specified index, casted to type <typeparamref name="T"/></returns>
        public static T GetObject<T>(this ModdedObject moddedObject, int index) where T : UnityEngine.Object
        {
            if (index < 0 || index >= moddedObject.objects.Count)
                return null;

            if (!(moddedObject.objects[index] is T))
            {
                throw new InvalidCastException("Object at index " + index + " was not of type " + typeof(T).ToString());
            }

            return moddedObject.objects[index] as T;
        }

        /// <summary>
        /// Returns true of the mod is enbaled, false if its disabled
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static bool IsModEnabled(this Mod mod)
        {
            bool? isModDeactivated = ModsManager.Instance.IsModDeactivated(mod);

            if (!isModDeactivated.HasValue)
                throw new Exception("Mod \"" + mod.GetModName() + "\" with unique id \"" + mod.GetUniqueID() + "\" could not found in modsmanager's list of mods!");

            return !isModDeactivated.Value;
        }
    }
}
