﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ShaderUnit.Interfaces;
using ShaderUnit.Interfaces.Shader;
using ShaderUnit.Rendering;
using ShaderUnit.Util;

namespace ShaderUnit.TestRenderer
{
	class RenderTestHarness : IRenderHarness, IDisposable
	{
		private readonly ShaderUnitRenderer _renderer;
		private readonly TestWorkspace _workspace;
		private readonly ScriptRenderControl _src;

		private static bool bLoggedDevice = false;

		public IRenderInterface RenderInterface => _src;

		public RenderTestHarness(ShaderUnitRenderer renderer, string assetDir)
		{
			_renderer = renderer;
			_workspace = new TestWorkspace(assetDir);

			// Minor hack to avoid spamming the log with device names.
			if (!bLoggedDevice)
			{
				// Write adapter description to the console, since it can affect results.
				// Trim weird null characters that appear from somewhere.
				var deviceName = _renderer.Device.Adapter.Description.Description.Trim('\0');
				Console.WriteLine($"RenderTestHarness: Using device '{deviceName}'");
				bLoggedDevice = true;
			}

			// Create syrup renderer to drive the rendering.
			_src = new ScriptRenderControl(_workspace, _renderer.Device);
		}

		public void Dispose()
		{
			_renderer.Dispose();
			_src.Dispose();
		}

		public Bitmap RenderImage(FrameCallback callback)
		{
			// Render stuff and return the resulting image.
			return _renderer.Render(_src, callback);
		}

		// Helper for the common case of rendering a fullscreen quad.
		public Bitmap RenderFullscreenImage(IShader vs, IShader ps)
		{
			return RenderImage(context =>
			{
				context.DrawFullscreenQuad(vs, ps);
			});
		}

		// Dispatch with a frame callback.
		public void Dispatch(FrameCallback callback)
		{
			// Run the renderer to trigger compute shaders.
			_renderer.Dispatch(_src, callback);
		}

		// Dispatch with a single compute shader.
		public void Dispatch(IShader cs, int numThreadGroupsX, int numThreadGroupsY, int numThreadGroupsZ)
		{
			Dispatch(context => context.Dispatch(cs, numThreadGroupsX, numThreadGroupsY, numThreadGroupsZ));
		}

		// Simple wrapper for the common 1D case.
		public IEnumerable<T> DispatchToBuffer<T>(IShader cs, string outBufferVariable, int size) where T : struct =>
			DispatchToBuffer<T>(cs, outBufferVariable, Tuple.Create(size, 1, 1));

		// Dispatch the given compute shader to a buffer that contains the result of each thread.
		public IEnumerable<T> DispatchToBuffer<T>(IShader cs, string outBufferVariable, Tuple<int, int, int> size) where T : struct
		{
			// Create buffer to hold results.
			var bufferStride = MarshalUtil.SizeOf<T>();
			var bufferSize = size.Item1 * size.Item2 * size.Item3 * bufferStride;
			var outputBuffer = RenderInterface.CreateStructuredBuffer(bufferSize, bufferStride);
			cs.FindUavVariable(outBufferVariable).Set(outputBuffer);

			int numThreadGroupsX = DivideCeil(size.Item1, cs.ThreadGroupSize.Item1);
			int numThreadGroupsY = DivideCeil(size.Item2, cs.ThreadGroupSize.Item2);
			int numThreadGroupsZ = DivideCeil(size.Item3, cs.ThreadGroupSize.Item3);

			// Render a frame to dispatch the compute shader.
			Dispatch(cs, numThreadGroupsX, numThreadGroupsY, numThreadGroupsZ);

			// Read results back from the buffer
			return outputBuffer.GetContents<T>();
		}

		// Integer division with round up instead of down.
		private int DivideCeil(int x, int multipleOf) => (x + multipleOf - 1) / multipleOf;

		public T ExecuteShaderFunction<T>(string shaderFile, string function, params object[] parameters) where T : struct
		{
			string shader = HlslTestHarness.GenerateComputeShader(shaderFile, function, typeof(T), parameters);
			var cs = RenderInterface.CompileShaderFromString(shader, HlslTestHarness.EntryPoint, "cs_5_0");

			return DispatchToBuffer<T>(cs, HlslTestHarness.OutBufferName, 1).Single();
		}
	}
}
