using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalModBot
{
	public static class ThreadedDelegateScheduler
	{
		static ConcurrentQueue<Action> _scheduledActions = new ConcurrentQueue<Action>();

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
