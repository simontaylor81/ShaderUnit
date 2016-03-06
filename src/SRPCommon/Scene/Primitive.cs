using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRPCommon.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace SRPCommon.Scene
{
	public enum PrimitiveType
	{
		MeshInstance,
		Sphere,
		Cube,
		Plane,
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public abstract class Primitive
	{
		[JsonProperty]
		[JsonConverter(typeof(StringEnumConverter))]
		public abstract PrimitiveType Type { get; }

		[JsonProperty]
		public Vector3 Position { get; set; }

		[JsonProperty]

		public Vector3 Scale { get; set; }

		[JsonProperty]
		public Vector3 Rotation { get; set; }

		public Material Material { get; private set; }

		[JsonProperty("material")]
		[SuppressMessage("Language", "CSE0002:Use getter-only auto properties", Justification = "Needed for serialisation")]
		private string MaterialName { get; set; }

		public virtual bool IsValid => true;

		protected Primitive()
		{
			Scale = new Vector3(1.0f, 1.0f, 1.0f);
		}

		public Matrix4x4 LocalToWorld
			=> Matrix4x4.CreateScale(Scale)
				* Matrix4x4.CreateFromYawPitchRoll(ToRadians(Rotation.Y), ToRadians(Rotation.X), ToRadians(Rotation.Z))
				* Matrix4x4.CreateTranslation(Position);

		private float ToRadians(float degrees) => degrees * ((float)Math.PI / 180.0f);

		internal virtual void PostLoad(Scene scene)
		{
			if (MaterialName != null)
			{
				// Look up mesh in the scene's collection.
				Material mat;
				if (scene.Materials.TryGetValue(MaterialName, out mat))
				{
					Material = mat;
				}
				else
				{
					OutputLogger.Instance.LogLine(LogCategory.Log, "Material not found: " + MaterialName);
				}
			}
		}
	}
}
