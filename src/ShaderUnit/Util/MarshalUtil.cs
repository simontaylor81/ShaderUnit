using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ShaderUnit.Util
{
	public static class MarshalUtil
	{
		// For some reason, Marshal.SizeOf will throw for generic types even though it works fine.
		// You can bypass the check using the SizeOf(object) overload. This executes the exact same
		// underlying function, so it makes no sense, but hey...
		public static int SizeOf<T>() => Marshal.SizeOf(default(T));
	}
}
