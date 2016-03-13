using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShaderUnit.Util;

namespace ShaderUnit.TestRenderer
{
	// Implementation of IWorkspace that finds test files.
	class TestWorkspace : IWorkspace
	{
		private readonly string _baseDir;

		public TestWorkspace(string baseDir)
		{
			_baseDir = baseDir;
		}

		public string GetAbsolutePath(string path)
		{
			if (Path.IsPathRooted(path))
			{
				return path;
			}
			return Path.Combine(_baseDir, path);
		}
	}
}
