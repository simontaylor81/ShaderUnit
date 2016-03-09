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

			var hlslReturnType = ClrTypeToHlsl(returnType);
			var arguments = string.Join(", ", parameters.Select(ClrValueToHlslLiteral));

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

		public static string ClrTypeToHlsl(Type type)
		{
			if (type == typeof(float))
			{
				return "float";
			}
			else if (type == typeof(int))
			{
				return "int";
			}
			else if (type == typeof(uint))
			{
				return "uint";
			}
			else if (type == typeof(Vector2))
			{
				return "float2";
			}
			else if (type == typeof(Vector3))
			{
				return "float3";
			}
			else if (type == typeof(Vector4))
			{
				return "float4";
			}
			else if (type == typeof(Matrix4x4))
			{
				// System.Numerics.Matrix4x4 is row-major in memory, so match that.
				return "row_major float4x4";
			}

			throw new ArgumentException($"Type cannot be converted to HLSL: {type.ToString()}", nameof(type));
		}

		public static string ClrValueToHlslLiteral(object value)
		{
			var type = value.GetType();
			if (type == typeof(float) || type == typeof(int) || type == typeof(uint))
			{
				// Scalars are easy.
				return value.ToString();
			}
			else if (type == typeof(Vector2))
			{
				var vec = (Vector2)value;
				return $"float2({vec.X}, {vec.Y})";
			}
			else if (type == typeof(Vector3))
			{
				var vec = (Vector3)value;
				return $"float3({vec.X}, {vec.Y}, {vec.Z})";
			}
			else if (type == typeof(Vector4))
			{
				var vec = (Vector4)value;
				return $"float4({vec.X}, {vec.Y}, {vec.Z}, {vec.W})";
			}
			else if (type == typeof(Matrix4x4))
			{
				var m = (Matrix4x4)value;
				return $"float4x4({m.M11}, {m.M12}, {m.M13}, {m.M14}, {m.M21}, {m.M22}, {m.M23}, {m.M24}, {m.M31}, {m.M32}, {m.M33}, {m.M34}, {m.M41}, {m.M42}, {m.M43}, {m.M44})";
			}

			throw new ArgumentException($"Value cannot be converted to HLSL: {value.ToString()}", nameof(value));
		}
	}
}
