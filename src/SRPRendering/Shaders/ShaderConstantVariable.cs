using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX.D3DCompiler;
using SharpDX;
using SRPCommon.Util;
using SRPScripting;
using SRPScripting.Shader;

namespace SRPRendering.Shaders
{
	// Need out own shader variable type descriptor because the SharpDX one just references a D3D
	// object that needs to be cleaned up after compilation is complete.
	struct ShaderVariableTypeDesc : IEquatable<ShaderVariableTypeDesc>
	{
		public ShaderVariableClass Class;
		public ShaderVariableType Type;
		public int Columns;
		public int Rows;

		public ShaderVariableTypeDesc(ShaderReflectionType type)
		{
			Class = type.Description.Class;
			Type = type.Description.Type;
			Columns = type.Description.ColumnCount;
			Rows = type.Description.RowCount;
		}

		public bool Equals(ShaderVariableTypeDesc other)
			=> Class == other.Class && Type == other.Type && Columns == other.Columns && Rows == other.Rows;
	}

	/// <summary>
	/// Concrete implementation of IShaderConstantVariable
	/// </summary>
	class ShaderConstantVariable : IShaderConstantVariable
	{
		// IShaderVariable interface.
		public string Name { get; }

		public void Set<T>(T value) where T : struct
		{
			Binding = new DirectShaderVariableBind<T>(this, value);
		}

		public void Bind(ShaderVariableBindSource bindSource)
		{
			Binding = new SimpleShaderVariableBind(this, bindSource);
		}

		public void BindToMaterial(string materialParam)
		{
			Binding = new MaterialShaderVariableBind(this, materialParam);
		}

		public ShaderVariableTypeDesc VariableType { get; }

		private IShaderVariableBinding _binding;
		public IShaderVariableBinding Binding
		{
			get { return _binding; }
			set
			{
				if (_binding != null)
				{
					throw new ShaderUnitException("Attempting to bind already bound shader variable: " + Name);
				}
				_binding = value;
			}
		}

		// For debugger prettiness.
		public override string ToString() => Name;

		// Get the current value of the variable.
		public T GetValue<T>() where T : struct
		{
			if (Marshal.SizeOf(typeof(T)) != data.Length)
				throw new ArgumentException("Given size does not match shader variable size.");

			data.Position = 0;
			return data.Read<T>();
		}

		// Set the value of the variable.
		public void SetValue<T>(T value) where T : struct
		{
			if (Marshal.SizeOf(typeof(T)) < data.Length)
				throw new ArgumentException(String.Format("Cannot set shader variable '{0}': given value is the wrong size.", Name));

			data.Position = 0;
			data.Write(value);

			bDirty = true;
		}

		// Get the current value of an individual component of the array.
		public T GetComponent<T>(int index) where T : struct
		{
			int componentSize = Marshal.SizeOf(typeof(T));
			if (componentSize * (index + 1) > data.Length)
				throw new IndexOutOfRangeException();

			data.Position = index * componentSize;
			return data.Read<T>();
		}

		// Get the current value of an individual component of the array.
		public void SetComponent<T>(int index, T value) where T : struct
		{
			int componentSize = Marshal.SizeOf(typeof(T));
			if (componentSize * (index + 1) > data.Length)
				throw new IndexOutOfRangeException();

			data.Position = index * componentSize;
			data.Write(value);

			bDirty = true;
		}

		// Reset to initial state.
		public void SetDefault()
		{
			data.Position = 0;
			data.Write(initialValue, 0, initialValue.Length);
			bDirty = true;
		}

		// Constructors.
		public ShaderConstantVariable(ShaderReflectionVariable shaderVariable)
		{
			Name = shaderVariable.Description.Name;
			VariableType = new ShaderVariableTypeDesc(shaderVariable.GetVariableType());

			offset = shaderVariable.Description.StartOffset;
			data = new DataStream(shaderVariable.Description.Size, true, true);

			if (shaderVariable.Description.DefaultValue != (IntPtr)0)
			{
				data.WriteRange(shaderVariable.Description.DefaultValue, shaderVariable.Description.Size);
			}
			else
			{
				// No initial contents: zero fill.
				// Not sure if this is necessary, but better safe than sorry.
				for (int i = 0; i < shaderVariable.Description.Size; i++)
					data.WriteByte(0);
			}

			// Save initial state so we can reset to it.
			initialValue = new byte[shaderVariable.Description.Size];
			data.Position = 0;
			data.Read(initialValue, 0, shaderVariable.Description.Size);
		}

		// Write the value into the destination stream at the correct offset.
		public bool WriteToBuffer(DataStream dest)
		{
			if (bDirty)
			{
				dest.Position = offset;
				data.Position = 0;
				data.CopyTo(dest);

				bDirty = false;
				return true;
			}
			return false;
		}

		private int offset;
		private DataStream data;
		private bool bDirty = true;
		private byte[] initialValue;
	}
}
