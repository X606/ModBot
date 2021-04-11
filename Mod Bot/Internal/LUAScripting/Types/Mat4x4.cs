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
    class Mat4x4
    {
		public double m00;
		public double m10;
		public double m20;
		public double m30;
		public double m01;
		public double m11;
		public double m21;
		public double m31;
		public double m02;
		public double m12;
		public double m22;
		public double m32;
		public double m03;
		public double m13;
		public double m23;
		public double m33;

		public Mat4x4() : this(Vector4.zero, Vector4.zero, Vector4.zero, Vector4.zero)
        {
        }

		public Mat4x4(Vec4 column0, Vec4 column1, Vec4 column2, Vec4 column3)
		{
			m00 = column0.x;
			m01 = column1.x;
			m02 = column2.x;
			m03 = column3.x;
			m10 = column0.y;
			m11 = column1.y;
			m12 = column2.y;
			m13 = column3.y;
			m20 = column0.z;
			m21 = column1.z;
			m22 = column2.z;
			m23 = column3.z;
			m30 = column0.w;
			m31 = column1.w;
			m32 = column2.w;
			m33 = column3.w;
		}

		public Quat rotation => ((Matrix4x4)this).rotation;

		public Vec3 lossyScale => ((Matrix4x4)this).lossyScale;

		public bool isIdentity => ((Matrix4x4)this).isIdentity;

		public double determinant => ((Matrix4x4)this).determinant;

		public FrustumPlanes decomposeProjection => ((Matrix4x4)this).decomposeProjection;

		public bool validTRS => ((Matrix4x4)this).ValidTRS();

		public Mat4x4 inverse => ((Matrix4x4)this).inverse;

		public Mat4x4 transpose => ((Matrix4x4)this).transpose;

		[MoonSharpHidden]
		public double this[int row, int column]
		{
			get
			{
				return this[row + column * 4];
			}
			set
			{
				this[row + column * 4] = value;
			}
		}

		[MoonSharpHidden]
		public double this[int index]
		{
			get
			{
				double result;
				switch (index)
				{
					case 0:
						result = this.m00;
						break;
					case 1:
						result = this.m10;
						break;
					case 2:
						result = this.m20;
						break;
					case 3:
						result = this.m30;
						break;
					case 4:
						result = this.m01;
						break;
					case 5:
						result = this.m11;
						break;
					case 6:
						result = this.m21;
						break;
					case 7:
						result = this.m31;
						break;
					case 8:
						result = this.m02;
						break;
					case 9:
						result = this.m12;
						break;
					case 10:
						result = this.m22;
						break;
					case 11:
						result = this.m32;
						break;
					case 12:
						result = this.m03;
						break;
					case 13:
						result = this.m13;
						break;
					case 14:
						result = this.m23;
						break;
					case 15:
						result = this.m33;
						break;
					default:
						throw new IndexOutOfRangeException("Invalid matrix index!");
				}
				return result;
			}
			set
			{
				switch (index)
				{
					case 0:
						this.m00 = value;
						break;
					case 1:
						this.m10 = value;
						break;
					case 2:
						this.m20 = value;
						break;
					case 3:
						this.m30 = value;
						break;
					case 4:
						this.m01 = value;
						break;
					case 5:
						this.m11 = value;
						break;
					case 6:
						this.m21 = value;
						break;
					case 7:
						this.m31 = value;
						break;
					case 8:
						this.m02 = value;
						break;
					case 9:
						this.m12 = value;
						break;
					case 10:
						this.m22 = value;
						break;
					case 11:
						this.m32 = value;
						break;
					case 12:
						this.m03 = value;
						break;
					case 13:
						this.m13 = value;
						break;
					case 14:
						this.m23 = value;
						break;
					case 15:
						this.m33 = value;
						break;
					default:
						throw new IndexOutOfRangeException("Invalid matrix index!");
				}
			}
		}

		public override bool Equals(object obj)
		{
			return obj is Mat4x4 other && GetColumn(0).Equals(other.GetColumn(0)) && GetColumn(1).Equals(other.GetColumn(1)) && GetColumn(2).Equals(other.GetColumn(2)) && GetColumn(3).Equals(other.GetColumn(3));
		}

		[MoonSharpHidden]
		public static implicit operator Matrix4x4(Mat4x4 mat4x4)
        {
			return new Matrix4x4(new Vector4(Convert.ToSingle(mat4x4.m00), Convert.ToSingle(mat4x4.m10), Convert.ToSingle(mat4x4.m20), Convert.ToSingle(mat4x4.m30)),
								 new Vector4(Convert.ToSingle(mat4x4.m01), Convert.ToSingle(mat4x4.m11), Convert.ToSingle(mat4x4.m21), Convert.ToSingle(mat4x4.m31)),
								 new Vector4(Convert.ToSingle(mat4x4.m02), Convert.ToSingle(mat4x4.m12), Convert.ToSingle(mat4x4.m22), Convert.ToSingle(mat4x4.m32)),
								 new Vector4(Convert.ToSingle(mat4x4.m03), Convert.ToSingle(mat4x4.m13), Convert.ToSingle(mat4x4.m23), Convert.ToSingle(mat4x4.m33)));
        }

		public static implicit operator Mat4x4(Matrix4x4 matrix4x4)
        {
			return new Mat4x4(new Vec4(matrix4x4.m00, matrix4x4.m10, matrix4x4.m20, matrix4x4.m30),
							  new Vec4(matrix4x4.m01, matrix4x4.m11, matrix4x4.m21, matrix4x4.m31),
							  new Vec4(matrix4x4.m02, matrix4x4.m12, matrix4x4.m22, matrix4x4.m32),
							  new Vec4(matrix4x4.m03, matrix4x4.m13, matrix4x4.m23, matrix4x4.m33));
		}

		public static Mat4x4 operator *(Mat4x4 lhs, Mat4x4 rhs)
		{
			Mat4x4 result = new Mat4x4();
			result.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30;
			result.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31;
			result.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32;
			result.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33;
			result.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30;
			result.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31;
			result.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32;
			result.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33;
			result.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30;
			result.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31;
			result.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32;
			result.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33;
			result.m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30;
			result.m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31;
			result.m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32;
			result.m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33;
			return result;
		}

		public static Vec4 operator *(Mat4x4 lhs, Vec4 vector)
		{
			Vec4 result = new Vec4();
			result.x = lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02 * vector.z + lhs.m03 * vector.w;
			result.y = lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12 * vector.z + lhs.m13 * vector.w;
			result.z = lhs.m20 * vector.x + lhs.m21 * vector.y + lhs.m22 * vector.z + lhs.m23 * vector.w;
			result.w = lhs.m30 * vector.x + lhs.m31 * vector.y + lhs.m32 * vector.z + lhs.m33 * vector.w;
			return result;
		}

		public Vec4 GetColumn(int index)
		{
			Vec4 result;
			switch (index)
			{
				case 0:
					result = new Vec4(m00, m10, m20, m30);
					break;
				case 1:
					result = new Vec4(m01, m11, m21, m31);
					break;
				case 2:
					result = new Vec4(m02, m12, m22, m32);
					break;
				case 3:
					result = new Vec4(m03, m13, m23, m33);
					break;
				default:
					throw new IndexOutOfRangeException("Invalid column index!");
			}
			return result;
		}

		public Vec4 GetRow(int index)
		{
			Vec4 result;
			switch (index)
			{
				case 0:
					result = new Vec4(m00, m01, m02, m03);
					break;
				case 1:
					result = new Vec4(m10, m11, m12, m13);
					break;
				case 2:
					result = new Vec4(m20, m21, m22, m23);
					break;
				case 3:
					result = new Vec4(m30, m31, m32, m33);
					break;
				default:
					throw new IndexOutOfRangeException("Invalid row index!");
			}
			return result;
		}

		public void SetColumn(int index, Vec4 column)
		{
			this[0, index] = column.x;
			this[1, index] = column.y;
			this[2, index] = column.z;
			this[3, index] = column.w;
		}

		public void SetRow(int index, Vec4 row)
		{
			this[index, 0] = row.x;
			this[index, 1] = row.y;
			this[index, 2] = row.z;
			this[index, 3] = row.w;
		}

		public Vec3 MultiplyPoint(Vec3 point)
		{
			Vec3 result = new Vec3();
			result.x = this.m00 * point.x + this.m01 * point.y + this.m02 * point.z + this.m03;
			result.y = this.m10 * point.x + this.m11 * point.y + this.m12 * point.z + this.m13;
			result.z = this.m20 * point.x + this.m21 * point.y + this.m22 * point.z + this.m23;
			double num = this.m30 * point.x + this.m31 * point.y + this.m32 * point.z + this.m33;
			num = 1f / num;
			result.x *= num;
			result.y *= num;
			result.z *= num;
			return result;
		}

		public Vec3 MultiplyPoint3x4(Vec3 point)
		{
			Vec3 result = new Vec3();
			result.x = this.m00 * point.x + this.m01 * point.y + this.m02 * point.z + this.m03;
			result.y = this.m10 * point.x + this.m11 * point.y + this.m12 * point.z + this.m13;
			result.z = this.m20 * point.x + this.m21 * point.y + this.m22 * point.z + this.m23;
			return result;
		}

		public Vec3 MultiplyVector(Vec3 vector)
		{
			Vec3 result = new Vec3();
			result.x = this.m00 * vector.x + this.m01 * vector.y + this.m02 * vector.z;
			result.y = this.m10 * vector.x + this.m11 * vector.y + this.m12 * vector.z;
			result.z = this.m20 * vector.x + this.m21 * vector.y + this.m22 * vector.z;
			return result;
		}

		public Plane TransformPlane(Plane plane)
		{
			Mat4x4 inverse = this.inverse;
			double x = plane.normal.x;
			double y = plane.normal.y;
			double z = plane.normal.z;
			double distance = plane.distance;
			double x2 = inverse.m00 * x + inverse.m10 * y + inverse.m20 * z + inverse.m30 * distance;
			double y2 = inverse.m01 * x + inverse.m11 * y + inverse.m21 * z + inverse.m31 * distance;
			double z2 = inverse.m02 * x + inverse.m12 * y + inverse.m22 * z + inverse.m32 * distance;
			double d = inverse.m03 * x + inverse.m13 * y + inverse.m23 * z + inverse.m33 * distance;
			return new Plane(new Vec3(x2, y2, z2), Convert.ToSingle(d));
		}
	}
}
