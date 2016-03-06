using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPCommon.Util
{
	// Misc path utils.
	public static class PathUtils
	{
		// Compare two paths for equality.
		public static bool PathsEqual(string path1, string path2)
		{
			// Note: this doesn't support non-windows platforms currently.
			return Path.GetFullPath(path1).Equals(Path.GetFullPath(path2), StringComparison.OrdinalIgnoreCase);
		}

		// Search up a directory tree looking for a given sub-directory.
		public static string FindPathInTree(string startPath, string toFind)
		{
			var path = startPath;

			// Search up directory tree looking for toFind.
			while (!Directory.Exists(Path.Combine(path, toFind)))
			{
				var newDir = Path.GetFullPath(Path.Combine(path, ".."));
				if (newDir == path)
				{
					// Moved up one but it didn't change anything, so we're in the drive root.
					throw new Exception("Could not find directory " + toFind);
				}
				path = newDir;
			}

			return Path.Combine(path, toFind);
		}
	}
}
