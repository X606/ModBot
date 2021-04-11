using ModLibrary;
using MoonSharp.Interpreter;
using UnityEngine;

namespace InternalModBot.Scripting
{
	/// <summary>
	/// A wrapper for the <see cref="Vector3"/> class
	/// </summary>
	//[MoonSharpUserData]
	internal class Vector3Ref
	{
		Vector3 _value;
		static Vector3Ref create(Vector3 vector)
		{
			return new Vector3Ref()
			{
				_value = vector
			};
		}

		/// <summary>
		/// The X part of the Vector
		/// </summary>
		public double x
		{
			get => _value.x;
			set => _value.x = (float)value;
		}
		/// <summary>
		/// The Y part of the vector
		/// </summary>
		public double y
		{
			get => _value.y;
			set => _value.y = (float)value;
		}
		/// <summary>
		/// The Z part of the vector
		/// </summary>
		public double z
		{
			get => _value.z;
			set => _value.z = (float)value;
		}

		/// <summary>
		/// Adds another vector3 to this one
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Vector3Ref add(Vector3Ref other) => (Vector3)this + (Vector3)other;
		/// <summary>
		/// Subtracts another vector3 from this one
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Vector3Ref subtract(Vector3Ref other) => (Vector3)this - (Vector3)other;

		/// <summary>
		/// Mutliplies this vector by a number
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Vector3Ref multiply(double other) => (Vector3)this * (float)other;
		/// <summary>
		/// Divides this vector by a number
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Vector3Ref divide(double other) => (Vector3)this / (float)other;

		/// <summary>
		/// Allows us to implicitly convert this wrapper to a <see cref="Vector3"/>
		/// </summary>
		/// <param name="vector3"></param>
		public static implicit operator Vector3(Vector3Ref vector3) => vector3._value;
		/// <summary>
		/// Allows us to implicitly convert a <see cref="Vector3"/> to our wrapper
		/// </summary>
		/// <param name="vector3"></param>
		public static implicit operator Vector3Ref(Vector3 vector3) => create(vector3);
	}

}
