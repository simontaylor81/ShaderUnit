using System;

namespace ShaderUnit.TestRenderer
{
	static class HlslTypeConversion
	{
		// Get the HLSL type that coresponds to a given CLR type.
		// Supports only certain known types.
		public static string ClrTypeToHlsl(Type type)
		{
			if (type == typeof(float))
			{
				return "float";
			}
			else if (type == typeof(int))
			{
				return "int";
			}
			else if (type == typeof(uint))
			{
				return "uint";
			}
			else if (type == typeof(System.Numerics.Vector2))
			{
				return "float2";
			}
			else if (type == typeof(System.Numerics.Vector3))
			{
				return "float3";
			}
			else if (type == typeof(System.Numerics.Vector4))
			{
				return "float4";
			}
			else if (type == typeof(ShaderUnit.Maths.Vec2<float>))
			{
				return "float2";
			}
			else if (type == typeof(ShaderUnit.Maths.Vec3<float>))
			{
				return "float3";
			}
			else if (type == typeof(ShaderUnit.Maths.Vec4<float>))
			{
				return "float4";
			}
			else if (type == typeof(System.Numerics.Matrix4x4))
			{
				// System.Numerics.Matrix4x4 is row-major in memory, so match that.
				return "row_major float4x4";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix1x1<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float1x1";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix1x2<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float1x2";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix1x3<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float1x3";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix1x4<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float1x4";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix2x1<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float2x1";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix2x2<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float2x2";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix2x3<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float2x3";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix2x4<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float2x4";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix3x1<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float3x1";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix3x2<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float3x2";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix3x3<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float3x3";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix3x4<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float3x4";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix4x1<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float4x1";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix4x2<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float4x2";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix4x3<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float4x3";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix4x4<float>))
			{
				// ShaderUnit.Maths.Matrix4x4 is also row-major in memory, so match that.
				return "row_major float4x4";
			}

			throw new ArgumentException($"Type cannot be converted to HLSL: {type.ToString()}", nameof(type));
		}

		// Get a HLSL type constructor literal for a given CLR object.
		// Supports only certain known types.
		public static string ClrValueToHlslLiteral(object value)
		{
			var type = value.GetType();
			if (type == typeof(float) || type == typeof(int) || type == typeof(uint))
			{
				// Scalars are easy.
				return value.ToString();
			}
			else if (type == typeof(System.Numerics.Vector2))
			{
				var vec = (System.Numerics.Vector2)value;
				return $"float2({vec.X}, {vec.Y})";
			}
			else if (type == typeof(System.Numerics.Vector3))
			{
				var vec = (System.Numerics.Vector3)value;
				return $"float3({vec.X}, {vec.Y}, {vec.Z})";
			}
			else if (type == typeof(System.Numerics.Vector4))
			{
				var vec = (System.Numerics.Vector4)value;
				return $"float4({vec.X}, {vec.Y}, {vec.Z}, {vec.W})";
			}
			else if (type == typeof(ShaderUnit.Maths.Vec2<float>))
			{
				var vec = (ShaderUnit.Maths.Vec2<float>)value;
				return $"float2({vec.x}, {vec.y})";
			}
			else if (type == typeof(ShaderUnit.Maths.Vec3<float>))
			{
				var vec = (ShaderUnit.Maths.Vec3<float>)value;
				return $"float3({vec.x}, {vec.y}, {vec.z})";
			}
			else if (type == typeof(ShaderUnit.Maths.Vec4<float>))
			{
				var vec = (ShaderUnit.Maths.Vec4<float>)value;
				return $"float4({vec.x}, {vec.y}, {vec.z}, {vec.w})";
			}
			else if (type == typeof(System.Numerics.Matrix4x4))
			{
				var m = (System.Numerics.Matrix4x4)value;
				return $"float4x4({m.M11}, {m.M12}, {m.M13}, {m.M14}, {m.M21}, {m.M22}, {m.M23}, {m.M24}, {m.M31}, {m.M32}, {m.M33}, {m.M34}, {m.M41}, {m.M42}, {m.M43}, {m.M44})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix1x1<float>))
			{
				var m = (ShaderUnit.Maths.Matrix1x1<float>)value;
				return $"float1x1({m.m11})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix1x2<float>))
			{
				var m = (ShaderUnit.Maths.Matrix1x2<float>)value;
				return $"float1x2({m.m11}, {m.m12})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix1x3<float>))
			{
				var m = (ShaderUnit.Maths.Matrix1x3<float>)value;
				return $"float1x3({m.m11}, {m.m12}, {m.m13})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix1x4<float>))
			{
				var m = (ShaderUnit.Maths.Matrix1x4<float>)value;
				return $"float1x4({m.m11}, {m.m12}, {m.m13}, {m.m14})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix2x1<float>))
			{
				var m = (ShaderUnit.Maths.Matrix2x1<float>)value;
				return $"float2x1({m.m11}, {m.m21})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix2x2<float>))
			{
				var m = (ShaderUnit.Maths.Matrix2x2<float>)value;
				return $"float2x2({m.m11}, {m.m12}, {m.m21}, {m.m22})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix2x3<float>))
			{
				var m = (ShaderUnit.Maths.Matrix2x3<float>)value;
				return $"float2x3({m.m11}, {m.m12}, {m.m13}, {m.m21}, {m.m22}, {m.m23})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix2x4<float>))
			{
				var m = (ShaderUnit.Maths.Matrix2x4<float>)value;
				return $"float2x4({m.m11}, {m.m12}, {m.m13}, {m.m14}, {m.m21}, {m.m22}, {m.m23}, {m.m24})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix3x1<float>))
			{
				var m = (ShaderUnit.Maths.Matrix3x1<float>)value;
				return $"float3x1({m.m11}, {m.m21}, {m.m31})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix3x2<float>))
			{
				var m = (ShaderUnit.Maths.Matrix3x2<float>)value;
				return $"float3x2({m.m11}, {m.m12}, {m.m21}, {m.m22}, {m.m31}, {m.m32})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix3x3<float>))
			{
				var m = (ShaderUnit.Maths.Matrix3x3<float>)value;
				return $"float3x3({m.m11}, {m.m12}, {m.m13}, {m.m21}, {m.m22}, {m.m23}, {m.m31}, {m.m32}, {m.m33})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix3x4<float>))
			{
				var m = (ShaderUnit.Maths.Matrix3x4<float>)value;
				return $"float3x4({m.m11}, {m.m12}, {m.m13}, {m.m14}, {m.m21}, {m.m22}, {m.m23}, {m.m24}, {m.m31}, {m.m32}, {m.m33}, {m.m34})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix4x1<float>))
			{
				var m = (ShaderUnit.Maths.Matrix4x1<float>)value;
				return $"float4x1({m.m11}, {m.m21}, {m.m31}, {m.m41})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix4x2<float>))
			{
				var m = (ShaderUnit.Maths.Matrix4x2<float>)value;
				return $"float4x2({m.m11}, {m.m12}, {m.m21}, {m.m22}, {m.m31}, {m.m32}, {m.m41}, {m.m42})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix4x3<float>))
			{
				var m = (ShaderUnit.Maths.Matrix4x3<float>)value;
				return $"float4x3({m.m11}, {m.m12}, {m.m13}, {m.m21}, {m.m22}, {m.m23}, {m.m31}, {m.m32}, {m.m33}, {m.m41}, {m.m42}, {m.m43})";
			}
			else if (type == typeof(ShaderUnit.Maths.Matrix4x4<float>))
			{
				var m = (ShaderUnit.Maths.Matrix4x4<float>)value;
				return $"float4x4({m.m11}, {m.m12}, {m.m13}, {m.m14}, {m.m21}, {m.m22}, {m.m23}, {m.m24}, {m.m31}, {m.m32}, {m.m33}, {m.m34}, {m.m41}, {m.m42}, {m.m43}, {m.m44})";
			}

			throw new ArgumentException($"Value cannot be converted to HLSL: {value.ToString()}", nameof(value));
		}
	}
}

