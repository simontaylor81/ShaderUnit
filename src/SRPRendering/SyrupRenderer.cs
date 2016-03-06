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
			_device = device;

			// Create object for interacting with script.
			_scriptRenderControl = new ScriptRenderControl(workspace, device);
			_disposables.Add(_scriptRenderControl);

			ScriptInterface = _scriptRenderControl;
		}

		public void Dispose()
		{
			_disposables.Dispose();
			_device = null;
		}

		public void Render(DeviceContext deviceContext, ViewInfo viewInfo)
		{
			// Always clear the back buffer to black to avoid the script having to do so for trivial stuff.
			deviceContext.ClearRenderTargetView(viewInfo.BackBuffer, new RawColor4());

			// Let the script do its thing.
			_scriptRenderControl.Render(deviceContext, viewInfo);
		}

		// Wrapper class that gets given to the script, acting as a firewall to prevent it from accessing this class directly.
		public IRenderInterface ScriptInterface { get; }

		private RenderDevice _device;

		// List of things to dispose.
		private CompositeDisposable _disposables = new CompositeDisposable();

		private readonly ScriptRenderControl _scriptRenderControl;
	}
}
