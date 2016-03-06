using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.D3DCompiler;
using SRPScripting;
using System.Numerics;
using SRPCommon.Util;

namespace SRPRendering.Shaders
{
	public interface IShaderVariableBinding
	{
		void UpdateVariable(ViewInfo viewInfo, IPrimitive primitive);
	}

	class SimpleShaderVariableBind : IShaderVariableBinding
	{
		public SimpleShaderVariableBind(ShaderConstantVariable variable, ShaderVariableBindSource source)
		{
			this.variable = variable;
			this.source = source;
		}

		public void UpdateVariable(ViewInfo viewInfo, IPrimitive primitive)
		{
			switch (source)
			{
				case ShaderVariableBindSource.WorldToProjectionMatrix:
					variable.SetValue(viewInfo.WorldToViewMatrix * viewInfo.ViewToProjMatrix);
					return;

				case ShaderVariableBindSource.ProjectionToWorldMatrix:
					{
						var matrix = viewInfo.WorldToViewMatrix * viewInfo.ViewToProjMatrix;
						Matrix4x4.Invert(matrix, out matrix);
						variable.SetValue(matrix);
					}
					return;

				case ShaderVariableBindSource.LocalToWorldMatrix:
					if (primitive != null)
					{
						variable.SetValue(primitive.LocalToWorld);
						return;
					}
					break;

				case ShaderVariableBindSource.WorldToLocalMatrix:
					if (primitive != null)
					{
						var matrix = primitive.LocalToWorld;
						Matrix4x4.Invert(matrix, out matrix);
						variable.SetValue(matrix);
						return;
					}
					break;

				case ShaderVariableBindSource.LocalToWorldInverseTransposeMatrix:
					if (primitive != null)
					{
						var matrix = primitive.LocalToWorld;
						Matrix4x4.Invert(matrix, out matrix);
						variable.SetValue(Matrix4x4.Transpose(matrix));
						return;
					}
					break;

				case ShaderVariableBindSource.CameraPosition:
					variable.SetValue(viewInfo.EyePosition);
					return;
			}

			// If we got this far, the variable was not set, so fall back to the default.
			variable.SetDefault();
		}

		private ShaderConstantVariable variable;
		private ShaderVariableBindSource source;
	}

	class MaterialShaderVariableBind : IShaderVariableBinding
	{
		public MaterialShaderVariableBind(ShaderConstantVariable variable, string source)
		{
			this.variable = variable;
			this.source = source;

			if (variable.VariableType.Type != ShaderVariableType.Float)
			{
				throw new ShaderUnitException(String.Format("Cannot bind shader variable '{0}' to material parameter: only float parameters are supported.", variable.Name));
			}
		}

		public void UpdateVariable(ViewInfo viewInfo, IPrimitive primitive)
		{
			if (primitive != null && primitive.Material != null)
			{
				Vector4 value;
				if (primitive.Material.Parameters.TryGetValue(source, out value))
				{
					var valueArray = value.ToArray();

					// Set each component individually, as the variable might be < 4 components.
					int numComponents = Math.Min(variable.VariableType.Columns * variable.VariableType.Rows, 4);
					for (int i = 0; i < numComponents; i++)
					{
						variable.SetComponent(i, valueArray[i]);
					}

					return;
				}
			}

			// Could not get value from material, so reset to default.
			variable.SetDefault();
		}

		private ShaderConstantVariable variable;
		private string source;
	}

	class DirectShaderVariableBind<T> : IShaderVariableBinding where T : struct
	{
		private T _value;
		private ShaderConstantVariable _variable;

		public DirectShaderVariableBind(ShaderConstantVariable variable, T value)
		{
			_variable = variable;
			_value = value;
		}

		public void UpdateVariable(ViewInfo viewInfo, IPrimitive primitive)
		{
			_variable.SetValue(_value);
		}
	}
}
