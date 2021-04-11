using MoonSharp.Interpreter;
using UnityEngine;

namespace InternalModBot.Proxies
{
    [Proxy(typeof(YieldInstruction))]
    class UnityEngine_YieldInstructionProxy
    {
        [MoonSharpHidden]
        YieldInstruction _target;

        [MoonSharpHidden]
        public UnityEngine_YieldInstructionProxy(YieldInstruction target)
        {
            _target = target;
        }
    }
}
