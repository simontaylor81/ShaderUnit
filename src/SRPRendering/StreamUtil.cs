using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace SRPRendering
{
	// Helpers for filling SlimDX streams.
	static class StreamUtil
	{
		// Create a stream from an enumerable (directly, no format conversion).
		public static DataStream ToDataStream<T>(this IEnumerable<T> contents) where T : struct
		{
			var size = contents.Count() * Marshal.SizeOf(typeof(T));
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
