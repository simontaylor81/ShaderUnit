using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderUnit.Interfaces.Shader
{
	// A shader variable containing raw constant data (float/int) that goes in a constant buffer.
	public interface IShaderConstantVariable : IShaderVariable
	{
		// Set directly to a given value.
		void Set<T>(T value) where T : struct;

		// Bind to camera property.
		void Bind(ShaderVariableBindSource bindSource);
	}
}
