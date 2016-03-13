namespace ShaderUnit.Maths
{
	public struct Matrix1x1<T>
	{
		public T m11;

		public Matrix1x1(T m11)
		{
			this.m11 = m11;
		}
	}

	public struct Matrix1x2<T>
	{
		public T m11;
		public T m12;

		public Matrix1x2(T m11, T m12)
		{
			this.m11 = m11;
			this.m12 = m12;
		}
	}

	public struct Matrix1x3<T>
	{
		public T m11;
		public T m12;
		public T m13;

		public Matrix1x3(T m11, T m12, T m13)
		{
			this.m11 = m11;
			this.m12 = m12;
			this.m13 = m13;
		}
	}

	public struct Matrix1x4<T>
	{
		public T m11;
		public T m12;
		public T m13;
		public T m14;

		public Matrix1x4(T m11, T m12, T m13, T m14)
		{
			this.m11 = m11;
			this.m12 = m12;
			this.m13 = m13;
			this.m14 = m14;
		}
	}

	public struct Matrix2x1<T>
	{
		public T m11;
		public T m21;

		public Matrix2x1(T m11, T m21)
		{
			this.m11 = m11;
			this.m21 = m21;
		}
	}

	public struct Matrix2x2<T>
	{
		public T m11;
		public T m12;
		public T m21;
		public T m22;

		public Matrix2x2(T m11, T m12, T m21, T m22)
		{
			this.m11 = m11;
			this.m12 = m12;
			this.m21 = m21;
			this.m22 = m22;
		}
	}

	public struct Matrix2x3<T>
	{
		public T m11;
		public T m12;
		public T m13;
		public T m21;
		public T m22;
		public T m23;

		public Matrix2x3(T m11, T m12, T m13, T m21, T m22, T m23)
		{
			this.m11 = m11;
			this.m12 = m12;
			this.m13 = m13;
			this.m21 = m21;
			this.m22 = m22;
			this.m23 = m23;
		}
	}

	public struct Matrix2x4<T>
	{
		public T m11;
		public T m12;
		public T m13;
		public T m14;
		public T m21;
		public T m22;
		public T m23;
		public T m24;

		public Matrix2x4(T m11, T m12, T m13, T m14, T m21, T m22, T m23, T m24)
		{
			this.m11 = m11;
			this.m12 = m12;
			this.m13 = m13;
			this.m14 = m14;
			this.m21 = m21;
			this.m22 = m22;
			this.m23 = m23;
			this.m24 = m24;
		}
	}

	public struct Matrix3x1<T>
	{
		public T m11;
		public T m21;
		public T m31;

		public Matrix3x1(T m11, T m21, T m31)
		{
			this.m11 = m11;
			this.m21 = m21;
			this.m31 = m31;
		}
	}

	public struct Matrix3x2<T>
	{
		public T m11;
		public T m12;
		public T m21;
		public T m22;
		public T m31;
		public T m32;

		public Matrix3x2(T m11, T m12, T m21, T m22, T m31, T m32)
		{
			this.m11 = m11;
			this.m12 = m12;
			this.m21 = m21;
			this.m22 = m22;
			this.m31 = m31;
			this.m32 = m32;
		}
	}

	public struct Matrix3x3<T>
	{
		public T m11;
		public T m12;
		public T m13;
		public T m21;
		public T m22;
		public T m23;
		public T m31;
		public T m32;
		public T m33;

		public Matrix3x3(T m11, T m12, T m13, T m21, T m22, T m23, T m31, T m32, T m33)
		{
			this.m11 = m11;
			this.m12 = m12;
			this.m13 = m13;
			this.m21 = m21;
			this.m22 = m22;
			this.m23 = m23;
			this.m31 = m31;
			this.m32 = m32;
			this.m33 = m33;
		}
	}

	public struct Matrix3x4<T>
	{
		public T m11;
		public T m12;
		public T m13;
		public T m14;
		public T m21;
		public T m22;
		public T m23;
		public T m24;
		public T m31;
		public T m32;
		public T m33;
		public T m34;

		public Matrix3x4(T m11, T m12, T m13, T m14, T m21, T m22, T m23, T m24, T m31, T m32, T m33, T m34)
		{
			this.m11 = m11;
			this.m12 = m12;
			this.m13 = m13;
			this.m14 = m14;
			this.m21 = m21;
			this.m22 = m22;
			this.m23 = m23;
			this.m24 = m24;
			this.m31 = m31;
			this.m32 = m32;
			this.m33 = m33;
			this.m34 = m34;
		}
	}

	public struct Matrix4x1<T>
	{
		public T m11;
		public T m21;
		public T m31;
		public T m41;

		public Matrix4x1(T m11, T m21, T m31, T m41)
		{
			this.m11 = m11;
			this.m21 = m21;
			this.m31 = m31;
			this.m41 = m41;
		}
	}

	public struct Matrix4x2<T>
	{
		public T m11;
		public T m12;
		public T m21;
		public T m22;
		public T m31;
		public T m32;
		public T m41;
		public T m42;

		public Matrix4x2(T m11, T m12, T m21, T m22, T m31, T m32, T m41, T m42)
		{
			this.m11 = m11;
			this.m12 = m12;
			this.m21 = m21;
			this.m22 = m22;
			this.m31 = m31;
			this.m32 = m32;
			this.m41 = m41;
			this.m42 = m42;
		}
	}

	public struct Matrix4x3<T>
	{
		public T m11;
		public T m12;
		public T m13;
		public T m21;
		public T m22;
		public T m23;
		public T m31;
		public T m32;
		public T m33;
		public T m41;
		public T m42;
		public T m43;

		public Matrix4x3(T m11, T m12, T m13, T m21, T m22, T m23, T m31, T m32, T m33, T m41, T m42, T m43)
		{
			this.m11 = m11;
			this.m12 = m12;
			this.m13 = m13;
			this.m21 = m21;
			this.m22 = m22;
			this.m23 = m23;
			this.m31 = m31;
			this.m32 = m32;
			this.m33 = m33;
			this.m41 = m41;
			this.m42 = m42;
			this.m43 = m43;
		}
	}

	public struct Matrix4x4<T>
	{
		public T m11;
		public T m12;
		public T m13;
		public T m14;
		public T m21;
		public T m22;
		public T m23;
		public T m24;
		public T m31;
		public T m32;
		public T m33;
		public T m34;
		public T m41;
		public T m42;
		public T m43;
		public T m44;

		public Matrix4x4(T m11, T m12, T m13, T m14, T m21, T m22, T m23, T m24, T m31, T m32, T m33, T m34, T m41, T m42, T m43, T m44)
		{
			this.m11 = m11;
			this.m12 = m12;
			this.m13 = m13;
			this.m14 = m14;
			this.m21 = m21;
			this.m22 = m22;
			this.m23 = m23;
			this.m24 = m24;
			this.m31 = m31;
			this.m32 = m32;
			this.m33 = m33;
			this.m34 = m34;
			this.m41 = m41;
			this.m42 = m42;
			this.m43 = m43;
			this.m44 = m44;
		}
	}

}

