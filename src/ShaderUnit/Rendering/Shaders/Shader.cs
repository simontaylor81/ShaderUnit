using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;

using ShaderUnit.Util;
using System.Text.RegularExpressions;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX;
using SharpDX.Direct3D;
using ShaderUnit.Interfaces.Shader;

namespace ShaderUnit.Rendering.Shaders
{
	class Shader : IShader, IDisposable
	{
		private Shader(Device device, string profile, IncludeHandler includeHandler, Func<ShaderBytecode> compiler)
		{
			// Compile the shader to bytecode.
			using (var bytecode = compiler())
			{
				IncludedFiles = includeHandler != null ? includeHandler.IncludedFiles : Enumerable.Empty<IncludedFile>();

				// Create the shader object of the appropriate type.
				switch (profile.Substring(0, 2))
				{
					case "vs":
						_vertexShader = new VertexShader(device, bytecode);
						Signature = ShaderSignature.GetInputSignature(bytecode);
						Frequency = ShaderFrequency.Vertex;
						break;

					case "ps":
						_pixelShader = new PixelShader(device, bytecode);
						Frequency = ShaderFrequency.Pixel;
						break;

					case "cs":
						_computeShader = new ComputeShader(device, bytecode);
						Frequency = ShaderFrequency.Compute;
						break;

					default:
						throw new Exception("Unsupported shader profile: " + profile);
				}

				// Get info about the shader's inputs.
				using (var reflection = new ShaderReflection(bytecode))
				{
					// Gether constant buffers.
					_cbuffers = reflection.GetConstantBuffers()
						.Where(cbuffer => cbuffer.Description.Type == ConstantBufferType.ConstantBuffer)
						.Select(cbuffer => new ConstantBuffer(device, cbuffer))
						.ToArray();
					_cbuffers_buffers = _cbuffers.Select(cbuffer => cbuffer.Buffer).ToArray();

					// Gather resource and sampler inputs.
					var boundResources = reflection.GetBoundResources();
					_resourceVariables = boundResources
						.Where(desc => IsShaderResource(desc.Type))
						.Select(desc => new ShaderResourceVariable(desc, Frequency))
						.ToArray();
					_samplerVariables = boundResources
						.Where(desc => desc.Type == ShaderInputType.Sampler)
						.Select(desc => new ShaderSamplerVariable(desc, Frequency))
						.ToArray();
					_uavVariables = boundResources
						.Where(desc => IsUav(desc.Type))
						.Select(desc => new ShaderUavVariable(desc, Frequency))
						.ToArray();
				}
			}
		}

		// Create a new shader by compiling a file.
		public static Shader CompileFromFile(Device device, string filename, string entryPoint, string profile,
			Func<string, string> includeLookup, ShaderMacro[] defines)
		{
			try
			{
				var includeHandler = includeLookup != null ? new IncludeHandler(includeLookup) : null;
				return new Shader(device, profile, includeHandler,
					() => ShaderBytecode.CompileFromFile(filename, entryPoint, profile, ShaderFlags.None, EffectFlags.None, defines, includeHandler));
			}
			catch (CompilationException ex)
			{
				throw TranslateException(ex, filename, includeLookup);
			}
		}

		// Create a new shader compiled from in-memory string.
		// Includes still come from the file system.
		public static Shader CompileFromString(Device device, string source, string entryPoint, string profile,
			Func<string, string> includeLookup, ShaderMacro[] defines)
		{
			try
			{
				var includeHandler = includeLookup != null ? new IncludeHandler(includeLookup) : null;
				return new Shader(device, profile, includeHandler,
					() => ShaderBytecode.Compile(source, entryPoint, profile, ShaderFlags.None, EffectFlags.None, defines, includeHandler));
			}
			catch (CompilationException ex)
			{
				throw TranslateException(ex, "<string>", includeLookup);
			}
		}

		// IShader interface.

		// Find a variable by name.
		public IShaderConstantVariable FindConstantVariable(string name)
		{
			var variable = Variables.FirstOrDefault(v => v.Name == name);
			if (variable == null)
			{
				throw new ShaderUnitException("Could not find shader variable: " + name);
			}
			return variable;
		}

		// Find a resource variable by name.
		public IShaderResourceVariable FindResourceVariable(string name)
		{
			var variable = _resourceVariables.FirstOrDefault(v => v.Name == name);
			if (variable == null)
			{
				throw new ShaderUnitException("Could not find shader resource variable: " + name);
			}
			return variable;
		}

		// Find a sampler variable by name.
		public IShaderSamplerVariable FindSamplerVariable(string name)
		{
			var variable = _samplerVariables.FirstOrDefault(v => v.Name == name);
			if (variable == null)
			{
				throw new ShaderUnitException("Could not find shader sampler variable: " + name);
			}
			return variable;
		}

		// Find a UAV variable by name.
		public IShaderUavVariable FindUavVariable(string name)
		{
			var variable = _uavVariables.FirstOrDefault(v => v.Name == name);
			if (variable == null)
			{
				throw new ShaderUnitException("Could not find shader UAV variable: " + name);
			}
			return variable;
		}

		// Frequency (i.e. type) of shader.
		public ShaderFrequency Frequency { get; }

		// List of files that were included by this shader.
		public IEnumerable<IncludedFile> IncludedFiles { get; }

		private static Exception TranslateException(CompilationException ex, string baseFilename, Func<string, string> includeLookup)
		{
			// The shader compiler error messages contain the name used to
			// include the file, rather than the full path, so we convert them back
			// with some regex fun.

			var filenameRegex = new Regex(@"^(.*)(\([0-9]+,[0-9]+\))", RegexOptions.Multiline);

			// When compiling from string, the errors come from some weird non-existant path.
			var inMemoryFileRegex = new Regex(@"Shader@0x[0-9A-F]{8}$");

			MatchEvaluator replacer = match =>
			{
				var matchedFile = match.Groups[1].Value;
				
				// If the filename is the original input filename, or the weird in-memory file, use the given name.
				string path;
				if (matchedFile == baseFilename || inMemoryFileRegex.IsMatch(matchedFile))
				{
					path = baseFilename;
				}
				else
				{
					// Otherwise run it through the include lookup function again.
					path = includeLookup(matchedFile);
				}

				// Add back the line an column numbers.
				return path + match.Groups[2];
			};

			var message = filenameRegex.Replace(ex.Message, replacer);

			return new ShaderUnitException(message, ex);
		}

		// Get all variables from all cbuffers.
		private IEnumerable<ShaderConstantVariable> Variables => _cbuffers.SelectMany(cbuffer => cbuffer.Variables);

		private static bool IsShaderResource(ShaderInputType type) =>
			type == ShaderInputType.Texture ||
			type == ShaderInputType.Structured ||
			type == ShaderInputType.ByteAddress;

		private bool IsUav(ShaderInputType type) =>
			type == ShaderInputType.UnorderedAccessViewRWTyped ||
			type == ShaderInputType.UnorderedAccessViewRWStructured ||
			type == ShaderInputType.UnorderedAccessViewRWStructuredWithCounter ||
			type == ShaderInputType.UnorderedAccessViewRWByteAddress;

		public void Dispose()
		{
			foreach (var cbuffer in _cbuffers)
				cbuffer.Dispose();

			DisposableUtil.SafeDispose(_vertexShader);
			DisposableUtil.SafeDispose(_pixelShader);
			DisposableUtil.SafeDispose(_computeShader);
			DisposableUtil.SafeDispose(Signature);
		}


		// Bind the shader to the device.
		public void Set(DeviceContext context)
		{
			if (_vertexShader != null)
			{
				context.VertexShader.Set(_vertexShader);
				context.VertexShader.SetConstantBuffers(0, _cbuffers_buffers);
			}
			else if (_pixelShader != null)
			{
				context.PixelShader.Set(_pixelShader);
				context.PixelShader.SetConstantBuffers(0, _cbuffers_buffers);
			}
			else if (_computeShader != null)
			{
				context.ComputeShader.Set(_computeShader);
				context.ComputeShader.SetConstantBuffers(0, _cbuffers_buffers);
			}
		}

		// Upload constants if required.
		public void UpdateVariables(DeviceContext context, ViewInfo viewInfo, IGlobalResources globalResources)
		{
			// First, update the value of bound and overridden variables.
			foreach (var variable in Variables)
			{
				// Is the variable bound?
				if (variable.Binding != null)
				{
					variable.Binding.UpdateVariable(viewInfo);
				}
			}

			// Next, do the actual upload the constant buffers.
			foreach (var cubffer in _cbuffers)
			{
				cubffer.Update(context);
			}

			// Update resource variables too.
			foreach (var resourceVariable in _resourceVariables)
			{
				if (resourceVariable.Binding != null)
				{
					resourceVariable.Resource = resourceVariable.Binding.GetResource(viewInfo, globalResources);
				}

				resourceVariable.SetToDevice(context);
			}

			// And samplers.
			foreach (var samplerVariable in _samplerVariables)
			{
				samplerVariable.SetToDevice(context, globalResources);
			}

			// And UAVs.
			foreach (var uavVariable in _uavVariables)
			{
				uavVariable.SetToDevice(context);
			}
		}

		// Actual shader. Only one of these is non-null.
		private VertexShader _vertexShader;
		private PixelShader _pixelShader;
		private ComputeShader _computeShader;

		// Input signature. Vertex shader only.
		public ShaderSignature Signature { get; }

		// Constant buffer info.
		private ConstantBuffer[] _cbuffers;
		private SharpDX.Direct3D11.Buffer[] _cbuffers_buffers;

		// Resource variable info.
		private ShaderResourceVariable[] _resourceVariables;

		// Sampler input info.
		private ShaderSamplerVariable[] _samplerVariables;

		// UAV variable info.
		private ShaderUavVariable[] _uavVariables;

		// Class for handling include file lookups.
		private class IncludeHandler : CallbackBase, Include
		{
			private Func<string, string> includeLookup;

			private List<IncludedFile> _includedFiles = new List<IncludedFile>();
			public IEnumerable<IncludedFile> IncludedFiles => _includedFiles;

			public IncludeHandler(Func<string, string> includeLookup)
			{
				this.includeLookup = includeLookup;
			}

			// Include interface.
			public Stream Open(IncludeType type, string filename, Stream parentStream)
			{
				// Find full path.
				var path = includeLookup(filename);

				// Remember that we included this file.
				_includedFiles.Add(new IncludedFile { SourceName = filename, ResolvedFile = path });

				// Open file stream.
				return new FileStream(path, FileMode.Open);
			}

			public void Close(Stream stream)
			{
				stream.Dispose();
			}
		}
	}
}
