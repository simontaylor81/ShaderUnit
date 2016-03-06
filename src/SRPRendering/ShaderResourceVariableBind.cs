using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using SRPRendering.Resources;

namespace SRPRendering
{
	interface IShaderResourceVariableBinding
	{
		ID3DShaderResource GetResource(IPrimitive primitive, ViewInfo viewInfo, IGlobalResources globalResources);
	}

	class MaterialShaderResourceVariableBinding : IShaderResourceVariableBinding
	{
		public MaterialShaderResourceVariableBinding(string paramName, ID3DShaderResource fallback)
		{
			_paramName = paramName;
			_fallback = fallback;
		}

		public ID3DShaderResource GetResource(IPrimitive primitive, ViewInfo viewInfo, IGlobalResources globalResources)
		{
			// Look up texture filename in the material.
			if (primitive != null && primitive.Material != null)
			{
				string filename;
				if (primitive.Material.Textures.TryGetValue(_paramName, out filename))
				{
					// Get the actual texture object from the scene.
					return primitive.Scene.GetTexture(filename);
				}
			}

			// Fall back to fallback texture.
			return _fallback;
		}

		private readonly string _paramName;
		private readonly ID3DShaderResource _fallback;
	}

	class DirectShaderResourceVariableBinding : IShaderResourceVariableBinding
	{
		public DirectShaderResourceVariableBinding(ID3DShaderResource resource)
		{
			_resource = resource;
		}

		public ID3DShaderResource GetResource(IPrimitive primitive, ViewInfo viewInfo, IGlobalResources globalResources)
		{
			return _resource;
		}

		private readonly ID3DShaderResource _resource;
	}

	class RenderTargetShaderResourceVariableBinding : IShaderResourceVariableBinding
	{
		public RenderTargetShaderResourceVariableBinding(RenderTargetDescriptor descriptor)
		{
			this.descriptor = descriptor;
		}

		public ID3DShaderResource GetResource(IPrimitive primitive, ViewInfo viewInfo, IGlobalResources globalResources)
		{
			System.Diagnostics.Debug.Assert(descriptor.renderTarget != null);
			return descriptor.renderTarget;
		}

		private readonly RenderTargetDescriptor descriptor;
	}

	class DefaultDepthBufferShaderResourceVariableBinding : IShaderResourceVariableBinding
	{
		public ID3DShaderResource GetResource(IPrimitive primitive, ViewInfo viewInfo, IGlobalResources globalResources)
		{
			return viewInfo.DepthBuffer;
		}
	}
}
