using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using SharpDX;
using ShaderUnit.Interfaces;
using ShaderUnit.Util;
using DirectXTexNet;
using System.Runtime.InteropServices;

namespace ShaderUnit.Rendering.Resources
{
	public enum MipGenerationMode
	{
		None,		// Create texture without mipchain
		Full,		// Generate full mipchain
		CreateOnly,	// Create the mip chain, but don't put any data in it.
	}

	public class Texture : ID3DShaderResource, ITexture2D, IDisposable
	{
		public int Width { get; }
		public int Height { get; }

		public Texture2D Texture2D { get; }
		public ShaderResourceView SRV { get; }

		public UnorderedAccessView UAV { get { throw new NotImplementedException("TODO: Texture UAVs"); } }

		// Simple constructor taking a texture and shader resource view.
		public Texture(Device device, IScratchImage image, MipGenerationMode mipGenerationMode)
		{
			if (mipGenerationMode == MipGenerationMode.Full)
			{
				image.GenerateMipMaps();
			}
			else if (mipGenerationMode == MipGenerationMode.CreateOnly)
			{
				image.CreateEmptyMipChain();
			}

			Texture2D = new Texture2D(image.CreateTexture(device.NativePointer));

			// Create the SRV.
			SRV = new ShaderResourceView(device, Texture2D);

			var metadata = image.MetaData;
			Width = (int)metadata.width;
			Height = (int)metadata.height;
		}

		public void Dispose()
		{
			DisposableUtil.SafeDispose(SRV);
			DisposableUtil.SafeDispose(Texture2D);
		}

		// Create a texture from a file.
		// TODO: Not sure if this is the best strategy long term.
		// Probably want to separate import from render resource creation.
		public static Texture LoadFromFile(Device device, string filename, MipGenerationMode mipGenerationMode)
		{
			try
			{
				//var stopwatch = System.Diagnostics.Stopwatch.StartNew();

				// Load the texture itself using DirectXTex.
				var image = LoadImage(filename);

				return new Texture(device, image, mipGenerationMode);
			}
			catch (Exception ex)
			{
				throw new ShaderUnitException(string.Format("Failed to load texture file {0} Error code: 0x{1:x8}", filename, ex.HResult), ex);
			}
		}

		// Create with given contents.
		public static Texture Create<T>(Device device, int width, int height, Format format, IEnumerable<T> contents, MipGenerationMode mipGenerationMode) where T : struct
		{
			if (format.Size() != Marshal.SizeOf<T>())
			{
				throw new ShaderUnitException($"Data of type {typeof(T).ToString()} is not suitable for texture format {format.ToString()}.");
			}
			var stream = contents.Take(width * height).ToDataStream();

			var image = DirectXTex.Create2D(stream.DataPointer, format.Size() * width, width, height, (uint)format.ToDXGI());
			return new Texture(device, image, mipGenerationMode);
		}

		private static IScratchImage LoadImage(string filename)
		{
			var ext = Path.GetExtension(filename).ToLowerInvariant();
			if (ext == ".dds")
			{
				// Load .dds files using DDS loader.
				return DirectXTex.LoadFromDDSFile(filename);
			}
			else if (ext == ".tga")
			{
				// Load .tga files using TGA loader.
				return DirectXTex.LoadFromTGAFile(filename);
			}
			else
			{
				// Attempt to load all other images using WIC.
				return DirectXTex.LoadFromWICFile(filename);
			}
		}
	}
}
