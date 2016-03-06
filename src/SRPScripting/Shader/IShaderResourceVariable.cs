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
		void Set(IShaderResource value);

		void BindToMaterial(string materialParam, IShaderResource fallback = null);
	}
}
