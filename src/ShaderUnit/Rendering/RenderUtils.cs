using ShaderUnit.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ShaderUnit.Rendering
{
	static class RenderUtils
	{
		public static string GetShaderFilename(string file)
		{
			var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			return Path.Combine(PathUtils.FindPathInTree(assemblyPath, "Shaders"), file);
		}
	}
}
