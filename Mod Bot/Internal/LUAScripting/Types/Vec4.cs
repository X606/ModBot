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
    class Vec4
    {
		public double x;
		public double y;
		public double z;
		public double w;

        public Vec4() : this(0d, 0d, 0d, 0d)
        {
        }

        public Vec4(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public void Scale(Vec4 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
            w *= scale.w;
        }

        public override bool Equals(object obj)
        {
            return obj is Vec4 other && x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
        }

        public Vec4 normalized
        {
            get
            {
                double num = magnitude;
                Vec4 result;
                if (num > 1E-05f)
                {
                    result = this / num;
                }
                else
                {
                    result = new Vec4();
                }
                return result;
            }
        }

        public double magnitude => Math.Sqrt(StaticLUACallbackFunctions.Vec4Dot(this, this));

        public double sqrMagnitude => StaticLUACallbackFunctions.Vec4Dot(this, this);

        [MoonSharpHidden]
        public static implicit operator Vector4(Vec4 vec4)
        {
            return new Vector4(Convert.ToSingle(vec4.x), Convert.ToSingle(vec4.y), Convert.ToSingle(vec4.z), Convert.ToSingle(vec4.w));
        }

        [MoonSharpHidden]
        public static implicit operator Vec4(Vector4 vector4)
        {
            return new Vec4(vector4.x, vector4.y, vector4.z, vector4.w);
        }

        public static Vec4 operator +(Vec4 a, Vec4 b)
        {
            return new Vec4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        public static Vec4 operator -(Vec4 a, Vec4 b)
        {
            return new Vec4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        public static Vec4 operator -(Vec4 a)
        {
            return new Vec4(-a.x, -a.y, -a.z, -a.w);
        }

        public static Vec4 operator *(Vec4 a, double d)
        {
            return new Vec4(a.x * d, a.y * d, a.z * d, a.w * d);
        }

        public static Vec4 operator *(double d, Vec4 a)
        {
            return new Vec4(a.x * d, a.y * d, a.z * d, a.w * d);
        }

        public static Vec4 operator /(Vec4 a, double d)
        {
            return new Vec4(a.x / d, a.y / d, a.z / d, a.w / d);
        }
    }
}
