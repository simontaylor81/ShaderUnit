using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ShaderUnit.Maths;

namespace ShaderUnitTests
{
	// Tests for the vector and matrix structs.
	public class MathsTests
	{
		[TestCase(1, 2)]
		[TestCase(1.0f, 2.0f)]
		[TestCase(1u, 2u)]
		[TestCase(1.0, 2.0)]
		public void Vec2ConstructorTest<T>(T x, T y) where T : struct
		{
			var vec = new Vec2<T>(x, y);
			Assert.That(vec.x, Is.EqualTo(x));
			Assert.That(vec.y, Is.EqualTo(y));
		}

		[TestCase(1, 2, 3)]
		[TestCase(1.0f, 2.0f, 3.0f)]
		[TestCase(1u, 2u, 3u)]
		[TestCase(1.0, 2.0, 3.0)]
		public void Vec3ConstructorTest<T>(T x, T y, T z) where T : struct
		{
			var vec = new Vec3<T>(x, y, z);
			Assert.That(vec.x, Is.EqualTo(x));
			Assert.That(vec.y, Is.EqualTo(y));
			Assert.That(vec.z, Is.EqualTo(z));
		}

		[TestCase(1, 2, 3, 4)]
		[TestCase(1.0f, 2.0f, 3.0f, 4.0f)]
		[TestCase(1u, 2u, 3u, 4u)]
		[TestCase(1.0, 2.0, 3.0, 4.0)]
		public void Vec4ConstructorTest<T>(T x, T y, T z, T w) where T : struct
		{
			var vec = new Vec4<T>(x, y, z, w);
			Assert.That(vec.x, Is.EqualTo(x));
			Assert.That(vec.y, Is.EqualTo(y));
			Assert.That(vec.z, Is.EqualTo(z));
			Assert.That(vec.w, Is.EqualTo(w));
		}

		[TestCase(1, 2, 3, 4)]
		[TestCase(1.0f, 2.0f, 3.0f, 4.0f)]
		[TestCase(1u, 2u, 3u, 4u)]
		[TestCase(1.0, 2.0, 3.0, 4.0)]
		public void Matrix2x2ConstructorTest<T>(T m11, T m12, T m21, T m22) where T : struct
		{
			var mat = new Matrix2x2<T>(m11, m12, m21, m22);
			Assert.That(mat.m11, Is.EqualTo(m11));
			Assert.That(mat.m12, Is.EqualTo(m12));
			Assert.That(mat.m21, Is.EqualTo(m21));
			Assert.That(mat.m22, Is.EqualTo(m22));
		}
	}
}
