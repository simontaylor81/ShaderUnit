using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ShaderUnit.Interfaces.Shader;

namespace ShaderUnit.Interfaces
{
	public interface IRenderContext
	{
		// Draw a shaded sphere.
		void DrawSphere(
			IShader vertexShaderIndex,
			IShader pixelShaderIndex,
			RastState rastState = null,
			DepthStencilState depthStencilState = null,
			BlendState blendState = null,
			IEnumerable<object> renderTargets = null,
			object depthBuffer = null);

		// Draw a fullscreen quad.
		void DrawFullscreenQuad(
			IShader vertexShaderIndex,
			IShader pixelShaderIndex,
			IEnumerable<object> renderTargets = null);

		// Dispatch a compute shader.
		void Dispatch(IShader shader, int numGroupsX, int numGroupsY, int numGroupsZ);

		// Clear render targets.
		void Clear(Vector4 colour, IEnumerable<object> renderTargets = null);
	}
}
