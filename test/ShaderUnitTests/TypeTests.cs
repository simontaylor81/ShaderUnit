using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ShaderUnit;

namespace ShaderUnitTests
{
	// Tests for passing and returning various HLSL types to/from the unit test functionality.
	public class TypeTests : RenderTestBase
	{
		[Test, TestCaseSource(nameof(ReturnTypeCases))]
		public void ReturnType<T>(string type, T expected) where T : struct
		{
			var result = CreateComputeHarness().ExecuteShaderFunction<T>("TypeTests.hlsl", "Ret_" + type);
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
		};
	}
}
