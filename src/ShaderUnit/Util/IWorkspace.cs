using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderUnit.Util
{
	// Interface for the "workspace", basically the top-level view model of the app.
	public interface IWorkspace
	{
		// Given an absolute or project-relative path, get an absolute path.
		string GetAbsolutePath(string path);
	}
}
