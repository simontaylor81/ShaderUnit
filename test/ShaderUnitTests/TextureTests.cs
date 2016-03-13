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

		private IEnumerable<Vector4> ReadTexture(ITexture2D texture)
		{
			// Dispatch CS to read texture contents.
			var cs = _testHarness.RenderInterface.CompileShader("ReadTexture.hlsl", "Main", "cs_4_0");
			cs.FindResourceVariable("Texture").Set(texture);
			return _testHarness.DispatchToBuffer<Vector4>(cs, "OutBuffer", Tuple.Create(texture.Width, texture.Height, 1));
		}

		[Test]
		public void TextureFromEnumerable_RGBA32()
		{
			int width = 4;
			int height = 4;

			// Create texture with constant colour.
			var texture = _testHarness.RenderInterface.CreateTexture2D(width, height, Format.R8G8B8A8_UNorm, EnumerableEx.Repeat(0x000000FF, 16));
			Assert.That(texture.Width, Is.EqualTo(width));
			Assert.That(texture.Height, Is.EqualTo(height));

			var results = ReadTexture(texture);

			var expected = EnumerableEx.Repeat(new Vector4(1, 0, 0, 0), width * height);
			Assert.That(results, Is.EqualTo(expected));
		}

		[Test]
		public void TextureFromFile()
		{
			// Load texture from file.
			var texture = _testHarness.RenderInterface.LoadTexture("Images/red.png", generateMips: false);
			var results = ReadTexture(texture);

			var expected = EnumerableEx.Repeat(new Vector4(1, 0, 0, 1), texture.Width * texture.Height);
			Assert.That(results, Is.EqualTo(expected));
		}
	}
}
