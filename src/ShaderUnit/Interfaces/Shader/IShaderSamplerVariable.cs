﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderUnit.Interfaces.Shader
{
	public interface IShaderSamplerVariable : IShaderVariable
	{
		void Set(SamplerState state);
	}
}
