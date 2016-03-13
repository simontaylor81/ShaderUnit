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

namespace ShaderUnit.Rendering.Resources
{
	public class Texture : ID3DShaderResource, ITexture2D, IDisposable
	{
		public int Width { get; }
		public int Height { get; }

		public Texture2D Texture2D { get; }
		public ShaderResourceView SRV { get; }

		public UnorderedAccessView UAV { get { throw new NotImplementedException("TODO: Texture UAVs"); } }

		// Simple constructor taking a texture and shader resource view.
		public Texture(Device device, IScratchImage image, bool generateMips)
		{
			if (generateMips)
			{
				image.GenerateMipMaps();
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
		public static Texture LoadFromFile(Device device, string filename, bool generateMips)
		{
			try
			{
				// Load the texture itself using DirectXTex.
				var image = LoadImage(filename);

				return new Texture(device, image, generateMips);
			}
			catch (Exception ex)
			{
				throw new ShaderUnitException(string.Format("Failed to load texture file {0} Error code: 0x{1:x8}", filename, ex.HResult), ex);
			}
		}

		// Create with given contents.
		public static Texture Create<T>(Device device, int width, int height, Format format, IEnumerable<T> contents, bool generateMips) where T : struct
		{
			if (format.Size() != MarshalUtil.SizeOf<T>())
			{
				throw new ShaderUnitException($"Data of type {typeof(T).ToString()} is not suitable for texture format {format.ToString()}.");
			}
			var stream = contents.Take(width * height).ToDataStream();

			var image = DirectXTex.Create2D(stream.DataPointer, format.Size() * width, width, height, (uint)format.ToDXGI());
			return new Texture(device, image, generateMips);
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
