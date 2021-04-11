using Esprima.Ast;
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
    class Vec3
    {
        [MoonSharpHidden]
        public Vec3() : this(0d, 0d, 0d)
        {
        }

        [MoonSharpHidden]
        public Vec3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double x;
        public double y;
        public double z;

        public double magnitude => Math.Sqrt((x * x) + (y * y) + (z * z));

        public double sqrMagnitude => (x * x) + (y * y) + (z * z);

        public Vec3 normalized
        {
            get
            {
                double mag = magnitude;
                Vec3 result;
                if (mag > 1E-05f)
                {
                    result = this / mag;
                }
                else
                {
                    result = new Vec3(0, 0, 0);
                }
                return result;
            }
        }

        public void Scale(Vec3 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }

        public override bool Equals(object obj)
        {
            return obj is Vec3 vec && x.Equals(vec.x) && y.Equals(vec.y) && z.Equals(vec.z);
        }

        [MoonSharpHidden]
        public override int GetHashCode()
        {
            int hashCode = 373119288;
            hashCode = (hashCode * -1521134295) + x.GetHashCode();
            hashCode = (hashCode * -1521134295) + y.GetHashCode();
            hashCode = (hashCode * -1521134295) + z.GetHashCode();
            return hashCode;
        }

        [MoonSharpHidden]
        public static implicit operator Vector3(Vec3 vec3)
        {
            return new Vector3(Convert.ToSingle(vec3.x), Convert.ToSingle(vec3.y), Convert.ToSingle(vec3.z));
        }

        [MoonSharpHidden]
        public static implicit operator Vec3(Vector3 vector3)
        {
            return new Vec3(vector3.x, vector3.y, vector3.z);
        }
        
        public static Vec3 operator +(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vec3 operator -(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vec3 operator -(Vec3 a)
        {
            return new Vec3(-a.x, -a.y, -a.z);
        }

        public static Vec3 operator *(Vec3 a, double d)
        {
            return new Vec3(a.x * d, a.y * d, a.z * d);
        }

        public static Vec3 operator *(double d, Vec3 a)
        {
            return new Vec3(a.x * d, a.y * d, a.z * d);
        }

        public static Vec3 operator /(Vec3 a, double d)
        {
            return new Vec3(a.x / d, a.y / d, a.z / d);
        }
    }
}
