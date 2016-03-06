using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using SRPCommon.Util;
using SRPRendering.Resources;
using SRPRendering.Shaders;

namespace SRPRendering
{
	interface IGlobalResources : IDisposable
	{
		// Simple constant colour textures.
		Texture BlackTexture { get; }
		Texture WhiteTexture { get; }
		Texture DefaultNormalTexture { get; }

		// Texture to use to indicate error when non is found.
		Texture ErrorTexture { get; }

		IDrawable CubeMesh { get; }
		IDrawable PlaneMesh { get; }
		IDrawable SphereMesh { get; }
		IDrawable FullscreenQuad { get; }

		IBasicShaders BasicShaders { get; }

		// State object caches.
		IStateObjectCache<RasterizerState, RasterizerStateDescription> RastStateCache { get; }
		IStateObjectCache<DepthStencilState, DepthStencilStateDescription> DepthStencilStateCache { get; }
		IStateObjectCache<BlendState, BlendStateDescription> BlendStateCache { get; }
		IStateObjectCache<SamplerState, SamplerStateDescription> SamplerStateCache { get; }
		IInputLayoutCache InputLayoutCache { get; }
	}

	// Holder for various global D3D resources.
	internal class GlobalResources : IGlobalResources
	{
		// The resources themselves.
		public Texture BlackTexture { get; }
		public Texture WhiteTexture { get; }
		public Texture DefaultNormalTexture { get; }
		public Texture ErrorTexture { get; }

		public IDrawable CubeMesh { get; }
		public IDrawable PlaneMesh { get; }
		public IDrawable SphereMesh { get; }
		public IDrawable FullscreenQuad { get; }

		public IBasicShaders BasicShaders { get; }

		// State object caches.
		public IStateObjectCache<RasterizerState, RasterizerStateDescription> RastStateCache { get; }
		public IStateObjectCache<DepthStencilState, DepthStencilStateDescription> DepthStencilStateCache { get; }
		public IStateObjectCache<BlendState, BlendStateDescription> BlendStateCache { get; }
		public IStateObjectCache<SamplerState, SamplerStateDescription> SamplerStateCache { get; }
		public IInputLayoutCache InputLayoutCache { get; }

		private CompositeDisposable disposables = new CompositeDisposable();

		// Initialise the resources.
		public GlobalResources(Device device)
		{
			// Create constant pink error texture.
			BlackTexture = CreateConstantColourTexture(device, Color.Black);
			WhiteTexture = CreateConstantColourTexture(device, Color.White);
			DefaultNormalTexture = CreateConstantColourTexture(device, Color.FromArgb(128, 128, 255), sRGB: false);
			ErrorTexture = CreateConstantColourTexture(device, Color.Magenta);
			disposables.Add(BlackTexture);
			disposables.Add(WhiteTexture);
			disposables.Add(DefaultNormalTexture);
			disposables.Add(ErrorTexture);

			// Create simple utility meshes.
			var cubeMesh = BasicMesh.CreateCube(device);
			CubeMesh = cubeMesh;
			disposables.Add(cubeMesh);

			var planeMesh = BasicMesh.CreatePlane(device);
			PlaneMesh = planeMesh;
			disposables.Add(planeMesh);

			var sphereMesh = BasicMesh.CreateSphere(device, 12, 6);
			SphereMesh = sphereMesh;
			disposables.Add(sphereMesh);

			var fullscreenQuad = new FullscreenQuad(device);
			FullscreenQuad = fullscreenQuad;
			disposables.Add(fullscreenQuad);

			BasicShaders = new BasicShaders(device);
			disposables.Add(BasicShaders);

			// Create the state object caches.
			RastStateCache = StateObjectCache.Create((RasterizerStateDescription desc) => new RasterizerState(device, desc));
			DepthStencilStateCache = StateObjectCache.Create((DepthStencilStateDescription desc) => new DepthStencilState(device, desc));
			BlendStateCache = StateObjectCache.Create((BlendStateDescription desc) => new BlendState(device, desc));
			SamplerStateCache = StateObjectCache.Create((SamplerStateDescription desc) => new SamplerState(device, desc));

			InputLayoutCache = new InputLayoutCache();

			disposables.Add(RastStateCache);
			disposables.Add(DepthStencilStateCache);
			disposables.Add(BlendStateCache);
			disposables.Add(SamplerStateCache);
			disposables.Add(InputLayoutCache);
		}

		// Release all resources.
		public void Dispose()
		{
			disposables.Dispose();
		}

		// Create a texture with a solid colour.
		private Texture CreateConstantColourTexture(Device device, Color colour, bool sRGB = true)
		{
			// Convert BGRA colour to RGBA.
			var rgba = Color.FromArgb(colour.A, colour.B, colour.G, colour.R).ToArgb();

			// Make a 1x1 texture.
			var format = sRGB ? SRPScripting.Format.R8G8B8A8_UNorm_SRgb : SRPScripting.Format.R8G8B8A8_UNorm;
			return Texture.Create(device, 1, 1, format, new[] { rgba }, MipGenerationMode.None);
		}
	}
}
