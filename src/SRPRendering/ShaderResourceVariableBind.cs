using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;

namespace SRPRendering
{
	interface IShaderResourceVariableBinding
	{
		ShaderResourceView GetResource(IPrimitive primitive, ViewInfo viewInfo, IGlobalResources globalResources);
	}

	class MaterialShaderResourceVariableBinding : IShaderResourceVariableBinding
	{
		public MaterialShaderResourceVariableBinding(string paramName, Texture fallback)
		{
			_paramName = paramName;
			_fallback = fallback;
		}

		public ShaderResourceView GetResource(IPrimitive primitive, ViewInfo viewInfo, IGlobalResources globalResources)
		{
			// Look up texture filename in the material.
			if (primitive != null && primitive.Material != null)
			{
				string filename;
				if (primitive.Material.Textures.TryGetValue(_paramName, out filename))
				{
					// Get the actual texture object from the scene.
					return primitive.Scene.GetTexture(filename).SRV;
				}
			}

			// Fall back to fallback texture.
			return _fallback.SRV;
		}

		private readonly string _paramName;
		private readonly Texture _fallback;
	}

	class TextureShaderResourceVariableBinding : IShaderResourceVariableBinding
	{
		public TextureShaderResourceVariableBinding(Texture texture)
		{
			this.texture = texture;
		}

		public ShaderResourceView GetResource(IPrimitive primitive, ViewInfo viewInfo, IGlobalResources globalResources)
		{
			return texture.SRV;
		}

		private readonly Texture texture;
	}

	class RenderTargetShaderResourceVariableBinding : IShaderResourceVariableBinding
	{
		public RenderTargetShaderResourceVariableBinding(RenderTargetDescriptor descriptor)
		{
			this.descriptor = descriptor;
		}

		public ShaderResourceView GetResource(IPrimitive primitive, ViewInfo viewInfo, IGlobalResources globalResources)
		{
			System.Diagnostics.Debug.Assert(descriptor.renderTarget != null);
			return descriptor.renderTarget.SRV;
		}

		private readonly RenderTargetDescriptor descriptor;
	}

	class DefaultDepthBufferShaderResourceVariableBinding : IShaderResourceVariableBinding
	{
		public ShaderResourceView GetResource(IPrimitive primitive, ViewInfo viewInfo, IGlobalResources globalResources)
		{
			return viewInfo.DepthBuffer.SRV;
		}
	}

	class BufferShaderResourceVariableBinding : IShaderResourceVariableBinding
	{
		private readonly Resources.Buffer _buffer;

		public BufferShaderResourceVariableBinding(Resources.Buffer buffer)
		{
			_buffer = buffer;
		}

		public ShaderResourceView GetResource(IPrimitive primitive, ViewInfo viewInfo, IGlobalResources globalResources)
		{
			return _buffer.SRV;
		}
	}
}
