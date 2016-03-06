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

namespace SRPRendering
{
	class ShaderUavVariable : IShaderUavVariable
	{
		public string Name { get; }

		public UnorderedAccessView UAV { get; set; }

		public void Set(IBuffer buffer)
		{
			var handle = buffer as BufferHandle;
			if (buffer == null)
			{
				throw new ShaderUnitException("Invalid buffer for UAV");
			}

			UAV = handle.Buffer.UAV;
		}

		public void SetToDevice(DeviceContext context)
		{
			if (_shaderFrequency != ShaderFrequency.Compute)
			{
				// TODO: Pixel shader UAVs.
				throw new ShaderUnitException("UAVs are only supported for compute shaders.");
			}

			context.ComputeShader.SetUnorderedAccessView(_slot, UAV);
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
