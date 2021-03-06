﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShaderUnit.Interfaces.Shader;

namespace ShaderUnit.Interfaces
{
	public enum ShaderVariableBindSource
	{
		WorldToProjectionMatrix,	// Bind to the combined world to projection space matrix.
		ProjectionToWorldMatrix,	// Bind to the combined projection to world space matrix (i.e. the inverse of the WorldToProjectionMatrix).
		CameraPosition,				// Bind to the position of the camera in world-space.
	}

	// Delegate type for the per-frame callback. Cannot be inside the interface cos C# is silly.
	public delegate void FrameCallback(IRenderContext context);

	// Interface to the rendering system exposed to the scripting system.
	public interface IRenderInterface
	{
		IShader CompileShader(string filename, string entryPoint, string profile,
			IDictionary<string, object> defines = null);

		// Compile a shader from an in-memory string.
		// All includes still must come from the file system.
		IShader CompileShaderFromString(string source, string entryPoint, string profile,
			IDictionary<string, object> defines = null);

		// Create a render target of dimensions equal to the viewport.
		object CreateRenderTarget();

		// Create a 2D texture of the given size and format, and fill it with the given data.
		ITexture2D CreateTexture2D<T>(int width, int height, Format format, IEnumerable<T> contents, bool generateMips = false) where T : struct;

		// Create a structured buffer with the given contents.
		IBuffer CreateStructuredBuffer<T>(IEnumerable<T> contents) where T : struct;

		// Create a structure buffer with the given element size and stride, with undefined contents.
		IBuffer CreateStructuredBuffer(int sizeInBytes, int elementStride);

		// Load a texture from a file.
		ITexture2D LoadTexture(string path, bool generateMips = true);

		// Handles to special resources.
		//object BackBuffer { get; }
		object DepthBuffer { get; }
		object NoDepthBuffer { get; }

		object BlackTexture { get; }
		object WhiteTexture { get; }
		object DefaultNormalTexture { get; }
	}
}
