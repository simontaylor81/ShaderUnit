using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;
using SRPCommon.Util;
using SRPRendering.Resources;
using SRPRendering.Shaders;
using SRPScripting;
using SRPScripting.Shader;

namespace SRPRendering
{
	class ScriptRenderContext : IRenderContext
	{
		public ScriptRenderContext(DeviceContext deviceContext,
								   ViewInfo viewInfo,
								   IList<RenderTarget> renderTargets,
								   IGlobalResources globalResources)
		{
			this.deviceContext = deviceContext;
			this.viewInfo = viewInfo;
			this.renderTargetResources = renderTargets;
			_globalResources = globalResources;
		}

		#region IRenderContext interface

		// Draw a unit sphere.
		public void DrawSphere(IShader vertexShaderInterface,
							   IShader pixelShaderInterface,
							   RastState rastState = null,
							   SRPScripting.DepthStencilState depthStencilState = null,
							   SRPScripting.BlendState blendState = null,
							   IEnumerable<object> renderTargetHandles = null,
							   object depthBuffer = null)
		{
			Shader vertexShader = GetShader(vertexShaderInterface);
			Shader pixelShader = GetShader(pixelShaderInterface);

			// Vertex shader is not optional.
			if (vertexShader == null)
				throw new ShaderUnitException("DrawSphere: Cannot draw without a vertex shader.");

			// Set input layout
			deviceContext.InputAssembler.InputLayout = _globalResources.InputLayoutCache.GetInputLayout(
				deviceContext.Device, vertexShader.Signature, BasicMesh.InputElements);

			// Set render state.
			SetRenderTargets(renderTargetHandles, depthBuffer);
			deviceContext.Rasterizer.State = _globalResources.RastStateCache.Get(rastState.ToD3D11());
			deviceContext.OutputMerger.DepthStencilState = _globalResources.DepthStencilStateCache.Get(depthStencilState.ToD3D11());
			deviceContext.OutputMerger.BlendState = _globalResources.BlendStateCache.Get(blendState.ToD3D11());
			SetShaders(vertexShader, pixelShader);

			// Draw the sphere mesh.
			UpdateShaders(vertexShader, pixelShader);
			_globalResources.SphereMesh.Draw(deviceContext);

			// Force all state to defaults -- we're completely stateless.
			deviceContext.ClearState();
		}

		// Draw a fullscreen quad.
		public void DrawFullscreenQuad(IShader vertexShaderInterface,
									   IShader pixelShaderInterface,
									   IEnumerable<object> renderTargetHandles = null)
		{
			Shader vertexShader = GetShader(vertexShaderInterface);
			Shader pixelShader = GetShader(pixelShaderInterface);

			// Vertex shader is not optional.
			if (vertexShader == null)
				throw new ShaderUnitException("DrawFullscreenQuad: Cannot draw without a vertex shader.");

			// Set input layout
			deviceContext.InputAssembler.InputLayout = _globalResources.InputLayoutCache.GetInputLayout(
				deviceContext.Device, vertexShader.Signature, FullscreenQuad.InputElements);

			// Set render state.
			SetRenderTargets(renderTargetHandles, DepthBufferHandle.NoDepthBuffer);
			deviceContext.Rasterizer.State = _globalResources.RastStateCache.Get(RastState.Default.ToD3D11());
			deviceContext.OutputMerger.DepthStencilState = _globalResources.DepthStencilStateCache.Get(SRPScripting.DepthStencilState.DisableDepth.ToD3D11());
			deviceContext.OutputMerger.BlendState = _globalResources.BlendStateCache.Get(SRPScripting.BlendState.NoBlending.ToD3D11());
			SetShaders(vertexShader, pixelShader);

			// Draw the quad.
			UpdateShaders(vertexShader, pixelShader);
			_globalResources.FullscreenQuad.Draw(deviceContext);

			// Force all state to defaults -- we're completely stateless.
			deviceContext.ClearState();
		}

		// Dispatch a compute shader.
		public void Dispatch(IShader shaderInterface, int numGroupsX, int numGroupsY, int numGroupsZ)
		{
			Shader cs = GetShader(shaderInterface);
			if (cs == null)
			{
				throw new ShaderUnitException("Dispatch: compute shader is required");
			}

			cs.Set(deviceContext);
			cs.UpdateVariables(deviceContext, viewInfo, _globalResources);
			deviceContext.Dispatch(numGroupsX, numGroupsY, numGroupsZ);

			// Enforce statelessness.
			deviceContext.ClearState();
		}

		// Clear render targets.
		public void Clear(Vector4 colour, IEnumerable<object> renderTargetHandles = null)
		{
			// Convert list of floats to a colour.
			try
			{
				var rawColour = new RawColor4(colour.X, colour.Y, colour.Z, colour.W);

				// Clear each specified target.
				var rtvs = GetRTVS(renderTargetHandles);
				foreach (var rtv in rtvs)
				{
					deviceContext.ClearRenderTargetView(rtv, rawColour);
				}
			}
			catch (ShaderUnitException ex)
			{
				throw new ShaderUnitException("Clear: Invalid colour.", ex);
			}
		}

		#endregion

		// Access a shader by handle.
		private Shader GetShader(IShader ishader, [CallerMemberName] string caller = null)
		{
			// null means no shader.
			if (ishader == null)
				return null;

			// If it's not null, but not a valid concrete shader, throw.
			var shader = ishader as Shader;
			if (shader == null)
				throw new ShaderUnitException(string.Format("Invalid shader given to {0}.", caller));

			return shader;
		}

		// Access a render target by handle.
		private RenderTarget GetRenderTarget(object handleObj, [CallerMemberName] string caller = null)
		{
			var handle = handleObj as RenderTargetHandle;

			// If it's null or not a valid index, throw.
			if (handle == null || handle.index < 0 || handle.index >= renderTargetResources.Count)
				throw new ShaderUnitException(string.Format("Invalid render target given to {0}.", caller));

			return renderTargetResources[handle.index];
		}

		// Set the given shaders to the device.
		private void SetShaders(params Shader[] shaders)
		{
			foreach (var shader in shaders)
			{
				if (shader != null)
					shader.Set(deviceContext);
			}
		}

		// Update the variables of the given shaders, unless they're null.
		private void UpdateShaders(Shader vs, Shader ps)
		{
			if (vs != null)
				vs.UpdateVariables(deviceContext, viewInfo, _globalResources);
			else
				deviceContext.VertexShader.Set(null);

			if (ps != null)
				ps.UpdateVariables(deviceContext, viewInfo, _globalResources);
			else
				deviceContext.PixelShader.Set(null);
		}

		// Set render targets based on the given list of handles.
		private void SetRenderTargets(IEnumerable<object> renderTargetHandles, object depthBuffer)
		{
			// Collect render target views for the given handles.
			var rtvs = GetRTVS(renderTargetHandles).ToArray();

			// Find the depth buffer.
			DepthStencilView dsv;
			if (depthBuffer == null || DepthBufferHandle.Default.Equals(depthBuffer))
			{
				dsv = viewInfo.DepthBuffer.DSV;
			}
			else if (DepthBufferHandle.NoDepthBuffer.Equals(depthBuffer))
			{
				dsv = null;
			}
			else
			{
				// TODO: User-allocated depth buffers.
				throw new ShaderUnitException("Invalid depth buffer.");
			}

			// Set them to the device.
			deviceContext.OutputMerger.SetTargets(dsv, rtvs);

			// Set viewport.
			deviceContext.Rasterizer.SetViewports(new[] { GetViewport(renderTargetHandles) });
		}

		// Converts a list of render target handles to a list of RTVs, resolving nulls to the back buffer.
		private IEnumerable<RenderTargetView> GetRTVS(IEnumerable<object> renderTargetHandles)
		{
			if (renderTargetHandles != null)
			{
				return from handle in renderTargetHandles
					   select handle != null ? GetRenderTarget(handle).RTV : viewInfo.BackBuffer;
			}
			else
			{
				// No render targets specified, so write to backbuffer.
				return new[] { viewInfo.BackBuffer };
			}
		}

		// Get the viewport dimensions to use.
		private RawViewportF GetViewport(IEnumerable<object> renderTargetHandles)
		{
			if (renderTargetHandles != null)
			{
				// Get viewport size from the first render target.
				var handle = renderTargetHandles.FirstOrDefault();
				if (handle != null)
				{
					var rt = GetRenderTarget(handle);
					return new RawViewportF
					{
						X = 0.0f,
						Y = 0.0f,
						Width = rt.Width,
						Height = rt.Height,
						MinDepth = 0.0f,
						MaxDepth = 1.0f,
					};
				}
			}

			return new RawViewportF
			{
				X = 0.0f,
				Y = 0.0f,
				Width = viewInfo.ViewportWidth,
				Height = viewInfo.ViewportHeight,
				MinDepth = 0.0f,
				MaxDepth = 1.0f,
			};
		}

		private DeviceContext deviceContext;
		private IList<RenderTarget> renderTargetResources;
		private ViewInfo viewInfo;
		private IGlobalResources _globalResources;
	}
}
