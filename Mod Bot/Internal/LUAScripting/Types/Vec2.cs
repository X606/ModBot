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
    class Vec2
    {
        [MoonSharpHidden]
        public Vec2() : this(0d, 0d)
        {
        }

        [MoonSharpHidden]
        public Vec2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double x;
        public double y;

        public double magnitude => Math.Sqrt((x * x) + (y * y));

        public double sqrMagnitude => (x * x) + (y * y);

        public Vec2 normalized
        {
            get
            {
                double mag = magnitude;
                Vec2 result;
                if (mag > 1E-05f)
                {
                    result = this / mag;
                }
                else
                {
                    result = new Vec2(0, 0);
                }
                return result;
            }
        }

        public void Scale(Vec2 scale)
        {
            x *= scale.x;
            y *= scale.y;
        }

        public override bool Equals(object obj)
        {
            return obj is Vec2 vec && x.Equals(vec.x) && y.Equals(vec.y);
        }

        [MoonSharpHidden]
        public override int GetHashCode()
        {
            int hashCode = 373119288;
            hashCode = (hashCode * -1521134295) + x.GetHashCode();
            hashCode = (hashCode * -1521134295) + y.GetHashCode();
            return hashCode;
        }

        [MoonSharpHidden]
        public static implicit operator Vector2(Vec2 Vec2)
        {
            return new Vector2(Convert.ToSingle(Vec2.x), Convert.ToSingle(Vec2.y));
        }

        [MoonSharpHidden]
        public static implicit operator Vec2(Vector2 vector3)
        {
            return new Vec2(vector3.x, vector3.y);
        }

        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x + b.x, a.y + b.y);
        }

        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x - b.x, a.y - b.y);
        }

        public static Vec2 operator -(Vec2 a)
        {
            return new Vec2(-a.x, -a.y);
        }

        public static Vec2 operator *(Vec2 a, double d)
        {
            return new Vec2(a.x * d, a.y * d);
        }

        public static Vec2 operator *(double d, Vec2 a)
        {
            return new Vec2(a.x * d, a.y * d);
        }

        public static Vec2 operator /(Vec2 a, double d)
        {
            return new Vec2(a.x / d, a.y / d);
        }
    }
}
