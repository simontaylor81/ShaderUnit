using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPCommon.Util
{
	public class ShaderUnitException : Exception
	{
		public ShaderUnitException()
			: base()
		{ }
		public ShaderUnitException(string message)
			: base(message)
		{ }
		public ShaderUnitException(string message, Exception innerException)
			: base(message, innerException)
		{ }
	}
}
