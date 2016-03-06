using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ShaderUnit.TestRenderer;

namespace ShaderUnit.ShaderTests
{
	public class TextureTest : RenderTestBase
	{
		[Test]
		public void TextureRender()
		{
			var ri = RenderHarness.RenderInterface;

			var vs = ri.CompileShader("FullscreenTexture.hlsl", "FullscreenTexture_VS", "vs_4_0");
			var ps = ri.CompileShader("FullscreenTexture.hlsl", "FullscreenTexture_PS", "ps_4_0");

			var texture = ri.LoadTexture("Assets/Textures/ThisIsATest.png");

			ps.FindResourceVariable("tex").Set(texture);

			var result = RenderHarness.RenderFullscreenImage(vs, ps);
			CompareImage(result);
		}
	}
}
