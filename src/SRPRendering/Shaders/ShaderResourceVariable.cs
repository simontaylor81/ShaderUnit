using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SRPCommon.Util;
using SRPRendering.Resources;
using SRPScripting;
using SRPScripting.Shader;

namespace SRPRendering.Shaders
{
	class ShaderResourceVariable : IShaderResourceVariable
	{
		// IShaderVariable interface.
		public string Name { get; }

		public ID3DShaderResource Resource { get; set; }

		public void Set(IShaderResource iresource)
		{
			var resource = iresource as ID3DShaderResource;
			if (iresource == null)
			{
				throw new ShaderUnitException("Invalid buffer for UAV");
			}

			Binding = new DirectShaderResourceVariableBinding(resource);
		}

		public void BindToMaterial(string materialParam, IShaderResource fallback = null)
		{
			if (fallback != null)
			{
				throw new NotImplementedException("TODO: Shader resource abstraction");
			}
			Binding = new MaterialShaderResourceVariableBinding(materialParam, null);
		}

		// TODO: Render target & depth buffer bindings.

		private IShaderResourceVariableBinding _binding;
		public IShaderResourceVariableBinding Binding
		{
			get { return _binding; }
			set
			{
				if (_binding != null)
				{
					throw new ShaderUnitException("Attempting to bind already bound shader variable: " + Name);
				}
				_binding = value;
			}
		}

		public void SetToDevice(DeviceContext context)
		{
			switch (shaderFrequency)
			{
				case ShaderFrequency.Vertex:
					context.VertexShader.SetShaderResource(slot, Resource.SRV);
					break;

				case ShaderFrequency.Pixel:
					context.PixelShader.SetShaderResource(slot, Resource.SRV);
					break;

				case ShaderFrequency.Compute:
					context.ComputeShader.SetShaderResource(slot, Resource.SRV);
					break;
			}
		}

		// Constructors.
		public ShaderResourceVariable(InputBindingDescription desc, ShaderFrequency shaderFrequency)
		{
			Name = desc.Name;
			slot = desc.BindPoint;

			this.shaderFrequency = shaderFrequency;

			// TODO: Support arrays.
			Trace.Assert(desc.BindCount == 1);
		}

		private int slot;
		private ShaderFrequency shaderFrequency;
	}
}
