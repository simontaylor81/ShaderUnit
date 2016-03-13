using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ShaderUnit;
using ShaderUnit.Interfaces;
using ShaderUnit.Interfaces.Shader;

namespace ShaderUnitTests
{
	public class TextureTests : RenderTestBase
	{
		private IComputeHarness _testHarness;

		[SetUp]
		public void Setup()
		{
			_testHarness = CreateComputeHarness();
		}

		private IEnumerable<Vector4> ReadTexture(IShaderResource texture, int width, int height)
		{
			// Dispatch CS to read texture contents.
			var cs = _testHarness.RenderInterface.CompileShader("ReadTexture.hlsl", "Main", "cs_4_0");
			cs.FindResourceVariable("Texture").Set(texture);
			return _testHarness.DispatchToBuffer<Vector4>(cs, "OutBuffer", Tuple.Create(width, height, 1), Tuple.Create(1, 1, 1));
		}

		[Test]
		public void TextureFromEnumerable_RGBA32()
		{
			// Create texture with constant colour.
			var texture = _testHarness.RenderInterface.CreateTexture2D(4, 4, Format.R8G8B8A8_UNorm, EnumerableEx.Repeat(0x000000FF, 16));
			var results = ReadTexture(texture, 4, 4);

			var expected = EnumerableEx.Repeat(new Vector4(1, 0, 0, 0), 16);
			Assert.That(results, Is.EqualTo(expected));
		}

		[Test]
		public void TextureFromFile()
		{
			// Load texture from file.
			var texture = _testHarness.RenderInterface.LoadTexture("Images/red.png");
			var results = ReadTexture(texture, 16, 16);

			var expected = EnumerableEx.Repeat(new Vector4(1, 0, 0, 1), 16 * 16);
			Assert.That(results, Is.EqualTo(expected));
		}
	}
}
