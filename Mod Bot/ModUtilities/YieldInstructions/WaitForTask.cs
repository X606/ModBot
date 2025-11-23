using System.Threading.Tasks;
using UnityEngine;

namespace ModLibrary.YieldInstructions
{
    internal class WaitForTask : CustomYieldInstruction
    {
        readonly Task _task;

        public WaitForTask(Task task)
        {
            _task = task;
        }

        public override bool keepWaiting => !_task.IsCompleted && !_task.IsCanceled;
    }
}
