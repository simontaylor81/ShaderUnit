using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ShaderUnit.TestRenderer;

[assembly: ShaderUnit.TestRenderer.UseTestReporter]

namespace ShaderUnit.SampleTests
{
	public class MyPixelShaderTest : RenderTestBase
	{
		[Test]
		public void MyTest()
		{
			var harness = CreateRenderHarness(64, 64);
			var ri = harness.RenderInterface;

			var vs = ri.CompileShader("ConstantColour.hlsl", "VS", "vs_4_0");
			var ps = ri.CompileShader("ConstantColour.hlsl", "PS", "ps_4_0");

			ps.FindConstantVariable("Colour").Set(new Vector4(1, 0, 0, 1));

			var result = harness.RenderFullscreenImage(vs, ps);
			CompareImage(result);
		}

		[TestCase(0, 1, 0, 1)]
		[TestCase(0, 0, 1, 1)]
		public void MyParameterisedTest(float r, float g, float b, float a)
		{
			var harness = CreateRenderHarness(64, 64);
			var ri = harness.RenderInterface;

			var vs = ri.CompileShader("ConstantColour.hlsl", "VS", "vs_4_0");
			var ps = ri.CompileShader("ConstantColour.hlsl", "PS", "ps_4_0");

			ps.FindConstantVariable("Colour").Set(new Vector4(r, g, b, a));

			var result = harness.RenderImage(context =>
			{
				context.DrawFullscreenQuad(vs, ps);
			});
			CompareImage(result);
		}
	}
}
