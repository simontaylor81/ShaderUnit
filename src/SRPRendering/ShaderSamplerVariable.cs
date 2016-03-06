using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SRPScripting.Shader;

namespace SRPRendering
{
	// Info about a sampler input to a shader.
	class ShaderSamplerVariable : IShaderSamplerVariable
	{
		public string Name { get; }

		private readonly int _slot;
		private readonly ShaderFrequency _shaderFrequency;
		private SamplerStateDescription _stateDesc;

		public void Set(SRPScripting.SamplerState state)
		{
			_stateDesc = state.ToD3D11();
		}

		public ShaderSamplerVariable(InputBindingDescription desc, ShaderFrequency frequency)
		{
			Name = desc.Name;
			_slot = desc.BindPoint;
			_shaderFrequency = frequency;
			_stateDesc = SamplerStateDescription.Default();
		}

		public void SetToDevice(DeviceContext context, IGlobalResources globalResources)
		{
			var state = globalResources.SamplerStateCache.Get(_stateDesc);

			switch (_shaderFrequency)
			{
				case ShaderFrequency.Vertex:
					context.VertexShader.SetSampler(_slot, state);
					break;

				case ShaderFrequency.Pixel:
					context.PixelShader.SetSampler(_slot, state);
					break;

				case ShaderFrequency.Compute:
					context.ComputeShader.SetSampler(_slot, state);
					break;
			}
		}
	}
}
