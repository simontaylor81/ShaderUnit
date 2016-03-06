using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using ShaderUnit.Util;
using ShaderUnit.Rendering.Resources;
using ShaderUnit.Interfaces;
using ShaderUnit.Interfaces.Shader;

namespace ShaderUnit.Rendering.Shaders
{
	class ShaderUavVariable : IShaderUavVariable
	{
		public string Name { get; }

		private ID3DShaderResource _resource;

		public void Set(IShaderResource iresource)
		{
			var resource = iresource as ID3DShaderResource;
			if (iresource == null)
			{
				throw new ShaderUnitException("Invalid buffer for UAV");
			}

			_resource = resource;
		}

		public void SetToDevice(DeviceContext context)
		{
			if (_shaderFrequency != ShaderFrequency.Compute)
			{
				// TODO: Pixel shader UAVs.
				throw new ShaderUnitException("UAVs are only supported for compute shaders.");
			}

			context.ComputeShader.SetUnorderedAccessView(_slot, _resource.UAV);
		}

		public ShaderUavVariable(InputBindingDescription desc, ShaderFrequency shaderFrequency)
		{
			Name = desc.Name;
			_slot = desc.BindPoint;

			_shaderFrequency = shaderFrequency;

			// TODO: Support arrays.
			Trace.Assert(desc.BindCount == 1);
		}

		private int _slot;
		private ShaderFrequency _shaderFrequency;
	}
}
