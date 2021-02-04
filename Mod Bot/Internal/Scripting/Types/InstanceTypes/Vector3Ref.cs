using ModLibrary;
using MoonSharp.Interpreter;
using UnityEngine;

namespace InternalModBot.Scripting
{
	[MoonSharpUserData]
	public class Vector3Ref
	{
		Vector3 _value;
		static Vector3Ref create(Vector3 vector)
		{
			return new Vector3Ref()
			{
				_value = vector
			};
		}

		public double x
		{
			get => _value.x;
			set => _value.x = (float)value;
		}
		public double y
		{
			get => _value.y;
			set => _value.y = (float)value;
		}
		public double z
		{
			get => _value.z;
			set => _value.z = (float)value;
		}

		public Vector3Ref add(Vector3Ref other) => (Vector3)this + (Vector3)other;
		public Vector3Ref subtract(Vector3Ref other) => (Vector3)this - (Vector3)other;

		public Vector3Ref multiply(double other) => (Vector3)this * (float)other;
		public Vector3Ref divide(double other) => (Vector3)this / (float)other;

		public static implicit operator Vector3(Vector3Ref vector3) => vector3._value;
		public static implicit operator Vector3Ref(Vector3 vector3) => create(vector3);
	}

}
