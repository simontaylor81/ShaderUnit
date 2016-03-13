using System;
using System.Linq;
using System.Text;
using System.Numerics;

namespace ShaderUnit.TestRenderer
{
	class HlslTestHarness
	{
		// Use weird names to reduce chance of conflicts with the SUT.
		public static string EntryPoint => "__Main";
		public static string OutBufferName => "__OutputBuffer";
		public static string OutStructName => "__OutStruct";

		public static string GenerateComputeShader(string shaderFile, string function, Type returnType, object[] parameters)
		{
			var result = new StringBuilder();

			var hlslReturnType = HlslTypeConversion.ClrTypeToHlsl(returnType);
			var arguments = string.Join(", ", parameters.Select(HlslTypeConversion.ClrValueToHlslLiteral));

			result.AppendLine($"#include \"{shaderFile}\"");

			// This is slightly messy, but we actually put the returned value inside a struct
			// so we can have type modifiers (e.g. row_major) on the return type.
			// (You can't do RWStructuredBuffer<row_major float4x4> for example, it won't compile).
			result.AppendLine($"struct {OutStructName}");
			result.AppendLine("{");
			result.AppendLine($"	{hlslReturnType} value;");
			result.AppendLine("};");
			result.AppendLine($"RWStructuredBuffer<{OutStructName}> {OutBufferName};");

			result.AppendLine("[numthreads(1, 1, 1)]");
			result.AppendLine($"void {EntryPoint}()");
			result.AppendLine("{");
			result.AppendLine($"\t{OutBufferName}[0].value = {function}({arguments});");
			result.AppendLine("}");

			return result.ToString();
		}
	}
}
