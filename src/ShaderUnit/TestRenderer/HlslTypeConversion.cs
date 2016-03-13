using System;
using System.Collections.Generic;

namespace ShaderUnit.TestRenderer
{
	static class HlslTypeConversion
	{
		// Get the HLSL type that coresponds to a given CLR type.
		// Supports only certain known types.
		public static string ClrTypeToHlsl(Type type)
		{
			HlslTypeInfo typeInfo;
			if (_types.TryGetValue(type, out typeInfo))
			{
				return typeInfo.hlslType;
			}

			throw new ArgumentException($"Type cannot be converted to HLSL: {type.ToString()}", nameof(type));
		}

		// Get a HLSL type constructor literal for a given CLR object.
		// Supports only certain known types.
		public static string ClrValueToHlslLiteral(object value)
		{
			HlslTypeInfo typeInfo;
			if (_types.TryGetValue(value.GetType(), out typeInfo))
			{
				return typeInfo.makeLiteral(value);
			}

			throw new ArgumentException($"Value cannot be converted to HLSL: {value.ToString()}", nameof(value));
		}

		// Initialise type lookup table.
		static HlslTypeConversion()
		{
			// Scalars.
			AddTypeInfo(typeof(float), "float", value => value.ToString());
			AddTypeInfo(typeof(int), "int", value => value.ToString());
			AddTypeInfo(typeof(uint), "uint", value => value.ToString());

			// System.Numerics Vectors
			AddTypeInfo(typeof(System.Numerics.Vector2), "float2", value =>
			{
				var vec = (System.Numerics.Vector2)value;
				return $"float2({vec.X}, {vec.Y})";
			});
			AddTypeInfo(typeof(System.Numerics.Vector3), "float3", value =>
			{
				var vec = (System.Numerics.Vector3)value;
				return $"float3({vec.X}, {vec.Y}, {vec.Z})";
			});
			AddTypeInfo(typeof(System.Numerics.Vector4), "float4", value =>
			{
				var vec = (System.Numerics.Vector4)value;
				return $"float4({vec.X}, {vec.Y}, {vec.Z}, {vec.W})";
			});

			// ShaderUnit.Maths Vec types
			AddTypeInfo(typeof(ShaderUnit.Maths.Vec2<float>), "float2", value =>
			{
				var vec = (ShaderUnit.Maths.Vec2<float>)value;
				return $"float2({vec.x}, {vec.y})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Vec3<float>), "float3", value =>
			{
				var vec = (ShaderUnit.Maths.Vec3<float>)value;
				return $"float3({vec.x}, {vec.y}, {vec.z})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Vec4<float>), "float4", value =>
			{
				var vec = (ShaderUnit.Maths.Vec4<float>)value;
				return $"float4({vec.x}, {vec.y}, {vec.z}, {vec.w})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Vec2<int>), "int2", value =>
			{
				var vec = (ShaderUnit.Maths.Vec2<int>)value;
				return $"int2({vec.x}, {vec.y})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Vec3<int>), "int3", value =>
			{
				var vec = (ShaderUnit.Maths.Vec3<int>)value;
				return $"int3({vec.x}, {vec.y}, {vec.z})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Vec4<int>), "int4", value =>
			{
				var vec = (ShaderUnit.Maths.Vec4<int>)value;
				return $"int4({vec.x}, {vec.y}, {vec.z}, {vec.w})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Vec2<uint>), "uint2", value =>
			{
				var vec = (ShaderUnit.Maths.Vec2<uint>)value;
				return $"uint2({vec.x}, {vec.y})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Vec3<uint>), "uint3", value =>
			{
				var vec = (ShaderUnit.Maths.Vec3<uint>)value;
				return $"uint3({vec.x}, {vec.y}, {vec.z})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Vec4<uint>), "uint4", value =>
			{
				var vec = (ShaderUnit.Maths.Vec4<uint>)value;
				return $"uint4({vec.x}, {vec.y}, {vec.z}, {vec.w})";
			});

			// System.Numerics.Matrix4x4
			// System.Numerics.Matrix4x4 is row-major in memory, so match that.
			AddTypeInfo(typeof(System.Numerics.Matrix4x4), "row_major float4x4", value =>
			{
				var m = (System.Numerics.Matrix4x4)value;
				return $"float4x4({m.M11}, {m.M12}, {m.M13}, {m.M14}, {m.M21}, {m.M22}, {m.M23}, {m.M24}, {m.M31}, {m.M32}, {m.M33}, {m.M34}, {m.M41}, {m.M42}, {m.M43}, {m.M44})";
			});

			// ShaderUnit.Maths Matrix types.
			// ShaderUnit.Maths Matrix types are also row-major in memory, so match that here too.
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix1x1<float>), "row_major float1x1", value =>
			{
				var m = (ShaderUnit.Maths.Matrix1x1<float>)value;
				return $"float1x1({m.m11})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix1x2<float>), "row_major float1x2", value =>
			{
				var m = (ShaderUnit.Maths.Matrix1x2<float>)value;
				return $"float1x2({m.m11}, {m.m12})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix1x3<float>), "row_major float1x3", value =>
			{
				var m = (ShaderUnit.Maths.Matrix1x3<float>)value;
				return $"float1x3({m.m11}, {m.m12}, {m.m13})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix1x4<float>), "row_major float1x4", value =>
			{
				var m = (ShaderUnit.Maths.Matrix1x4<float>)value;
				return $"float1x4({m.m11}, {m.m12}, {m.m13}, {m.m14})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix2x1<float>), "row_major float2x1", value =>
			{
				var m = (ShaderUnit.Maths.Matrix2x1<float>)value;
				return $"float2x1({m.m11}, {m.m21})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix2x2<float>), "row_major float2x2", value =>
			{
				var m = (ShaderUnit.Maths.Matrix2x2<float>)value;
				return $"float2x2({m.m11}, {m.m12}, {m.m21}, {m.m22})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix2x3<float>), "row_major float2x3", value =>
			{
				var m = (ShaderUnit.Maths.Matrix2x3<float>)value;
				return $"float2x3({m.m11}, {m.m12}, {m.m13}, {m.m21}, {m.m22}, {m.m23})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix2x4<float>), "row_major float2x4", value =>
			{
				var m = (ShaderUnit.Maths.Matrix2x4<float>)value;
				return $"float2x4({m.m11}, {m.m12}, {m.m13}, {m.m14}, {m.m21}, {m.m22}, {m.m23}, {m.m24})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix3x1<float>), "row_major float3x1", value =>
			{
				var m = (ShaderUnit.Maths.Matrix3x1<float>)value;
				return $"float3x1({m.m11}, {m.m21}, {m.m31})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix3x2<float>), "row_major float3x2", value =>
			{
				var m = (ShaderUnit.Maths.Matrix3x2<float>)value;
				return $"float3x2({m.m11}, {m.m12}, {m.m21}, {m.m22}, {m.m31}, {m.m32})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix3x3<float>), "row_major float3x3", value =>
			{
				var m = (ShaderUnit.Maths.Matrix3x3<float>)value;
				return $"float3x3({m.m11}, {m.m12}, {m.m13}, {m.m21}, {m.m22}, {m.m23}, {m.m31}, {m.m32}, {m.m33})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix3x4<float>), "row_major float3x4", value =>
			{
				var m = (ShaderUnit.Maths.Matrix3x4<float>)value;
				return $"float3x4({m.m11}, {m.m12}, {m.m13}, {m.m14}, {m.m21}, {m.m22}, {m.m23}, {m.m24}, {m.m31}, {m.m32}, {m.m33}, {m.m34})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix4x1<float>), "row_major float4x1", value =>
			{
				var m = (ShaderUnit.Maths.Matrix4x1<float>)value;
				return $"float4x1({m.m11}, {m.m21}, {m.m31}, {m.m41})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix4x2<float>), "row_major float4x2", value =>
			{
				var m = (ShaderUnit.Maths.Matrix4x2<float>)value;
				return $"float4x2({m.m11}, {m.m12}, {m.m21}, {m.m22}, {m.m31}, {m.m32}, {m.m41}, {m.m42})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix4x3<float>), "row_major float4x3", value =>
			{
				var m = (ShaderUnit.Maths.Matrix4x3<float>)value;
				return $"float4x3({m.m11}, {m.m12}, {m.m13}, {m.m21}, {m.m22}, {m.m23}, {m.m31}, {m.m32}, {m.m33}, {m.m41}, {m.m42}, {m.m43})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix4x4<float>), "row_major float4x4", value =>
			{
				var m = (ShaderUnit.Maths.Matrix4x4<float>)value;
				return $"float4x4({m.m11}, {m.m12}, {m.m13}, {m.m14}, {m.m21}, {m.m22}, {m.m23}, {m.m24}, {m.m31}, {m.m32}, {m.m33}, {m.m34}, {m.m41}, {m.m42}, {m.m43}, {m.m44})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix1x1<int>), "row_major int1x1", value =>
			{
				var m = (ShaderUnit.Maths.Matrix1x1<int>)value;
				return $"int1x1({m.m11})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix1x2<int>), "row_major int1x2", value =>
			{
				var m = (ShaderUnit.Maths.Matrix1x2<int>)value;
				return $"int1x2({m.m11}, {m.m12})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix1x3<int>), "row_major int1x3", value =>
			{
				var m = (ShaderUnit.Maths.Matrix1x3<int>)value;
				return $"int1x3({m.m11}, {m.m12}, {m.m13})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix1x4<int>), "row_major int1x4", value =>
			{
				var m = (ShaderUnit.Maths.Matrix1x4<int>)value;
				return $"int1x4({m.m11}, {m.m12}, {m.m13}, {m.m14})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix2x1<int>), "row_major int2x1", value =>
			{
				var m = (ShaderUnit.Maths.Matrix2x1<int>)value;
				return $"int2x1({m.m11}, {m.m21})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix2x2<int>), "row_major int2x2", value =>
			{
				var m = (ShaderUnit.Maths.Matrix2x2<int>)value;
				return $"int2x2({m.m11}, {m.m12}, {m.m21}, {m.m22})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix2x3<int>), "row_major int2x3", value =>
			{
				var m = (ShaderUnit.Maths.Matrix2x3<int>)value;
				return $"int2x3({m.m11}, {m.m12}, {m.m13}, {m.m21}, {m.m22}, {m.m23})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix2x4<int>), "row_major int2x4", value =>
			{
				var m = (ShaderUnit.Maths.Matrix2x4<int>)value;
				return $"int2x4({m.m11}, {m.m12}, {m.m13}, {m.m14}, {m.m21}, {m.m22}, {m.m23}, {m.m24})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix3x1<int>), "row_major int3x1", value =>
			{
				var m = (ShaderUnit.Maths.Matrix3x1<int>)value;
				return $"int3x1({m.m11}, {m.m21}, {m.m31})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix3x2<int>), "row_major int3x2", value =>
			{
				var m = (ShaderUnit.Maths.Matrix3x2<int>)value;
				return $"int3x2({m.m11}, {m.m12}, {m.m21}, {m.m22}, {m.m31}, {m.m32})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix3x3<int>), "row_major int3x3", value =>
			{
				var m = (ShaderUnit.Maths.Matrix3x3<int>)value;
				return $"int3x3({m.m11}, {m.m12}, {m.m13}, {m.m21}, {m.m22}, {m.m23}, {m.m31}, {m.m32}, {m.m33})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix3x4<int>), "row_major int3x4", value =>
			{
				var m = (ShaderUnit.Maths.Matrix3x4<int>)value;
				return $"int3x4({m.m11}, {m.m12}, {m.m13}, {m.m14}, {m.m21}, {m.m22}, {m.m23}, {m.m24}, {m.m31}, {m.m32}, {m.m33}, {m.m34})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix4x1<int>), "row_major int4x1", value =>
			{
				var m = (ShaderUnit.Maths.Matrix4x1<int>)value;
				return $"int4x1({m.m11}, {m.m21}, {m.m31}, {m.m41})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix4x2<int>), "row_major int4x2", value =>
			{
				var m = (ShaderUnit.Maths.Matrix4x2<int>)value;
				return $"int4x2({m.m11}, {m.m12}, {m.m21}, {m.m22}, {m.m31}, {m.m32}, {m.m41}, {m.m42})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix4x3<int>), "row_major int4x3", value =>
			{
				var m = (ShaderUnit.Maths.Matrix4x3<int>)value;
				return $"int4x3({m.m11}, {m.m12}, {m.m13}, {m.m21}, {m.m22}, {m.m23}, {m.m31}, {m.m32}, {m.m33}, {m.m41}, {m.m42}, {m.m43})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix4x4<int>), "row_major int4x4", value =>
			{
				var m = (ShaderUnit.Maths.Matrix4x4<int>)value;
				return $"int4x4({m.m11}, {m.m12}, {m.m13}, {m.m14}, {m.m21}, {m.m22}, {m.m23}, {m.m24}, {m.m31}, {m.m32}, {m.m33}, {m.m34}, {m.m41}, {m.m42}, {m.m43}, {m.m44})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix1x1<uint>), "row_major uint1x1", value =>
			{
				var m = (ShaderUnit.Maths.Matrix1x1<uint>)value;
				return $"uint1x1({m.m11})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix1x2<uint>), "row_major uint1x2", value =>
			{
				var m = (ShaderUnit.Maths.Matrix1x2<uint>)value;
				return $"uint1x2({m.m11}, {m.m12})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix1x3<uint>), "row_major uint1x3", value =>
			{
				var m = (ShaderUnit.Maths.Matrix1x3<uint>)value;
				return $"uint1x3({m.m11}, {m.m12}, {m.m13})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix1x4<uint>), "row_major uint1x4", value =>
			{
				var m = (ShaderUnit.Maths.Matrix1x4<uint>)value;
				return $"uint1x4({m.m11}, {m.m12}, {m.m13}, {m.m14})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix2x1<uint>), "row_major uint2x1", value =>
			{
				var m = (ShaderUnit.Maths.Matrix2x1<uint>)value;
				return $"uint2x1({m.m11}, {m.m21})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix2x2<uint>), "row_major uint2x2", value =>
			{
				var m = (ShaderUnit.Maths.Matrix2x2<uint>)value;
				return $"uint2x2({m.m11}, {m.m12}, {m.m21}, {m.m22})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix2x3<uint>), "row_major uint2x3", value =>
			{
				var m = (ShaderUnit.Maths.Matrix2x3<uint>)value;
				return $"uint2x3({m.m11}, {m.m12}, {m.m13}, {m.m21}, {m.m22}, {m.m23})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix2x4<uint>), "row_major uint2x4", value =>
			{
				var m = (ShaderUnit.Maths.Matrix2x4<uint>)value;
				return $"uint2x4({m.m11}, {m.m12}, {m.m13}, {m.m14}, {m.m21}, {m.m22}, {m.m23}, {m.m24})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix3x1<uint>), "row_major uint3x1", value =>
			{
				var m = (ShaderUnit.Maths.Matrix3x1<uint>)value;
				return $"uint3x1({m.m11}, {m.m21}, {m.m31})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix3x2<uint>), "row_major uint3x2", value =>
			{
				var m = (ShaderUnit.Maths.Matrix3x2<uint>)value;
				return $"uint3x2({m.m11}, {m.m12}, {m.m21}, {m.m22}, {m.m31}, {m.m32})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix3x3<uint>), "row_major uint3x3", value =>
			{
				var m = (ShaderUnit.Maths.Matrix3x3<uint>)value;
				return $"uint3x3({m.m11}, {m.m12}, {m.m13}, {m.m21}, {m.m22}, {m.m23}, {m.m31}, {m.m32}, {m.m33})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix3x4<uint>), "row_major uint3x4", value =>
			{
				var m = (ShaderUnit.Maths.Matrix3x4<uint>)value;
				return $"uint3x4({m.m11}, {m.m12}, {m.m13}, {m.m14}, {m.m21}, {m.m22}, {m.m23}, {m.m24}, {m.m31}, {m.m32}, {m.m33}, {m.m34})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix4x1<uint>), "row_major uint4x1", value =>
			{
				var m = (ShaderUnit.Maths.Matrix4x1<uint>)value;
				return $"uint4x1({m.m11}, {m.m21}, {m.m31}, {m.m41})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix4x2<uint>), "row_major uint4x2", value =>
			{
				var m = (ShaderUnit.Maths.Matrix4x2<uint>)value;
				return $"uint4x2({m.m11}, {m.m12}, {m.m21}, {m.m22}, {m.m31}, {m.m32}, {m.m41}, {m.m42})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix4x3<uint>), "row_major uint4x3", value =>
			{
				var m = (ShaderUnit.Maths.Matrix4x3<uint>)value;
				return $"uint4x3({m.m11}, {m.m12}, {m.m13}, {m.m21}, {m.m22}, {m.m23}, {m.m31}, {m.m32}, {m.m33}, {m.m41}, {m.m42}, {m.m43})";
			});
			AddTypeInfo(typeof(ShaderUnit.Maths.Matrix4x4<uint>), "row_major uint4x4", value =>
			{
				var m = (ShaderUnit.Maths.Matrix4x4<uint>)value;
				return $"uint4x4({m.m11}, {m.m12}, {m.m13}, {m.m14}, {m.m21}, {m.m22}, {m.m23}, {m.m24}, {m.m31}, {m.m32}, {m.m33}, {m.m34}, {m.m41}, {m.m42}, {m.m43}, {m.m44})";
			});
		}

		private static void AddTypeInfo(Type type, string hlslType, Func<object, string> makeLiteral)
		{
			_types.Add(type, new HlslTypeInfo { hlslType = hlslType, makeLiteral = makeLiteral });
		}

		private struct HlslTypeInfo
		{
			public string hlslType;
			public Func<object, string> makeLiteral;
		}

		private static Dictionary<Type, HlslTypeInfo> _types = new Dictionary<Type, HlslTypeInfo>();
	}
}

