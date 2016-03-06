using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using SharpDX;
using SharpDX.Direct3D11;
using SRPCommon.Interfaces;
using SRPCommon.Scene;
using SRPCommon.Util;
using SRPScripting;
using SharpDX.Mathematics.Interop;

namespace SRPRendering
{
	// Central render loop logic.
	public class SyrupRenderer : IDisposable
	{
		public SyrupRenderer(IWorkspace workspace, RenderDevice device)
		{
			_workspace = workspace;
			_device = device;

			// Create object for interacting with script.
			_scriptRenderControl = new ScriptRenderControl(workspace, device);
			_disposables.Add(_scriptRenderControl);

			ScriptInterface = _scriptRenderControl;
		}

		private void Reset()
		{
			_scriptRenderControl.Reset();
		}

		public Scene Scene
		{
			get { return _scene; }
			set
			{
				if (_scene != value)
				{
					_scene = value;
					_scriptRenderControl.Scene = value;

					// Dispose of the old render scene.
					DisposableUtil.SafeDispose(_renderScene);

					// Create new one.
					_renderScene = new RenderScene(_scene, _device);

					// Missing scene can cause rendering to fail -- give it another try with the new one.
					bScriptRenderError = false;
				}
			}
		}

		public void Dispose()
		{
			Reset();

			_disposables.Dispose();
			DisposableUtil.SafeDispose(_renderScene);
			_device = null;
		}

		public void Render(DeviceContext deviceContext, ViewInfo viewInfo)
		{
			// Bail if there was a problem with the scripts.
			if (HasScriptError)
			{
				return;
			}

			try
			{
				// Always clear the back buffer to black to avoid the script having to do so for trivial stuff.
				deviceContext.ClearRenderTargetView(viewInfo.BackBuffer, new RawColor4());

				// Let the script do its thing.
				_scriptRenderControl.Render(deviceContext, viewInfo, _renderScene);
			}
			catch (Exception)
			{
				// Remember that the script fails so we don't just fail over and over.
				bScriptRenderError = true;

				// TODO: How do we handle exceptions here?
				throw;
			}

			// Make sure we're rendering to the back buffer before rendering the overlay.
			deviceContext.OutputMerger.SetTargets(viewInfo.DepthBuffer.DSV, viewInfo.BackBuffer);
			deviceContext.Rasterizer.SetViewports(new[] { new RawViewportF
			{
				X = 0.0f,
				Y = 0.0f,
				Width = viewInfo.ViewportWidth,
				Height = viewInfo.ViewportHeight,
				MinDepth = 0.0f,
				MaxDepth = 0.0f,
			} });
		}

		// Wrapper class that gets given to the script, acting as a firewall to prevent it from accessing this class directly.
		public IRenderInterface ScriptInterface { get; }

		private RenderDevice _device;

		// Pointer back to the workspace. Needed so we can access the project to get shaders from.
		private IWorkspace _workspace;

		private bool bScriptExecutionError = false;		// True if there was a problem executing the script
		private bool bScriptRenderError = false;        // True if there was a script error while rendering

		// If true, previous rendering failed with a script problem, so we don't keep re-running until the script is fixed & re-run.
		public bool HasScriptError => bScriptExecutionError || bScriptRenderError;

		// Renderer representation of the scene we're currently rendering
		private RenderScene _renderScene;

		// Original scene data the above was created from.
		private Scene _scene;

		// List of things to dispose.
		private CompositeDisposable _disposables = new CompositeDisposable();

		private readonly ScriptRenderControl _scriptRenderControl;
	}
}
