using MoonSharp.Interpreter;
using System.Collections;
using UnityEngine;

namespace InternalModBot.Proxies
{
    [Proxy(typeof(Transform))]
    class UnityEngine_TransformProxy : UnityEngine_ComponentProxy, IEnumerable
    {
        [MoonSharpHidden]
        Transform _target;

        [MoonSharpHidden]
        public UnityEngine_TransformProxy(Transform target) : base(target)
        {
            _target = target;
        }

        public Vec3 position
        {
            get => _target.position;
            set => _target.position = value;
        }

        public Vec3 localPosition
        {
            get => _target.localPosition;
            set => _target.localPosition = value;
        }

        public Vec3 eulerAngles
        {
            get => _target.eulerAngles;
            set => _target.eulerAngles = value;
        }

        public Vec3 localEulerAngles
        {
            get => _target.localEulerAngles;
            set => _target.localEulerAngles = value;
        }

        public Vec3 right
        {
            get => _target.right;
            set => _target.right = value;
        }
        public Vec3 left
        {
            get => -_target.right;
            set => _target.right = -value;
        }

        public Vec3 up
        {
            get => _target.up;
            set => _target.up = value;
        }
        public Vec3 down
        {
            get => -_target.up;
            set => _target.up = -value;
        }

        public Vec3 forward
        {
            get => _target.forward;
            set => _target.forward = value;
        }
        public Vec3 backward
        {
            get => -_target.forward;
            set => _target.forward = -value;
        }

        public Quat rotation
        {
            get => _target.rotation;
            set => _target.rotation = value;
        }
        public Quat localRotation
        {
            get => _target.localRotation;
            set => _target.localRotation = value;
        }

        public Vec3 localScale
        {
            get => _target.localScale;
            set => _target.localScale = value;
        }

        public Transform parent
        {
            get => _target.parent;
        }
        public void setParent(Transform parent, DynValue worldPositionStays)
        {
            bool worldPositionStaysValue = true;
            if (worldPositionStays != null && worldPositionStays.IsNotNil() && worldPositionStays.Type == DataType.Boolean)
                worldPositionStaysValue = worldPositionStays.Boolean;

            _target.SetParent(parent, worldPositionStaysValue);
        }

        public Mat4x4 worldToLocalMatrix => _target.worldToLocalMatrix;
        public Mat4x4 localToWorldMatrix => _target.localToWorldMatrix;

        public void translateWorld(Vec3 translation)
        {
            position += translation;
        }
        public void translateLocal(Vec3 translation)
        {
            position += transformDirection(translation);
        }
        public void translateRelative(Vec3 translation, Transform relativeTo)
        {
            position += (Vec3)relativeTo.TransformDirection(translation);
        }

        public void rotateWorld(Vec3 eulers)
        {
            Quat q = StaticLUACallbackFunctions.QuatEuler(eulers);
            rotation *= StaticLUACallbackFunctions.QuatInverse(rotation) * q * rotation;
        }
        public void rotateLocal(Vec3 eulers)
        {
            Quat q = StaticLUACallbackFunctions.QuatEuler(eulers);
            localRotation *= q;
        }

        public void rotateAxisWorld(Vec3 axis, float angle)
        {
            _target.Rotate(axis, angle, Space.World);
        }
        public void rotateAxisLocal(Vec3 axis, float angle)
        {
            _target.Rotate(axis, angle, Space.Self);
        }

        public void rotateAround(Vec3 point, Vec3 axis, float angle)
        {
            _target.RotateAround(point, axis, angle);
        }

        public void lookAt(Transform target, Vec3 up)
        {
            if (up == null)
                up = Vector3.up;

            _target.LookAt(target, up);
        }
        public void lookAt(Vec3 worldPosition, Vec3 up)
        {
            if (up == null)
                up = Vector3.up;

            _target.LookAt(worldPosition, up);
        }

        public Vec3 transformDirection(Vec3 direction)
        {
            return _target.TransformDirection(direction);
        }
        public Vec3 inverseTransformDirection(Vec3 direction)
        {
            return _target.InverseTransformDirection(direction);
        }

        public Vec3 transformVector(Vec3 vector)
        {
            return _target.TransformVector(vector);
        }
        public Vec3 inverseTransformVector(Vec3 vector)
        {
            return _target.InverseTransformVector(vector);
        }

        public Vec3 transformPoint(Vec3 position)
        {
            return _target.TransformPoint(position);
        }
        public Vec3 inverseTransformPoint(Vec3 position)
        {
            return _target.InverseTransformPoint(position);
        }

        public Transform root => _target.root;

        public int childCount => _target.childCount;

        public void detatchChildren()
        {
            _target.DetachChildren();
        }

        public void setAsFirstSibling()
        {
            _target.SetAsFirstSibling();
        }
        public void setAsLastSibling()
        {
            _target.SetAsLastSibling();
        }

        public int siblingIndex
        {
            get => _target.GetSiblingIndex();
            set => _target.SetSiblingIndex(value);
        }

        public Transform FindChild(string name)
        {
            return _target.Find(name);
        }

        public Vec3 lossyScale => _target.lossyScale;

        public bool isChildOf(Transform other)
        {
            return _target.IsChildOf(other);
        }

        public bool hasChanged
        {
            get => _target.hasChanged;
            set => _target.hasChanged = value;
        }

        public IEnumerator GetEnumerator()
        {
            return new Enumerator(_target);
        }

        public Transform getChildAtIndex(int index)
        {
            return _target.GetChild(index);
        }

        public int hierarchyCapacity
        {
            get => _target.hierarchyCapacity;
            set => _target.hierarchyCapacity = value;
        }

        public int hierarchyCount => _target.hierarchyCount;

        private class Enumerator : IEnumerator
        {
            Transform outer;
            int currentIndex = -1;

            internal Enumerator(Transform outer)
            {
                this.outer = outer;
            }

            public object Current
            {
                get
                {
                    return outer.GetChild(currentIndex);
                }
            }

            public bool MoveNext()
            {
                int childCount = outer.childCount;
                return ++currentIndex < childCount;
            }

            public void Reset()
            {
                currentIndex = -1;
            }
        }
    }
}
