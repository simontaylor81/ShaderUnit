using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ShaderUnit;
using ShaderUnit.Maths;

namespace ShaderUnitTests
{
	// Tests for passing and returning various HLSL types to/from the unit test functionality.
	public class TypeTests : RenderTestBase
	{
		[Test, TestCaseSource(nameof(ReturnTypeCases))]
		public void ReturnType<T>(string type, T expected) where T : struct
		{
			var result = CreateComputeHarness().ExecuteShaderFunction<T>("Shaders/TypeTests.hlsl", "Ret_" + type);
			Assert.That(result, Is.EqualTo(expected));
		}

		// Separate data source, as attribute params cannot be structs (e.g. Vector3).
		static object[] ReturnTypeCases => new[]
		{
			new object[] { "float", 1.0f },
			new object[] { "int", -1 },
			new object[] { "uint", 1u },
			new object[] { "float2", new Vector2(1.0f, 2.0f) },
			new object[] { "float3", new Vector3(1.0f, 2.0f, 3.0f) },
			new object[] { "float4", new Vector4(1.0f, 2.0f, 3.0f, 4.0f) },
			new object[] { "float4x4", new Matrix4x4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16) },
			new object[] { "float2", new Vec2<float>(1.0f, 2.0f) },
			new object[] { "float3", new Vec3<float>(1.0f, 2.0f, 3.0f) },
			new object[] { "float4", new Vec4<float>(1.0f, 2.0f, 3.0f, 4.0f) },
			new object[] { "int2", new Vec2<int>(1, 2) },
			new object[] { "int3", new Vec3<int>(1, 2, 3) },
			new object[] { "int4", new Vec4<int>(1, 2, 3, 4) },
			new object[] { "uint2", new Vec2<uint>(1, 2) },
			new object[] { "uint3", new Vec3<uint>(1, 2, 3) },
			new object[] { "uint4", new Vec4<uint>(1, 2, 3, 4) },
			new object[] { "float4x4", new Matrix4x4<float>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16) },
			new object[] { "int4x4", new Matrix4x4<int>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16) },
			new object[] { "uint4x4", new Matrix4x4<uint>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16) },
		};

		[Test, TestCaseSource(nameof(ParamTypeCases))]
		public void ParamType(string type, object param)
		{
			var result = CreateComputeHarness().ExecuteShaderFunction<float>("Shaders/TypeTests.hlsl", "In_" + type, param);
			Assert.That(result, Is.EqualTo(1.0f));	// All cases are written to return 1.0f
		}

		// Separate data source, as attribute params cannot be structs (e.g. Vector3).
		static object[] ParamTypeCases => new[]
		{
			new object[] { "float", 1.0f },
			new object[] { "int", 1 },
			new object[] { "uint", 1u },
			new object[] { "float2", new Vector2(0, 1.0f) },
			new object[] { "float3", new Vector3(0, 0, 1.0f) },
			new object[] { "float4", new Vector4(0, 0, 0, 1.0f) },
			new object[] { "float4x4", new Matrix4x4(0, 0, 0, 0, 0, 0, 0, 1.0f, 0, 0, 0, 0, 0, 0, 0, 0) },
			new object[] { "float2", new Vec2<float>(0, 1.0f) },
			new object[] { "float3", new Vec3<float>(0, 0, 1.0f) },
			new object[] { "float4", new Vec4<float>(0, 0, 0, 1.0f) },
			new object[] { "int2", new Vec2<int>(0, 1) },
			new object[] { "int3", new Vec3<int>(0, 0, 1) },
			new object[] { "int4", new Vec4<int>(0, 0, 0, 1) },
			new object[] { "uint2", new Vec2<uint>(0, 1) },
			new object[] { "uint3", new Vec3<uint>(0, 0, 1) },
			new object[] { "uint4", new Vec4<uint>(0, 0, 0, 1) },
			new object[] { "float4x4", new Matrix4x4<float>(0, 0, 0, 0, 0, 0, 0, 1.0f, 0, 0, 0, 0, 0, 0, 0, 0) },
			new object[] { "int4x4", new Matrix4x4<int>(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0) },
			new object[] { "uint4x4", new Matrix4x4<uint>(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0) },
		};
	}
}
