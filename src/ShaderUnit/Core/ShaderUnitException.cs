using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderUnit
{
	public class ShaderUnitException : Exception
	{
		public ShaderUnitException()
		{ }
		public ShaderUnitException(string message)
			: base(message)
		{ }
		public ShaderUnitException(string message, Exception innerException)
			: base(message, innerException)
		{ }
	}
}
