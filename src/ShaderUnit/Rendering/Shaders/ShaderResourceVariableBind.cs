using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using ShaderUnit.Rendering.Resources;

namespace ShaderUnit.Rendering.Shaders
{
	interface IShaderResourceVariableBinding
	{
		ID3DShaderResource GetResource(ViewInfo viewInfo, IGlobalResources globalResources);
	}

	class DirectShaderResourceVariableBinding : IShaderResourceVariableBinding
	{
		public DirectShaderResourceVariableBinding(ID3DShaderResource resource)
		{
			_resource = resource;
		}

		public ID3DShaderResource GetResource(ViewInfo viewInfo, IGlobalResources globalResources)
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

		public ID3DShaderResource GetResource(ViewInfo viewInfo, IGlobalResources globalResources)
		{
			System.Diagnostics.Debug.Assert(descriptor.renderTarget != null);
			return descriptor.renderTarget;
		}

		private readonly RenderTargetDescriptor descriptor;
	}

	class DefaultDepthBufferShaderResourceVariableBinding : IShaderResourceVariableBinding
	{
		public ID3DShaderResource GetResource(ViewInfo viewInfo, IGlobalResources globalResources)
		{
			return viewInfo.DepthBuffer;
		}
	}
}
