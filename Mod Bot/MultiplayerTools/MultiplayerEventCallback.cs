using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bolt;

namespace ModLibrary
{
    /// <summary>
    /// Methods defined in this class can add and remove listeners to <see cref="Event"/> and invoke methods when that <see cref="Event"/> is recieved by our client
    /// </summary>
    public static class MultiplayerEventCallback
    {
        private static Dictionary<Type, List<object>> eventListeners = new Dictionary<Type, List<object>>();

        private static void AddEventTypeToDictionary(Type eventType)
        {
            if (!eventListeners.ContainsKey(eventType))
            {
                eventListeners.Add(eventType, new List<object>());
            }
        }

        private static bool HasCallbackDefined<T>(Action<T> callback) where T : Event
        {
            if (eventListeners[typeof(T)] == null)
            {
                return false;
            }

            return eventListeners[typeof(T)].Contains(callback);
        }

        /// <summary>
        /// Add a callback to a method from event of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Event"/> to add the callback to</typeparam>
        /// <param name="callback">The <see cref="Action{T1}"/> to call when the <see cref="Event"/> <typeparamref name="T"/> is recieved</param>
        /// <param name="allowDuplicate">Determines if multiple callbacks to the same method should be allowed</param>
        public static void AddEventListener<T>(Action<T> callback, bool allowDuplicate = false) where T : Event
        {
            AddEventTypeToDictionary(typeof(T));

            if (allowDuplicate || !HasCallbackDefined(callback))
            {
                eventListeners[typeof(T)].Add(callback);
            }
        }

        /// <summary>
        /// Remove the given callback from the callback list for the given <see cref="Event"/> type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Event"/> to remove a callback from</typeparam>
        /// <param name="callback">The <see cref="Action{T}"/> to remove from the callback list</param>
        public static void RemoveEventListener<T>(Action<T> callback) where T : Event
        {
            AddEventTypeToDictionary(typeof(T));
            
            eventListeners[typeof(T)].Remove(callback);
        }

        /// <summary>
        /// Called from inside Mod-Bot whenever we recive an event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_event"></param>
        internal static void OnEventRecieved<T>(T _event) where T : Event
        {
            if (!eventListeners.ContainsKey(typeof(T)))
            {
                return;
            }

            List<object> callbacks = eventListeners[typeof(T)];
            for (int i = 0; i < callbacks.Count; i++)
            {
                Action<T> callbackAction = callbacks[i] as Action<T>;

                if (callbackAction == null)
                {
                    callbacks.RemoveAt(i);
                    i--;
                    continue;
                }

                callbackAction(_event);
            }

        }


    }
}
