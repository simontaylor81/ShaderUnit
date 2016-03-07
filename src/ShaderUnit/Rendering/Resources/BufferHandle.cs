﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using ShaderUnit.Interfaces;

namespace ShaderUnit.Rendering.Resources
{
	internal class BufferHandle : IBuffer, ID3DShaderResource, IDisposable
	{
		// Actual concrete buffer, lazily initialised.
		private Buffer _buffer;

		// Function for creating the buffer when it is required.
		private readonly Func<Buffer> _createBuffer;

		public Buffer Buffer
		{
			get
			{
				if (_buffer == null)
				{
					_buffer = _createBuffer();
				}
				return _buffer;
			}
		}

		public ShaderResourceView SRV => Buffer.SRV;
		public UnorderedAccessView UAV => Buffer.UAV;

		// Use static members to create.
		private BufferHandle(Func<Buffer> createBuffer)
		{
			Trace.Assert(createBuffer != null);
			_createBuffer = createBuffer;
		}

		public static BufferHandle CreateStructured<T>(RenderDevice device, bool uav, IEnumerable<T> contents) where T : struct
		{
			return new BufferHandle(() => Buffer.CreateStructured(device.Device, uav, contents));
		}

		public IEnumerable<T> GetContents<T>() where T : struct
		{
			// Don't create a buffer just to get its contents!
			if (_buffer == null)
			{
				throw new ShaderUnitException("Attempting to read contents of a buffer that is never written to.");
			}

			return _buffer.GetContents<T>();
		}

		public void Dispose()
		{
			// If we have a buffer, dispose it.
			_buffer?.Dispose();
		}
	}
}