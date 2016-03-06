using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.D3DCompiler;
using SRPScripting;
using System.Numerics;
using SRPCommon.Util;

namespace SRPRendering
{
	public interface IShaderVariableBind
	{
		void UpdateVariable(ViewInfo viewInfo, IPrimitive primitive, IDictionary<string, dynamic> overrides);
		bool AllowScriptOverride { get; }
	}

	class SimpleShaderVariableBind : IShaderVariableBind
	{
		public SimpleShaderVariableBind(IShaderVariable variable, ShaderVariableBindSource source)
		{
			this.variable = variable;
			this.source = source;
		}

		public void UpdateVariable(ViewInfo viewInfo, IPrimitive primitive, IDictionary<string, dynamic> overrides)
		{
			switch (source)
			{
				case ShaderVariableBindSource.WorldToProjectionMatrix:
					variable.Set(viewInfo.WorldToViewMatrix * viewInfo.ViewToProjMatrix);
					return;

				case ShaderVariableBindSource.ProjectionToWorldMatrix:
					{
						var matrix = viewInfo.WorldToViewMatrix * viewInfo.ViewToProjMatrix;
						Matrix4x4.Invert(matrix, out matrix);
						variable.Set(matrix);
					}
					return;

				case ShaderVariableBindSource.LocalToWorldMatrix:
					if (primitive != null)
					{
						variable.Set(primitive.LocalToWorld);
						return;
					}
					break;

				case ShaderVariableBindSource.WorldToLocalMatrix:
					if (primitive != null)
					{
						var matrix = primitive.LocalToWorld;
						Matrix4x4.Invert(matrix, out matrix);
						variable.Set(matrix);
						return;
					}
					break;

				case ShaderVariableBindSource.LocalToWorldInverseTransposeMatrix:
					if (primitive != null)
					{
						var matrix = primitive.LocalToWorld;
						Matrix4x4.Invert(matrix, out matrix);
						variable.Set(Matrix4x4.Transpose(matrix));
						return;
					}
					break;

				case ShaderVariableBindSource.CameraPosition:
					variable.Set(viewInfo.EyePosition);
					return;
			}

			// If we got this far, the variable was not set, so fall back to the default.
			variable.SetDefault();
		}

		public bool AllowScriptOverride => false;

		private IShaderVariable variable;
		private ShaderVariableBindSource source;
	}

	class MaterialShaderVariableBind : IShaderVariableBind
	{
		public MaterialShaderVariableBind(IShaderVariable variable, string source)
		{
			this.variable = variable;
			this.source = source;

			if (variable.VariableType.Type != ShaderVariableType.Float)
			{
				throw new ShaderUnitException(String.Format("Cannot bind shader variable '{0}' to material parameter: only float parameters are supported.", variable.Name));
			}
		}

		public void UpdateVariable(ViewInfo viewInfo, IPrimitive primitive, IDictionary<string, dynamic> overrides)
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

		public bool AllowScriptOverride => false;

		private IShaderVariable variable;
		private string source;
	}

	class SimpleShaderVariableBind<T> : IShaderVariableBind where T : struct
	{
		private T _value;
		private IShaderVariable _variable;

		public SimpleShaderVariableBind(IShaderVariable variable, T value)
		{
			_variable = variable;
			_value = value;
		}

		public bool AllowScriptOverride => false;

		public void UpdateVariable(ViewInfo viewInfo, IPrimitive primitive, IDictionary<string, dynamic> overrides)
		{
			_variable.Set(_value);
		}
	}

	// TODO: Remove?
	class ScriptOverrideShaderVariableBind : IShaderVariableBind
	{
		public ScriptOverrideShaderVariableBind(IShaderVariable variable)
		{
			this.variable = variable;
		}

		public void UpdateVariable(ViewInfo viewInfo, IPrimitive primitive, IDictionary<string, dynamic> overrides)
		{
			dynamic overriddenValue;

			// Is the variable overridden this drawcall?
			if (overrides != null && overrides.TryGetValue(variable.Name, out overriddenValue))
			{
				try
				{
					throw new NotImplementedException("TODO: Shader overrides");
					//variable.SetFromDynamic(overriddenValue);
				}
				catch (ShaderUnitException ex)
				{
					throw new ShaderUnitException("Incorrect type for shader variable override: " + variable.Name, ex);
				}
			}
			else
			{
				// Not overridden, so set to default.
				variable.SetDefault();
			}
		}

		public bool AllowScriptOverride => true;

		private IShaderVariable variable;
	}
}
