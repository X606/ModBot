using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InternalModBot
{
    [MoonSharpUserData]
    class Quat
    {
        public double x;
        public double y;
        public double z;
        public double w;

        [MoonSharpHidden]
        public Quat(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vec3 eulerAngles
        {
            get => ((Quaternion)this).eulerAngles;
            set
            {
                Quaternion quaternion = Quaternion.Euler(value);
                x = quaternion.x;
                y = quaternion.y;
                z = quaternion.z;
                w = quaternion.w;
            }
        }

        public DynValue GetAngleAxis(Script source)
        {
            ((Quaternion)this).ToAngleAxis(out float angle, out Vector3 axis);
            return DynValue.NewTuple(DynValue.NewNumber(angle), DynValue.FromObject(source, (Vec3)axis));
        }

        public Quat normalized
        {
            get
            {
                double num = Math.Sqrt(StaticLUACallbackFunctions.QuatDot(this, this));
                Quat result;
                if (num < Mathf.Epsilon)
                {
                    result = StaticLUACallbackFunctions.QuatIdentity();
                }
                else
                {
                    result = new Quat(x / num, y / num, z / num, w / num);
                }

                return result;
            }
        }

        [MoonSharpHidden]
        internal static bool IsConsideredEqual(double dotProduct)
        {
            return dotProduct > 0.999999d;
        }

        public override bool Equals(object obj)
        {
            return obj is Quat other && x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
        }

        [MoonSharpHidden]
        public override int GetHashCode()
        {
            int hashCode = -1743314642;
            hashCode = (hashCode * -1521134295) + x.GetHashCode();
            hashCode = (hashCode * -1521134295) + y.GetHashCode();
            hashCode = (hashCode * -1521134295) + z.GetHashCode();
            hashCode = (hashCode * -1521134295) + w.GetHashCode();
            return hashCode;
        }

        [MoonSharpHidden]
        public static implicit operator Quaternion(Quat quat)
        {
            return new Quaternion(Convert.ToSingle(quat.x), Convert.ToSingle(quat.y), Convert.ToSingle(quat.z), Convert.ToSingle(quat.w));
        }

        [MoonSharpHidden]
        public static implicit operator Quat(Quaternion quaternion)
        {
            return new Quat(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }

        public static Quat operator *(Quat lhs, Quat rhs)
        {
            return new Quat((lhs.w * rhs.x) + (lhs.x * rhs.w) + (lhs.y * rhs.z) - (lhs.z * rhs.y), (lhs.w * rhs.y) + (lhs.y * rhs.w) + (lhs.z * rhs.x) - (lhs.x * rhs.z), (lhs.w * rhs.z) + (lhs.z * rhs.w) + (lhs.x * rhs.y) - (lhs.y * rhs.x), (lhs.w * rhs.w) - (lhs.x * rhs.x) - (lhs.y * rhs.y) - (lhs.z * rhs.z));
        }

        public static Vec3 operator *(Quat rotation, Vec3 point)
        {
            double num = rotation.x * 2f;
            double num2 = rotation.y * 2f;
            double num3 = rotation.z * 2f;
            double num4 = rotation.x * num;
            double num5 = rotation.y * num2;
            double num6 = rotation.z * num3;
            double num7 = rotation.x * num2;
            double num8 = rotation.x * num3;
            double num9 = rotation.y * num3;
            double num10 = rotation.w * num;
            double num11 = rotation.w * num2;
            double num12 = rotation.w * num3;

            return new Vec3
            {
                x = ((1f - (num5 + num6)) * point.x) + ((num7 - num12) * point.y) + ((num8 + num11) * point.z),
                y = ((num7 + num12) * point.x) + ((1f - (num4 + num6)) * point.y) + ((num9 - num10) * point.z),
                z = ((num8 - num11) * point.x) + ((num9 + num10) * point.y) + ((1f - (num4 + num5)) * point.z)
            };
        }
    }
}
