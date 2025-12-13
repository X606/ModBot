using System.Threading.Tasks;
using UnityEngine;

namespace ModLibrary.YieldInstructions
{
    /// <summary>
    /// Waits until the task is completed or canceled
    /// </summary>
    public class WaitForTaskCompletion : CustomYieldInstruction
    {
        readonly Task _task;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForTaskCompletion"/> instruction
        /// </summary>
        /// <param name="task">The <see cref="Task"/> to check</param>
        public WaitForTaskCompletion(Task task)
        {
            _task = task;
        }

        /// <summary>
        /// Returns if the <see cref="Coroutine"/> should keep waiting
        /// </summary>
        public override bool keepWaiting => !_task.IsCompleted && !_task.IsCanceled;
    }
}