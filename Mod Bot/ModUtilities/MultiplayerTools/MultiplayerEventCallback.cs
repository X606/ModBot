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
        static Dictionary<Type, List<object>> _eventListeners = new Dictionary<Type, List<object>>();

        static void addEventTypeToDictionary(Type eventType)
        {
            if (!_eventListeners.ContainsKey(eventType))
                _eventListeners.Add(eventType, new List<object>());
        }

        static bool hasCallbackDefined<T>(Action<T> callback) where T : Event
        {
            if (_eventListeners[typeof(T)] == null)
                return false;

            return _eventListeners[typeof(T)].Contains(callback);
        }

        /// <summary>
        /// Add a callback to a method from event of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Event"/> to add the callback to</typeparam>
        /// <param name="callback">The <see cref="Action{T1}"/> to call when the <see cref="Event"/> <typeparamref name="T"/> is recieved</param>
        /// <param name="allowDuplicate">Determines if multiple callbacks to the same method should be allowed</param>
        public static void AddEventListener<T>(Action<T> callback, bool allowDuplicate = false) where T : Event
        {
            addEventTypeToDictionary(typeof(T));

            if (allowDuplicate || !hasCallbackDefined(callback))
                _eventListeners[typeof(T)].Add(callback);
        }

        /// <summary>
        /// Remove the given callback from the callback list for the given <see cref="Event"/> type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Event"/> to remove a callback from</typeparam>
        /// <param name="callback">The <see cref="Action{T}"/> to remove from the callback list</param>
        public static void RemoveEventListener<T>(Action<T> callback) where T : Event
        {
            addEventTypeToDictionary(typeof(T));
            
            _eventListeners[typeof(T)].Remove(callback);
        }

        /// <summary>
        /// Called from inside Mod-Bot whenever we recive an event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_event"></param>
        internal static void OnEventRecieved<T>(T _event) where T : Event
        {
            if (!_eventListeners.ContainsKey(typeof(T)))
                return;

            List<object> callbacks = _eventListeners[typeof(T)];
            for (int i = callbacks.Count - 1; i >= 0; i--)
            {
                if (callbacks[i] is Action<T> callbackAction)
                {
                    callbackAction(_event);
                }
                else
                {
                    callbacks.RemoveAt(i);
                }
            }
        }
    }
}
