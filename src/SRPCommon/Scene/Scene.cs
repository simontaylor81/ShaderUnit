using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SRPCommon.Util;

namespace SRPCommon.Scene
{
	// Representation of the scene that is to be rendered.
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public partial class Scene
	{
		public string Filename => _filename;
		public IEnumerable<Primitive> Primitives => _primitives;
		public IDictionary<string, SceneMesh> Meshes => _meshes;
		public IDictionary<string, Material> Materials => _materials;

		// Array of lights. Purely for access by script, so just dynamic objects.
		public IEnumerable<dynamic> Lights => _lights;

		private string _filename;

		[JsonProperty("primitives")]
		private List<Primitive> _primitives = new List<Primitive>();

		[JsonProperty("meshes")]
		private Dictionary<string, SceneMesh> _meshes = new Dictionary<string, SceneMesh>();

		[JsonProperty("materials")]
		private Dictionary<string, Material> _materials = new Dictionary<string, Material>();

		[JsonProperty("lights", ItemConverterType = typeof(JsonDynamicObjectConverter))]
		private List<dynamic> _lights = new List<dynamic>();

		public void AddPrimitive(Primitive primitive)
		{
			_primitives.Add(primitive);
		}

		public void RemovePrimitive(Primitive primitive)
		{
			_primitives.Remove(primitive);
		}
	}
}
