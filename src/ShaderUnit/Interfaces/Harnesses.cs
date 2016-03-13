using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShaderUnit.Interfaces.Shader;

namespace ShaderUnit.Interfaces
{
	public interface IComputeHarness
	{
		IRenderInterface RenderInterface { get; }

		void Dispatch(FrameCallback callback);
		IEnumerable<T> DispatchToBuffer<T>(IShader cs, string outBufferVariable, int size) where T : struct;
		IEnumerable<T> DispatchToBuffer<T>(IShader cs, string outBufferVariable, Tuple<int, int, int> size) where T : struct;
		T ExecuteShaderFunction<T>(string shaderFile, string function, params object[] parameters) where T : struct;
	}

	public interface IRenderHarness : IComputeHarness
	{
		Bitmap RenderImage(FrameCallback callback);
		Bitmap RenderFullscreenImage(IShader vs, IShader ps);
	}
}
