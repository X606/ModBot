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
    class LUAColor
    {
        public LUAColor() : this(0d, 0d, 0d, 0d)
        {
        }

        [MoonSharpHidden]
        public LUAColor(double r, double g, double b, double a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public double r;
        public double g;
        public double b;
        public double a;

        public double grayscale => (0.299d * r) + (0.587d * g) + (0.114d * b);

        public LUAColor linear => new LUAColor(Mathf.GammaToLinearSpace(Convert.ToSingle(r)), Mathf.GammaToLinearSpace(Convert.ToSingle(g)), Mathf.GammaToLinearSpace(Convert.ToSingle(b)), a);

        public LUAColor gamma => new LUAColor(Mathf.LinearToGammaSpace(Convert.ToSingle(r)), Mathf.LinearToGammaSpace(Convert.ToSingle(g)), Mathf.LinearToGammaSpace(Convert.ToSingle(b)), a);

        public double maxColorComponent => Math.Max(Math.Max(r, g), b);

        public override bool Equals(object obj)
        {
            return obj is LUAColor otherColor && otherColor.r == r && otherColor.g == g && otherColor.b == b && otherColor.a == a;
        }

        [MoonSharpHidden]
        public override int GetHashCode()
        {
            int hashCode = -490236692;
            hashCode = (hashCode * -1521134295) + r.GetHashCode();
            hashCode = (hashCode * -1521134295) + g.GetHashCode();
            hashCode = (hashCode * -1521134295) + b.GetHashCode();
            hashCode = (hashCode * -1521134295) + a.GetHashCode();
            return hashCode;
        }

        [MoonSharpHidden]
        public static implicit operator Color(LUAColor LuaColor)
        {
            return new Color(Convert.ToSingle(LuaColor.r), Convert.ToSingle(LuaColor.g), Convert.ToSingle(LuaColor.b), Convert.ToSingle(LuaColor.a));
        }

        [MoonSharpHidden]
        public static implicit operator LUAColor(Color color)
        {
            return new LUAColor(color.r, color.g, color.b, color.a);
        }

        public static LUAColor operator +(LUAColor a, LUAColor b)
		{
			return new LUAColor(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
		}

        public static LUAColor operator -(LUAColor a, LUAColor b)
		{
			return new LUAColor(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);
		}

        public static LUAColor operator *(LUAColor a, LUAColor b)
		{
			return new LUAColor(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
		}

        public static LUAColor operator *(LUAColor a, float b)
		{
			return new LUAColor(a.r * b, a.g * b, a.b * b, a.a * b);
		}

        public static LUAColor operator *(float b, LUAColor a)
		{
			return new LUAColor(a.r * b, a.g * b, a.b * b, a.a * b);
		}

        public static LUAColor operator /(LUAColor a, float b)
		{
			return new LUAColor(a.r / b, a.g / b, a.b / b, a.a / b);
		}
	}
}
