using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SharpDX.Direct3D;
using ShaderUnit.Util;
using ShaderUnit.Rendering.Resources;
using ShaderUnit.Rendering.Shaders;
using ShaderUnit.Interfaces;
using ShaderUnit.Interfaces.Shader;

namespace ShaderUnit.Rendering
{
	// Class that takes commands from the script and controls the rendering.
	public class ScriptRenderControl : IDisposable, IRenderInterface
	{
		public ScriptRenderControl(IWorkspace workspace, RenderDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}

			_workspace = workspace;
			_device = device;
		}

		// IScriptRenderInterface implementation.

		public IShader CompileShader(
			string filename, string entryPoint, string profile, IDictionary<string, object> defines = null)
		{
			var path = FindShader(filename);
			if (!File.Exists(path))
				throw new ShaderUnitException("Shader file " + filename + " not found in project.");

			return AddResource(Shader.CompileFromFile(_device.Device,
				path, entryPoint, profile, FindShader, ConvertDefines(defines)));
		}

		// Compile a shader from an in-memory string.
		// All includes still must come from the file system.
		public IShader CompileShaderFromString(string source, string entryPoint, string profile,
			IDictionary<string, object> defines = null)
		{
			// Don't cache shader from string.
			// Not needed in unit testing environment.
			return AddResource(Shader.CompileFromString(
				_device.Device, source, entryPoint, profile, FindShader, ConvertDefines(defines)));
		}

		private ShaderMacro[] ConvertDefines(IDictionary<string, object> defines) =>
			defines
				.EmptyIfNull()
				.Select(define => new ShaderMacro(define.Key, define.Value.ToString()))
				.ToArray();

		// Lookup a shader filename in the project to retrieve the full path.
		private string FindShader(string name)
		{
			var path = _workspace.FindProjectFile(name);
			if (path == null)
			{
				throw new ShaderUnitException("Could not find shader file: " + name);
			}

			return path;
		}

		// Create a render target of dimensions equal to the viewport.
		public object CreateRenderTarget()
		{
			renderTargets.Add(new RenderTargetDescriptor(new SharpDX.DXGI.Rational(1, 1), new SharpDX.DXGI.Rational(1, 1), true));
			return new RenderTargetHandle(renderTargets.Count - 1);
		}

		// Create a 2D texture of the given size and format, and fill it with the given data.
		public ITexture2D CreateTexture2D<T>(int width, int height, Format format, IEnumerable<T> contents, bool generateMips = false) where T : struct
		{
			return AddResource(Texture.Create(_device.Device, width, height, format, contents, generateMips));
		}

		// Load a texture from disk.
		public ITexture2D LoadTexture(string path, bool generateMips = true)
		{
			var absPath = _workspace.GetAbsolutePath(path);
			var texture = Texture.LoadFromFile(_device.Device, absPath, generateMips);
			return AddResource(texture);
		}

		// Create a structured buffer with the given contents.
		public IBuffer CreateStructuredBuffer<T>(IEnumerable<T> contents) where T : struct =>
			AddResource(Resources.Buffer.CreateStructured(_device.Device, contents));

		// Create a structure buffer with the given element size and stride, with undefined contents.
		public IBuffer CreateStructuredBuffer(int sizeInBytes, int elementStride) =>
			AddResource(new Resources.Buffer(_device.Device, sizeInBytes, elementStride, null));

		public void Dispose()
		{
			DisposableUtil.DisposeList(renderTargets);
			DisposableUtil.DisposeList(textures);
			DisposableUtil.DisposeList(_resources);

			_device = null;
		}

		public void Render(SharpDX.Direct3D11.DeviceContext deviceContext, ViewInfo viewInfo, FrameCallback frameCallback)
		{
			if (frameCallback == null)
			{
				throw new ArgumentNullException(nameof(frameCallback));
			}

			// Create render targets if necessary.
			UpdateRenderTargets(viewInfo.ViewportWidth, viewInfo.ViewportHeight);

			// Let the test do its thing.
			var renderContext = new ScriptRenderContext(
				deviceContext,
				viewInfo,
				(from desc in renderTargets select desc.renderTarget).ToArray(),
				_device.GlobalResources);

			frameCallback(renderContext);
		}

		// TODO: No resizing so this is pointless.
		private void UpdateRenderTargets(int viewportWidth, int viewportHeight)
		{
			foreach (var desc in renderTargets)
			{
				int width = desc.GetWidth(viewportWidth);
				int height = desc.GetHeight(viewportHeight);

				// If there's no resource, or it's the wrong size, create a new one.
				if (desc.renderTarget == null || desc.renderTarget.Width != width || desc.renderTarget.Height != height)
				{
					// Don't forget to release the old one.
					desc.renderTarget?.Dispose();

					// TODO: Custom format
					desc.renderTarget = new RenderTarget(_device.Device, width, height, SharpDX.DXGI.Format.R8G8B8A8_UNorm);
				}
			}
		}

		// Get the texture to use for a shader resource variable.
		private Texture GetTexture(object handle)
		{
			if (handle == BlackTexture)
			{
				return _device.GlobalResources.BlackTexture;
			}
			else if (handle == WhiteTexture)
			{
				return _device.GlobalResources.WhiteTexture;
			}
			else if (handle == DefaultNormalTexture)
			{
				return _device.GlobalResources.DefaultNormalTexture;
			}
			else if (handle is TextureHandle)
			{
				// Bind the variable to a texture's SRV.
				int texIndex = ((TextureHandle)handle).index;
				System.Diagnostics.Debug.Assert(texIndex >= 0 && texIndex < textures.Count);

				return textures[texIndex];
			}

			return null;
		}

		// Register a resource for later disposal, returning it for easy chaining.
		private T AddResource<T>(T resource) where T : IDisposable
		{
			_resources.Add(resource);
			return resource;
		}

		public object DepthBuffer => DepthBufferHandle.Default;
		public object NoDepthBuffer => DepthBufferHandle.NoDepthBuffer;

		// These don't need to be anything, we're just going to use them with reference equality checks.
		public object BlackTexture { get; } = new object();
		public object WhiteTexture { get; } = new object();
		public object DefaultNormalTexture { get; } = new object();


		private RenderDevice _device;

		// Script-generated resources.
		private List<Texture> textures = new List<Texture>();

		// List of resource to be disposed of when reseting or disposing.
		private List<IDisposable> _resources = new List<IDisposable>();

		// List of render targets and their descritors.
		private List<RenderTargetDescriptor> renderTargets = new List<RenderTargetDescriptor>();

		// Pointer back to the workspace. Needed so we can access the project to get shaders from.
		private IWorkspace _workspace;
	}
}
