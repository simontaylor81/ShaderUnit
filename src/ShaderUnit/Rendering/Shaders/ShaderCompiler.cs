using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ShaderUnit.Interfaces.Shader;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace ShaderUnit.Rendering.Shaders
{
	static class ShaderCompiler
	{
		// Create a new shader by compiling a file.
		public static Shader CompileFromFile(Device device, string filename, string entryPoint, string profile,
			Func<string, string> includeLookup, ShaderMacro[] defines)
		{
			// SharpDX doesn't throw for *all* shader compile errors, so simplify things an throw for none.
			Configuration.ThrowOnShaderCompileError = false;

			var includeHandler = includeLookup != null ? new IncludeHandler(includeLookup) : null;
			using (var compilationResult = ShaderBytecode.CompileFromFile(filename, entryPoint, profile, ShaderFlags.None, EffectFlags.None, defines, includeHandler))
			{
				// Don't check HasErrors or the status code, since apparent they sometimes
				// indicate succes even when the compile failed!
				if (compilationResult.Bytecode == null)
				{
					throw TranslateErrors(compilationResult, filename, includeLookup);
				}

				var includedFiles = includeHandler != null ? includeHandler.IncludedFiles : Enumerable.Empty<IncludedFile>();
				return new Shader(device, profile, includedFiles, compilationResult.Bytecode);
			}
		}

		// Create a new shader compiled from in-memory string.
		// Includes still come from the file system.
		public static Shader CompileFromString(Device device, string source, string entryPoint, string profile,
			Func<string, string> includeLookup, ShaderMacro[] defines)
		{
			// SharpDX doesn't throw for *all* shader compile errors, so simplify things an throw for none.
			Configuration.ThrowOnShaderCompileError = false;

			var includeHandler = includeLookup != null ? new IncludeHandler(includeLookup) : null;
			using (var compilationResult = ShaderBytecode.Compile(source, entryPoint, profile, ShaderFlags.None, EffectFlags.None, defines, includeHandler))
			{
				// Don't check HasErrors or the status code, since apparent they sometimes
				// indicate succes even when the compile failed!
				if (compilationResult.Bytecode == null)
				{
					throw TranslateErrors(compilationResult, "<string>", includeLookup);
				}

				var includedFiles = includeHandler != null ? includeHandler.IncludedFiles : Enumerable.Empty<IncludedFile>();
				return new Shader(device, profile, includedFiles, compilationResult.Bytecode);
			}
		}

		private static Exception TranslateErrors(CompilationResult result, string baseFilename, Func<string, string> includeLookup)
		{
			// The shader compiler error messages contain the name used to
			// include the file, rather than the full path, so we convert them back
			// with some regex fun.

			var filenameRegex = new Regex(@"^(.*)(\([0-9]+,[0-9\-]+\))", RegexOptions.Multiline);

			// SharpDX always passes a string to D3D, so errors in the original file (or string) are reported incorrectly.
			// For whatever reason, they show up as being in a file "unknown" in the current working directory.
			var unknownFile = Path.Combine(Environment.CurrentDirectory, "unknown");

			MatchEvaluator replacer = match =>
			{
				var matchedFile = match.Groups[1].Value;

				// If the filename is the original input filename, or the weird in-memory file, use the given name.
				string path;
				if (matchedFile == unknownFile)
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

			var message = filenameRegex.Replace(result.Message, replacer);

			return new ShaderUnitException(message);
		}

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
				return new FileStream(path, FileMode.Open, FileAccess.Read);
			}

			public void Close(Stream stream)
			{
				stream.Dispose();
			}
		}
	}
}
