using System;
using System.Collections.Concurrent;

namespace InternalModBot
{
    /// <summary>
    /// Allows us to schedule an action to run on the next update from another thread
    /// </summary>
    internal static class ThreadedDelegateScheduler
    {
        static ConcurrentQueue<Action> _scheduledActions = new ConcurrentQueue<Action>();

        /// <summary>
        /// Calls the passed action on the next update on the main thread
        /// </summary>
        /// <param name="action"></param>
        public static void CallActionNextUpdate(Action action)
        {
            _scheduledActions.Enqueue(action);
        }

        internal static void Update()
        {
            while (_scheduledActions.TryDequeue(out Action action))
            {
                if (action != null)
                    action();
            }
        }
    }
}