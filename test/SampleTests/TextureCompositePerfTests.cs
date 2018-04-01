using NUnit.Framework;
using ShaderUnit;
using ShaderUnit.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleTests
{
	public class TextureCompositePerfTests : RenderTestBase
	{
		[Test]
		[TestCase(true, TestName = "Texture Blend (WARP)")]
		[TestCase(false, TestName = "Texture Blend (GPU)")]
		public void TextureBlend(bool warp)
		{
			var harness = CreateRenderHarness(4096, 4096, warp);
			var ri = harness.RenderInterface;

			var vs = ri.CompileShader("Shaders/TextureBlend.hlsl", "vsMain", "vs_4_0");
			var ps = ri.CompileShader("Shaders/TextureBlend.hlsl", "psMain", "ps_4_0");

			var texture1 = ri.LoadTexture("Textures/Cerberus_N.tga");
			var texture2 = ri.LoadTexture("Textures/Cerberus_A.tga");
			var textureMask = ri.LoadTexture("Textures/Cerberus_M.tga");

			//ps.FindSamplerVariable("samp").Set(SamplerState.PointClamp);
			ps.FindSamplerVariable("samp").Set(SamplerState.LinearClamp);

			ps.FindResourceVariable("tex1").Set(texture1);
			ps.FindResourceVariable("tex2").Set(texture2);
			ps.FindResourceVariable("texMask").Set(textureMask);

			var sw = Stopwatch.StartNew();

			// Prime caches.
			harness.RenderFullscreenImage(vs, ps);

			Console.WriteLine("First pass time ({0}): {1} ms",
				TestContext.CurrentContext.Test.Name,
				sw.ElapsedMilliseconds);

			sw.Restart();

			const int iterationCount = 10;
			for (int i = 0; i < iterationCount; i++)
			{
				harness.RenderFullscreenImage(vs, ps);
			}

			sw.Stop();
			Console.WriteLine("Average render time ({0}): {1} ms",
				TestContext.CurrentContext.Test.Name,
				((float)sw.ElapsedMilliseconds) / iterationCount);

			//var result = harness.RenderFullscreenImage(vs, ps);
			//CompareImage(result);
		}
	}
}
