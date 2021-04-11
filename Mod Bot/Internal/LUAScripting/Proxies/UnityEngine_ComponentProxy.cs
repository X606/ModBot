using MoonSharp.Interpreter;
using UnityEngine;

namespace InternalModBot.Proxies
{
    [Proxy(typeof(Component))]
    class UnityEngine_ComponentProxy : UnityEngine_ObjectProxy
    {
        Component _target;
        UnityEngine_GameObjectProxy _gameObjectProxy;

        [MoonSharpHidden]
        public UnityEngine_ComponentProxy(Component target) : base(target)
        {
            _target = target;
            _gameObjectProxy = new UnityEngine_GameObjectProxy(target.gameObject);
        }

        public Transform transform => _target.transform;

        public GameObject gameObject => _target.gameObject;

        public Component getComponent(string typeName)
        {
            return _gameObjectProxy.getComponent(typeName);
        }

        public Component getComponentInChildren(string typeName, bool? includeInactive)
        {
            return _gameObjectProxy.getComponentInChildren(typeName, includeInactive);
        }

        public Component getComponentInParent(string typeName)
        {
            return _gameObjectProxy.getComponentInParent(typeName);
        }

        public Component[] getComponents(string typeName)
        {
            return _gameObjectProxy.getComponents(typeName);
        }

        public Component[] getComponentsInChildren(string typeName)
        {
            return _gameObjectProxy.getComponentsInChildren(typeName);
        }

        public Component[] getComponentsInParent(string typeName)
        {
            return _gameObjectProxy.getComponentsInParent(typeName);
        }

        public string tag
        {
            get => _gameObjectProxy.tag;
            set => _gameObjectProxy.tag = value;
        }

        public bool compareTag(string tag)
        {
            return _gameObjectProxy.compareTag(tag);
        }
    }
}
