using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPScripting.Shader
{
	// Shader resource (texture, buffer, etc.) variable.
	public interface IShaderResourceVariable : IShaderVariable
	{
		// TODO: Shader resource abstraction.
		void Set(object value);

		void BindToMaterial(string materialParam, object fallback = null);
	}
}
