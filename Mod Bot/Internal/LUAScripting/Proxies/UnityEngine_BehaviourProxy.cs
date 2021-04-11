using MoonSharp.Interpreter;
using UnityEngine;

namespace InternalModBot.Proxies
{
    [Proxy(typeof(Behaviour))]
    class UnityEngine_BehaviourProxy : UnityEngine_ComponentProxy
    {
        [MoonSharpHidden]
        Behaviour _target;

        [MoonSharpHidden]
        public UnityEngine_BehaviourProxy(Behaviour target) : base(target)
        {
            _target = target;
        }

        public bool enabled
        {
            get => _target.enabled;
            set => _target.enabled = value;
        }

        public bool isActiveAndEnabled => _target.isActiveAndEnabled;
    }
}
