using MoonSharp.Interpreter;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InternalModBot.Proxies
{
    [Proxy(typeof(GameObject))]
    class UnityEngine_GameObjectProxy : UnityEngine_ObjectProxy
    {
        [MoonSharpHidden]
        GameObject _target;

        [MoonSharpHidden]
        public UnityEngine_GameObjectProxy(GameObject target) : base(target)
        {
            _target = target;
        }

        public Component getComponent(string typeName)
        {
            Type componentType = InternalUtils.FindComponentType(typeName, true);

            if (componentType == null)
            {
                return null;
            }
            else
            {
                return _target.GetComponent(componentType);
            }
        }

        public Component getComponentInChildren(string typeName, bool? includeInactive)
        {
            Type componentType = InternalUtils.FindComponentType(typeName, true);

            if (componentType == null)
            {
                return null;
            }
            else
            {
                return _target.GetComponentInChildren(componentType, includeInactive ?? false);
            }
        }

        public Component getComponentInParent(string typeName)
        {
            Type componentType = InternalUtils.FindComponentType(typeName, true);

            if (componentType == null)
            {
                return null;
            }
            else
            {
                return _target.GetComponentInParent(componentType);
            }
        }

        public Component[] getComponents(string typeName)
        {
            Type componentType = InternalUtils.FindComponentType(typeName, true);

            if (componentType == null)
            {
                return null;
            }
            else
            {
                return _target.GetComponents(componentType);
            }
        }

        public Component[] getComponentsInChildren(string typeName)
        {
            Type componentType = InternalUtils.FindComponentType(typeName, true);

            if (componentType == null)
            {
                return null;
            }
            else
            {
                return _target.GetComponentsInChildren(componentType);
            }
        }

        public Component[] getComponentsInParent(string typeName)
        {
            Type componentType = InternalUtils.FindComponentType(typeName, true);

            if (componentType == null)
            {
                return null;
            }
            else
            {
                return _target.GetComponentsInParent(componentType);
            }
        }

        public void sendMessageUpwards(string methodName)
        {
            _target.SendMessageUpwards(methodName);
        }
        public void sendMessage(string methodName)
        {
            _target.SendMessage(methodName);
        }
        public void broadcastMessage(string methodName)
        {
            _target.BroadcastMessage(methodName);
        }

        public Component addComponent(string typeName)
        {
            Type componentType = InternalUtils.FindComponentType(typeName, true);

            if (componentType == null)
            {
                return null;
            }
            else
            {
                return _target.AddComponent(componentType);
            }
        }

        public Transform transform => _target.transform;

        public int layer => _target.layer;

        public void setActive(bool value)
        {
            _target.SetActive(value);
        }

        public bool activeSelf => _target.activeSelf;

        public bool activeInHierarchy => _target.activeInHierarchy;

        public bool isStatic
        {
            get => _target.isStatic;
            set => _target.isStatic = value;
        }

        public string tag
        {
            get => _target.tag;
            set => _target.tag = value;
        }

        public bool compareTag(string tag)
        {
            return _target.CompareTag(tag);
        }

        public Scene scene => _target.scene;

        public GameObject gameObject => _target.gameObject;
    }
}
