using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ShaderUnit.TestRenderer;

namespace ShaderUnit.SampleTests
{
	public class TextureTest : RenderTestBase
	{
		[Test]
		public void TextureFromFile()
		{
			var harness = CreateRenderHarness(64, 64);
			var ri = harness.RenderInterface;

			var vs = ri.CompileShader("FullscreenTexture.hlsl", "FullscreenTexture_VS", "vs_4_0");
			var ps = ri.CompileShader("FullscreenTexture.hlsl", "FullscreenTexture_PS", "ps_4_0");

			var texture = ri.LoadTexture("Textures/ThisIsATest.png");

			ps.FindResourceVariable("tex").Set(texture);

			var result = harness.RenderFullscreenImage(vs, ps);
			CompareImage(result);
		}

		[Test]
		public void GeneratedTexture()
		{
			var harness = CreateRenderHarness(64, 64);
			var ri = harness.RenderInterface;

			var vs = ri.CompileShader("FullscreenTexture.hlsl", "FullscreenTexture_VS", "vs_4_0");
			var ps = ri.CompileShader("FullscreenTexture.hlsl", "FullscreenTexture_PS", "ps_4_0");

			var size = 64;
			var contents = Enumerable.Range(0, size)
				.SelectMany(y => Enumerable.Range(0, size)
					.Select(x => MakeRGBA((uint)(x * 256 / size), (uint)(y * 256 / size), 0)));

			var texture = ri.CreateTexture2D(64, 64, SRPScripting.Format.R8G8B8A8_UNorm_SRgb, contents);

			ps.FindResourceVariable("tex").Set(texture);

			var result = harness.RenderFullscreenImage(vs, ps);
			CompareImage(result);
		}

		private uint MakeRGBA(uint r, uint g, uint b) => ((r & 0xFF) << 0) | ((g & 0xFF) << 8) | ((b & 0xFF) << 16) | 0xFF000000;
	}
}
