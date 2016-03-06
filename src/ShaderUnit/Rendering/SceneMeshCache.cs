using SharpDX.Direct3D11;
using ShaderUnit.Mesh;
using ShaderUnit.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShaderUnit.Rendering.Resources;

namespace ShaderUnit.Rendering
{
	// Super-simple cache to prevent recreating meshes over and over.
	class SceneMeshCache : IDisposable
	{
		private readonly Device _device;
		private readonly Dictionary<SceneMesh, RenderMesh> _sceneMeshes = new Dictionary<SceneMesh, RenderMesh>();
		private readonly Dictionary<Tuple<int, int>, RenderMesh> _sphereMeshes = new Dictionary<Tuple<int, int>, RenderMesh>();

		public SceneMeshCache(Device device)
		{
			_device = device;
		}

		public void Dispose()
		{
			foreach (var mesh in _sceneMeshes.Values.Concat(_sphereMeshes.Values))
			{
				mesh.Dispose();
			}
			_sceneMeshes.Clear();
			_sphereMeshes.Clear();
		}

		// Get a mesh for a scene mesh.
		public RenderMesh GetForSceneMesh(SceneMesh sceneMesh)
		{
			return _sceneMeshes.GetOrAdd(sceneMesh, () =>
			{
				using (var vertData = sceneMesh.Vertices.ToDataStream())
				using (var indexData = sceneMesh.Indices.ToDataStream())
				{
					return new RenderMesh(
						_device,
						vertData,
						SceneVertex.GetStride(),
						indexData,
						InputLayoutCache.SceneVertexInputElements);
				}
			});
		}

		// Get a mesh for a sphere.
		public RenderMesh GetForSphere(int slices, int stacks)
		{
			return _sphereMeshes.GetOrAdd(
				Tuple.Create(slices, stacks),
				() => BasicMesh.CreateSphere(_device, slices, stacks));
		}

		// Dispose and remove any meshes from the cache that aren't in the given list.
		public void ReleaseUnusedMeshes(IEnumerable<IDrawable> usedMeshes)
		{
			var unusedSceneMeshes = _sceneMeshes.Where(kvp => !usedMeshes.Contains(kvp.Value)).ToList();
			foreach (var kvp in unusedSceneMeshes)
			{
				_sceneMeshes.Remove(kvp.Key);
				kvp.Value.Dispose();
			}

			var unusedSphereMeshes = _sphereMeshes.Where(kvp => !usedMeshes.Contains(kvp.Value)).ToList();
			foreach (var kvp in unusedSphereMeshes)
			{
				_sphereMeshes.Remove(kvp.Key);
				kvp.Value.Dispose();
			}
		}
	}
}
