using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SharpDX.Direct3D;
using SRPCommon.Interfaces;
using SRPCommon.Scene;
using SRPCommon.Util;
using SRPRendering.Resources;
using SRPRendering.Shaders;
using SRPScripting;

namespace SRPRendering
{
	// Class that takes commands from the script and controls the rendering.
	class ScriptRenderControl : IDisposable, IRenderInterface
	{
		public ScriptRenderControl(IWorkspace workspace, RenderDevice device)
		{
			_workspace = workspace;
			_device = device;

			_mipGenerator = new MipGenerator(device, workspace);
		}

		public void Reset()
		{
			frameCallback = null;

			// Clear render target descriptors and dispose the actual render targets.
			DisposableUtil.DisposeList(renderTargets);
			DisposableUtil.DisposeList(textures);

			// Dispose resources registered for cleanup.
			DisposableUtil.DisposeList(_resources);
		}

		// IScriptRenderInterface implementation.

		// Set the master per-frame callback that lets the script control rendering.
		public void SetFrameCallback(FrameCallback callback)
		{
			frameCallback = callback;
		}

		public SRPScripting.Shader.IShader CompileShader(
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
		public SRPScripting.Shader.IShader CompileShaderFromString(string source, string entryPoint, string profile,
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
		public object CreateTexture2D(int width, int height, Format format, dynamic contents, bool generateMips = false)
		{
			throw new NotImplementedException("TODO: Dynamic texture generation");
			//textures.Add(Texture.CreateFromScript(_device.Device, width, height, format, contents, generateMips));
			//return new TextureHandle(textures.Count - 1);
		}

		// Load a texture from disk.
		public object LoadTexture(string path, object generateMips = null)
		{
			var absPath = _workspace.GetAbsolutePath(path);

			// Ugh, Castle DynamicProxy doesn't pass through the null default value, so detect it.
			if (generateMips == System.Reflection.Missing.Value)
			{
				generateMips = null;
			}

			MipGenerationMode mipGenerationMode = MipGenerationMode.None;
			if (generateMips == null || generateMips.Equals(true))
			{
				mipGenerationMode = MipGenerationMode.Full;
			}
			else if (generateMips is string)
			{
				mipGenerationMode = MipGenerationMode.CreateOnly;
			}

			Texture texture;
			try
			{
				texture = Texture.LoadFromFile(_device.Device, absPath, mipGenerationMode);
			}
			catch (FileNotFoundException ex)
			{
				throw new ShaderUnitException("Could not file texture file: " + absPath, ex);
			}
			catch (Exception ex)
			{
				throw new ShaderUnitException("Error loading texture file: " + absPath, ex);
			}

			// We want mip generation errors to be reported directly, so this is
			// outside the above try-catch.
			if (mipGenerationMode == MipGenerationMode.CreateOnly)
			{
				// Generate custom mips.
				_mipGenerator.Generate(texture, generateMips as string);
			}

			textures.Add(texture);
			return new TextureHandle(textures.Count - 1);
		}

		// Create a structured buffer.
		public IBuffer CreateStructuredBuffer<T>(IEnumerable<T> contents, bool uav = false) where T : struct =>
			AddResource(BufferHandle.CreateStructured(_device, uav, contents));

		public void Dispose()
		{
			Reset();

			DisposableUtil.DisposeList(renderTargets);

			_device = null;
		}

		public void Render(SharpDX.Direct3D11.DeviceContext deviceContext, ViewInfo viewInfo, RenderScene renderScene)
		{
			// Create render targets if necessary.
			UpdateRenderTargets(viewInfo.ViewportWidth, viewInfo.ViewportHeight);

			// Let the script do its thing.
			if (frameCallback != null)
			{
				var renderContext = new ScriptRenderContext(
					deviceContext,
					viewInfo,
					renderScene,
					(from desc in renderTargets select desc.renderTarget).ToArray(),
					_device.GlobalResources);

				frameCallback(renderContext);
			}
		}

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

		public dynamic GetScene() => Scene;

		public object DepthBuffer => DepthBufferHandle.Default;
		public object NoDepthBuffer => DepthBufferHandle.NoDepthBuffer;

		// These don't need to be anything, we're just going to use them with reference equality checks.
		public object BlackTexture { get; } = new object();
		public object WhiteTexture { get; } = new object();
		public object DefaultNormalTexture { get; } = new object();


		public Scene Scene { get; set; }

		private RenderDevice _device;

		// Script-generated resources.
		private List<Texture> textures = new List<Texture>();

		// List of resource to be disposed of when reseting or disposing.
		private List<IDisposable> _resources = new List<IDisposable>();

		// List of render targets and their descritors.
		private List<RenderTargetDescriptor> renderTargets = new List<RenderTargetDescriptor>();

		// Master callback that we call each frame.
		private FrameCallback frameCallback;

		// Pointer back to the workspace. Needed so we can access the project to get shaders from.
		private IWorkspace _workspace;

		private readonly MipGenerator _mipGenerator;
	}
}
