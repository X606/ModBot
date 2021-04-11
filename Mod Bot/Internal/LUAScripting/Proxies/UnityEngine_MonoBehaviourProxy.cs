using MoonSharp.Interpreter;
using UnityEngine;

namespace InternalModBot.Proxies
{
    [Proxy(typeof(MonoBehaviour))]
    class UnityEngine_MonoBehaviourProxy : UnityEngine_BehaviourProxy
    {
        [MoonSharpHidden]
        MonoBehaviour _target;

        [MoonSharpHidden]
        public UnityEngine_MonoBehaviourProxy(MonoBehaviour target) : base(target)
        {
            _target = target;
        }

        public bool isInvoking(string methodName = null)
        {
            if (methodName == null)
            {
                return _target.IsInvoking();
            }
            else
            {
                return _target.IsInvoking(methodName);
            }
        }

        public void cancelInvoke(string methodName = null)
        {
            if (methodName == null)
            {
                _target.CancelInvoke();
            }
            else
            {
                _target.CancelInvoke(methodName);
            }
        }

        public void invoke(string methodName, float delay = 0f)
        {
            _target.Invoke(methodName, delay);
        }

        public void invokeRepeating(string methodName, float delay, float repeatRate)
        {
            _target.InvokeRepeating(methodName, delay, repeatRate);
        }

        public UnityEngine.Coroutine startCoroutine(string methodName, DynValue value)
        {
            return _target.StartCoroutine(methodName, value?.ToObject());
        }

        public void stopCoroutine(UnityEngine.Coroutine routine)
        {
            _target.StopCoroutine(routine);
        }
        public void stopCoroutine(string methodName)
        {
            _target.StopCoroutine(methodName);
        }

        public void stopAllCoroutines()
        {
            _target.StopAllCoroutines();
        }

        public bool useGUILayout
        {
            get => _target.useGUILayout;
            set => _target.useGUILayout = value;
        }
    }
}
