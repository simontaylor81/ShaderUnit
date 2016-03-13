using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShaderUnit.Util;
using SharpDX;

namespace ShaderUnit.Rendering
{
	// Helpers for filling SlimDX streams.
	static class StreamUtil
	{
		// Create a stream from an enumerable (directly, no format conversion).
		public static DataStream ToDataStream<T>(this IEnumerable<T> contents) where T : struct
		{
			var size = contents.Count() * MarshalUtil.SizeOf<T>();
			var result = new DataStream(size, true, true);
			foreach (var element in contents)
			{
				result.Write(element);
			}

			// Reset position
			result.Position = 0;
			return result;
		}
	}
}
