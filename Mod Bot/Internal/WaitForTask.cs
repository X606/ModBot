using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InternalModBot
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
