using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderUnit.Interfaces.Shader
{
	public enum ShaderFrequency
	{
		Vertex,
		Pixel,
		Compute,
		MAX
	}

	public struct IncludedFile
	{
		public string SourceName;       // Original name from the #include line.
		public string ResolvedFile;     // File path that it was resolved to.
	}

	// A shader.
	public interface IShader
	{
		// Find a variable by name.
		IShaderConstantVariable FindConstantVariable(string name);

		// Find a resource variable by name.
		IShaderResourceVariable FindResourceVariable(string name);

		// Find a sample variable by name.
		IShaderSamplerVariable FindSamplerVariable(string name);

		// Find a UAV variable by name.
		IShaderUavVariable FindUavVariable(string name);

		// Frequency (i.e. type) of shader.
		ShaderFrequency Frequency { get; }

		// Number of threads in thread group in x, y, and z dimensions.
		// Only valid for compute shaders.
		Tuple<int, int, int> ThreadGroupSize { get; }

		// List of files that were included by this shader.
		IEnumerable<IncludedFile> IncludedFiles { get; }
	}
}
