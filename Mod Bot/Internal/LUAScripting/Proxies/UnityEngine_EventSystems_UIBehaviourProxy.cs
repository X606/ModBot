using MoonSharp.Interpreter;
using UnityEngine.EventSystems;

namespace InternalModBot.Proxies
{
    [Proxy(typeof(UIBehaviour))]
    class UnityEngine_EventSystems_UIBehaviourProxy : UnityEngine_MonoBehaviourProxy
    {
        [MoonSharpHidden]
        UIBehaviour _target;

        [MoonSharpHidden]
        public UnityEngine_EventSystems_UIBehaviourProxy(UIBehaviour target) : base(target)
        {
            _target = target;
        }

        public bool isActive()
        {
            return _target.IsActive();
        }

        public bool isDestroyed()
        {
            return _target.IsDestroyed();
        }
    }
}
