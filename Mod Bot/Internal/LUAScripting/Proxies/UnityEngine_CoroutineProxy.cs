using MoonSharp.Interpreter;

namespace InternalModBot.Proxies
{
    [Proxy(typeof(UnityEngine.Coroutine))]
    class UnityEngine_CoroutineProxy : UnityEngine_YieldInstructionProxy
    {
        [MoonSharpHidden]
        UnityEngine.Coroutine _target;

        [MoonSharpHidden]
        public UnityEngine_CoroutineProxy(UnityEngine.Coroutine target) : base(target)
        {
            _target = target;
        }
    }
}
