using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderUnit.Interfaces
{
	// A handle to a D3D Buffer
	public interface IBuffer : IShaderResource
	{
		IEnumerable<T> GetContents<T>() where T : struct;
	}
}
