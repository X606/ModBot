using ModLibrary;
using MoonSharp.Interpreter;
using System;

namespace InternalModBot.Proxies
{
    [Proxy(typeof(UnityEngine.Object))]
    class UnityEngine_ObjectProxy
    {
        [MoonSharpHidden]
        UnityEngine.Object _target;

        [MoonSharpHidden]
        public UnityEngine_ObjectProxy(UnityEngine.Object target)
        {
            _target = target;
        }

        public int instanceID => _target.GetInstanceID();

        public bool exists => _target;

        public string name
        {
            get => _target.name;
            set => _target.name = value;
        }

        public void destroy(DynValue delay)
        {
            if (delay != null && delay.IsNotNil() && delay.Type == DataType.Number)
            {
                UnityEngine.Object.Destroy(_target, Convert.ToSingle(delay.Number));
            }
            else
            {
                UnityEngine.Object.Destroy(_target);
            }
        }

        public void destroyImmediate(DynValue allowDestroyingAssets)
        {
            bool allowDestroyingAssetsValue = allowDestroyingAssets != null && allowDestroyingAssets.IsNotNil() && allowDestroyingAssets.Type == DataType.Boolean && allowDestroyingAssets.Boolean;
            UnityEngine.Object.DestroyImmediate(_target, allowDestroyingAssetsValue);
        }

        public override bool Equals(object obj)
        {
            if (obj is UnityEngine_ObjectProxy objectProxy)
                return objectProxy._target == _target;

            if (obj is UnityEngine.Object unityObj)
                return unityObj == _target;

            if (obj is DynValue dynValue)
            {
                if (dynValue.IsNil())
                    return !exists || _target == null;
            }

            return false;
        }
    }
}
