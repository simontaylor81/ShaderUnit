using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ShaderUnit;
using ShaderUnit.Interfaces;
using ShaderUnit.Interfaces.Shader;

namespace ShaderUnitTests
{
	public class ShaderCompileTests : RenderTestBase
	{
		private IComputeHarness harness;

		[SetUp]
		public void Init()
		{
			harness = CreateComputeHarness();
		}

		[TestCase("CS", "cs_4_0", ShaderFrequency.Compute)]
		[TestCase("PS", "ps_4_0", ShaderFrequency.Pixel)]
		[TestCase("VS", "vs_4_0", ShaderFrequency.Vertex)]
		public void ValidShaderCompiles(string entryPoint, string profile, ShaderFrequency frequency)
		{
			var shader = harness.RenderInterface.CompileShader("Shaders/ValidShaders.hlsl", entryPoint, profile);
			Assert.That(shader, Is.Not.Null);
			Assert.That(shader.Frequency, Is.EqualTo(frequency));
		}

		[TestCase("AbsoluteIncluder.hlsl")]
		[TestCase("RelativeIncluder.hlsl")]
		[TestCase("NestedRelativeIncluder.hlsl")]
		public void FileWithIncludeCompiles(string filename)
		{
			var shader = harness.RenderInterface.CompileShader("Shaders/" + filename, "entry", "cs_4_0");
			Assert.That(shader, Is.Not.Null);
		}

		[TestCase("MissingFile.hlsl", "Could not find shader file")]
		[TestCase("SyntaxError.hlsl", @"test\ShaderUnitTests\Assets\Shaders/Errors/SyntaxError.hlsl(3,1): error X3000: syntax error: unexpected token '}'")]
		[TestCase("IdentifierNotFound.hlsl", @"test\ShaderUnitTests\Assets\Shaders/Errors/IdentifierNotFound.hlsl(3,9-19): error X3004: undeclared identifier 'nonExistant'")]
		[TestCase("ErrorIncluder.hlsl", @"test\ShaderUnitTests\Assets\Shaders\Errors\ErrorIncludee.hlsl(5,1): error X3000: syntax error: unexpected token '}'")]
		[TestCase("MissingEntryPoint.hlsl", "error X3501: 'entry': entrypoint not found")]
		public void ShaderWithErrorThrows(string filename, string errorMessage)
		{
			var ex = Assert.Throws<ShaderUnitException>(() => harness.RenderInterface.CompileShader("Shaders/Errors/" + filename, "entry", "cs_4_0"));
			Assert.That(ex.Message, Does.Contain(errorMessage));
		}
	}
}
