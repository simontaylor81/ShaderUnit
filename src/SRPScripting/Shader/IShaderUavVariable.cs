using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPScripting.Shader
{
	public interface IShaderUavVariable : IShaderVariable
	{
		// TODO: UAV resource abstraction.
		void Set(IBuffer buffer);
	}
}
