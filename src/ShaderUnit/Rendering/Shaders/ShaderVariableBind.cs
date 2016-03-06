using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using ShaderUnit.Interfaces;

namespace ShaderUnit.Rendering.Shaders
{
	public interface IShaderVariableBinding
	{
		void UpdateVariable(ViewInfo viewInfo);
	}

	class SimpleShaderVariableBind : IShaderVariableBinding
	{
		public SimpleShaderVariableBind(ShaderConstantVariable variable, ShaderVariableBindSource source)
		{
			this.variable = variable;
			this.source = source;
		}

		public void UpdateVariable(ViewInfo viewInfo)
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

	class DirectShaderVariableBind<T> : IShaderVariableBinding where T : struct
	{
		private T _value;
		private ShaderConstantVariable _variable;

		public DirectShaderVariableBind(ShaderConstantVariable variable, T value)
		{
			_variable = variable;
			_value = value;
		}

		public void UpdateVariable(ViewInfo viewInfo)
		{
			_variable.SetValue(_value);
		}
	}
}
