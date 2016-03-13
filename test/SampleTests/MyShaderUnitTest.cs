using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ShaderUnit.SampleTests
{
	public class MyShaderUnitTest : RenderTestBase
	{
		[Test]
		public void NoParams_ReturnFloat()
		{
			var result = CreateComputeHarness().ExecuteShaderFunction<float>("Shaders/UnitTests.hlsl", "NoParams_ReturnFloat");
			Assert.That(result, Is.EqualTo(12.0f));
		}

		[Test]
		public void NoParams_ReturnInt()
		{
			var result = CreateComputeHarness().ExecuteShaderFunction<int>("Shaders/UnitTests.hlsl", "NoParams_ReturnInt");
			Assert.That(result, Is.EqualTo(57));
		}

		[Test]
		public void NoParams_ReturnFloat2()
		{
			var result = CreateComputeHarness().ExecuteShaderFunction<Vector2>("Shaders/UnitTests.hlsl", "NoParams_ReturnFloat2");
			Assert.That(result, Is.EqualTo(new Vector2(11.0f, 12.0f)));
		}

		[Test]
		public void NoParams_ReturnFloat3()
		{
			var result = CreateComputeHarness().ExecuteShaderFunction<Vector3>("Shaders/UnitTests.hlsl", "NoParams_ReturnFloat3");
			Assert.That(result, Is.EqualTo(new Vector3(11.0f, 12.0f, 13.0f)));
		}

		[Test]
		public void NoParams_ReturnFloat4()
		{
			var result = CreateComputeHarness().ExecuteShaderFunction<Vector4>("Shaders/UnitTests.hlsl", "NoParams_ReturnFloat4");
			Assert.That(result, Is.EqualTo(new Vector4(11.0f, 12.0f, 13.0f, 14.0f)));
		}

		[Test]
		public void OneFloatParam()
		{
			var result = CreateComputeHarness().ExecuteShaderFunction<float>("Shaders/UnitTests.hlsl", "OneFloatParam", 12.0f);
			Assert.That(result, Is.EqualTo(13.0f));
		}

		[Test]
		public void TwoFloatParams()
		{
			var result = CreateComputeHarness().ExecuteShaderFunction<float>("Shaders/UnitTests.hlsl", "TwoFloatParams", 12.0f, 13.0f);
			Assert.That(result, Is.EqualTo(25.0f));
		}

		[Test]
		public void OneFloat2Param()
		{
			var result = CreateComputeHarness().ExecuteShaderFunction<float>("Shaders/UnitTests.hlsl", "OneFloat2Param", new Vector2(1.0f, 2.0f));
			Assert.That(result, Is.EqualTo(5.0f));
		}

		[Test]
		public void OneFloat3Param()
		{
			var result = CreateComputeHarness().ExecuteShaderFunction<float>("Shaders/UnitTests.hlsl", "OneFloat3Param", new Vector3(1.0f, 2.0f, 3.0f));
			Assert.That(result, Is.EqualTo(14.0f));
		}

		[Test]
		public void OneFloat4Param()
		{
			var result = CreateComputeHarness().ExecuteShaderFunction<float>("Shaders/UnitTests.hlsl", "OneFloat4Param", new Vector4(1.0f, 2.0f, 3.0f, 4.0f));
			Assert.That(result, Is.EqualTo(30.0f));
		}
	}
}
