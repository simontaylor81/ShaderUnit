using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;

namespace ShaderUnit.Rendering
{
	// Extension methods for the various script interface types.
	static class ScriptInterfaceExtensions
	{
		// Convert a script-interface rasterizer state to a D3D11 one.
		public static RasterizerStateDescription ToD3D11(this ShaderUnit.Interfaces.RastState state)
		{
			// Allow transparent handling of null references.
			if (state == null)
				return ShaderUnit.Interfaces.RastState.Default.ToD3D11();

			return new RasterizerStateDescription()
				{
					FillMode = state.fillMode.ToD3D11(),
					CullMode = state.cullMode.ToD3D11(),
					IsFrontCounterClockwise = false,
					DepthBias = state.depthBias,
					DepthBiasClamp = state.depthBiasClamp,
					SlopeScaledDepthBias = state.slopeScaleDepthBias,
					IsScissorEnabled = state.enableScissor,
					IsDepthClipEnabled = state.enableDepthClip,
					IsMultisampleEnabled = false,
					IsAntialiasedLineEnabled = false
				};
		}

		// Convert a script-interface depth-stencil state to a D3D11 one.
		public static DepthStencilStateDescription ToD3D11(this ShaderUnit.Interfaces.DepthStencilState state)
		{
			// Allow transparent handling of null references.
			if (state == null)
				// Default to depth enabled.
				return ShaderUnit.Interfaces.DepthStencilState.EnableDepth.ToD3D11();

			return new DepthStencilStateDescription()
				{
					IsDepthEnabled = state.enableDepthTest,
					DepthWriteMask = state.enableDepthWrite ? DepthWriteMask.All : DepthWriteMask.Zero,
					DepthComparison = state.depthFunc.ToD3D11()
				};
		}

		// Convert a script-interface blend state to a D3D11 one.
		public static BlendStateDescription ToD3D11(this ShaderUnit.Interfaces.BlendState state)
		{
			// Allow transparent handling of null references.
			if (state == null)
				return ShaderUnit.Interfaces.BlendState.NoBlending.ToD3D11();

			var rtState = new RenderTargetBlendDescription()
				{
					IsBlendEnabled = state.enableBlending,
					SourceBlend = state.sourceInput.ToD3D11(),
					DestinationBlend = state.destInput.ToD3D11(),
					SourceAlphaBlend = state.sourceAlphaInput.ToD3D11(),
					DestinationAlphaBlend = state.destAlphaInput.ToD3D11(),
					BlendOperation = state.colourOp.ToD3D11(),
					AlphaBlendOperation = state.alphaOp.ToD3D11(),
					RenderTargetWriteMask = ColorWriteMaskFlags.All,
				};

			var desc = new BlendStateDescription()
				{
					IndependentBlendEnable = false,
					AlphaToCoverageEnable = false,
				};
			desc.RenderTarget[0] = rtState;

			return desc;
		}

		// Convert a script-interface sampler state to a D3D11 one.
		public static SamplerStateDescription ToD3D11(this ShaderUnit.Interfaces.SamplerState state)
		{
			// Allow transparent handling of null references.
			if (state == null)
				return ShaderUnit.Interfaces.SamplerState.LinearWrap.ToD3D11();

			return new SamplerStateDescription()
			{
				Filter = state.filter.ToD3D11(),
				AddressU = state.addressMode.ToD3D11(),
				AddressV = state.addressMode.ToD3D11(),
				AddressW = state.addressMode.ToD3D11(),
				MipLodBias = 0,
				MaximumAnisotropy = 8,
				MinimumLod = 0,
				MaximumLod = float.MaxValue,
			};
		}

		public static SharpDX.Direct3D11.FillMode ToD3D11(this ShaderUnit.Interfaces.FillMode fillMode)
		{
			switch (fillMode)
			{
				case ShaderUnit.Interfaces.FillMode.Solid:
					return SharpDX.Direct3D11.FillMode.Solid;

				case ShaderUnit.Interfaces.FillMode.Wireframe:
					return SharpDX.Direct3D11.FillMode.Wireframe;

				default:
					throw new ArgumentException("Invalid fill mode.");
			}
		}

		public static SharpDX.Direct3D11.CullMode ToD3D11(this ShaderUnit.Interfaces.CullMode cullMode)
		{
			switch (cullMode)
			{
				case ShaderUnit.Interfaces.CullMode.Back:
					return SharpDX.Direct3D11.CullMode.Back;

				case ShaderUnit.Interfaces.CullMode.Front:
					return SharpDX.Direct3D11.CullMode.Front;

				case ShaderUnit.Interfaces.CullMode.None:
					return SharpDX.Direct3D11.CullMode.None;

				default:
					throw new ArgumentException("Invalid cull mode.");
			}
		}

		public static Comparison ToD3D11(this ShaderUnit.Interfaces.ComparisonFunction func)
		{
			switch (func)
			{
				case ShaderUnit.Interfaces.ComparisonFunction.Never: return Comparison.Never;
				case ShaderUnit.Interfaces.ComparisonFunction.Always: return Comparison.Always;
				case ShaderUnit.Interfaces.ComparisonFunction.Equal: return Comparison.Equal;
				case ShaderUnit.Interfaces.ComparisonFunction.NotEqual: return Comparison.NotEqual;
				case ShaderUnit.Interfaces.ComparisonFunction.Less: return Comparison.Less;
				case ShaderUnit.Interfaces.ComparisonFunction.LessEqual: return Comparison.LessEqual;
				case ShaderUnit.Interfaces.ComparisonFunction.Greater: return Comparison.Greater;
				case ShaderUnit.Interfaces.ComparisonFunction.GreaterEqual: return Comparison.GreaterEqual;

				default:
					throw new ArgumentException("Invalid comparison function.");
			}
		}

		public static BlendOption ToD3D11(this ShaderUnit.Interfaces.BlendInput blendInput)
		{
			switch (blendInput)
			{
				case ShaderUnit.Interfaces.BlendInput.Zero: return BlendOption.Zero;
				case ShaderUnit.Interfaces.BlendInput.One: return BlendOption.One;
				case ShaderUnit.Interfaces.BlendInput.SourceColor: return BlendOption.SourceColor;
				case ShaderUnit.Interfaces.BlendInput.InvSourceColor: return BlendOption.InverseSourceColor;
				case ShaderUnit.Interfaces.BlendInput.SourceAlpha: return BlendOption.SourceAlpha;
				case ShaderUnit.Interfaces.BlendInput.InvSourceAlpha: return BlendOption.InverseSourceAlpha;
				case ShaderUnit.Interfaces.BlendInput.DestColor: return BlendOption.DestinationColor;
				case ShaderUnit.Interfaces.BlendInput.InvDestColor: return BlendOption.InverseDestinationColor;
				case ShaderUnit.Interfaces.BlendInput.DestAlpha: return BlendOption.DestinationAlpha;
				case ShaderUnit.Interfaces.BlendInput.InvDestAlpha: return BlendOption.InverseDestinationAlpha;
				case ShaderUnit.Interfaces.BlendInput.SourceAlphaSat: return BlendOption.SourceAlphaSaturate;
				case ShaderUnit.Interfaces.BlendInput.BlendFactor: return BlendOption.BlendFactor;
				case ShaderUnit.Interfaces.BlendInput.InvBlendFactor: return BlendOption.InverseBlendFactor;
				case ShaderUnit.Interfaces.BlendInput.Source1Color: return BlendOption.SecondarySourceColor;
				case ShaderUnit.Interfaces.BlendInput.InvSource1Color: return BlendOption.InverseSecondarySourceColor;
				case ShaderUnit.Interfaces.BlendInput.Source1Alpha: return BlendOption.SecondarySourceAlpha;
				case ShaderUnit.Interfaces.BlendInput.InvSource1Alpha: return BlendOption.InverseSecondarySourceAlpha;

				default:
					throw new ArgumentException("Invalid blend input.");
			}
		}

		public static BlendOperation ToD3D11(this ShaderUnit.Interfaces.BlendOp blendOp)
		{
			switch (blendOp)
			{
				case ShaderUnit.Interfaces.BlendOp.Add: return BlendOperation.Add;
				case ShaderUnit.Interfaces.BlendOp.Subtract: return BlendOperation.Subtract;
				case ShaderUnit.Interfaces.BlendOp.ReverseSubtract: return BlendOperation.ReverseSubtract;
				case ShaderUnit.Interfaces.BlendOp.Min: return BlendOperation.Minimum;
				case ShaderUnit.Interfaces.BlendOp.Max: return BlendOperation.Maximum;

				default:
					throw new ArgumentException("Invalid blend operation.");
			}
		}

		public static Filter ToD3D11(this ShaderUnit.Interfaces.TextureFilter filter)
		{
			switch (filter)
			{
				case ShaderUnit.Interfaces.TextureFilter.Point: return Filter.MinMagMipPoint;
				case ShaderUnit.Interfaces.TextureFilter.Linear: return Filter.MinMagMipLinear;
				case ShaderUnit.Interfaces.TextureFilter.Anisotropic: return Filter.Anisotropic;

				default:
					throw new ArgumentException("Invalid texture filter.");
			}
		}

		public static TextureAddressMode ToD3D11(this ShaderUnit.Interfaces.TextureAddressMode mode)
		{
			switch (mode)
			{
				case ShaderUnit.Interfaces.TextureAddressMode.Wrap: return TextureAddressMode.Wrap;
				case ShaderUnit.Interfaces.TextureAddressMode.Clamp: return TextureAddressMode.Clamp;
				case ShaderUnit.Interfaces.TextureAddressMode.Mirror: return TextureAddressMode.Mirror;

				default:
					throw new ArgumentException("Invalid texture addressing mode.");
			}
		}

		// Convert a script format to a DXGI one.
		public static SharpDX.DXGI.Format ToDXGI(this ShaderUnit.Interfaces.Format format)
		{
			// This is rather dirty -- the formats are just copies of the SharpDX ones, currently.
			SharpDX.DXGI.Format result;
			if (Enum.TryParse(format.ToString(), out result))
			{
				return result;
			}

			throw new ArgumentException("Invalid DXGI format: " + format.ToString());
		}
	}
}
