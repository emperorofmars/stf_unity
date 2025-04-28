using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Mesh : STF_DataResource
	{
		public const string STF_TYPE = "stf.mesh";
		public override string STF_Type => STF_TYPE;

		public Mesh ProcessedUnityMesh;

		[System.Serializable]
		public class NamedBuffer
		{
			public string name;
			public STF_Buffer uv;
		}

		[System.Serializable]
		public class WeightChannel
		{
			public bool indexed;
			public ulong count;
			public STF_Buffer buffer;
		}

		[System.Serializable]
		public class VertexGroup
		{
			public string name;
			public bool indexed;
			public ulong count;
			public STF_Buffer buffer;
		}

		[System.Serializable]
		public class Blendshape
		{
			public string name;
			public bool indexed;
			public ulong count;
			public float default_value;
			public float limit_upper;
			public float limit_lower;
			public STF_Buffer indices;
			public STF_Buffer position_offsets;
			public STF_Buffer normal_offsets;
		}

		// TODO material slots

		[Space]
		[Header("Vertices")]
		public ulong vertex_count;
		public uint vertex_width = 4;
		public uint vertex_indices_width = 4;
		public STF_Buffer vertices;
		public uint vertex_color_width = 4;
		public List<STF_Buffer> colors = new();

		[Space]
		[Header("Splits")]
		public ulong split_count;
		public uint split_indices_width = 4;
		public uint split_normal_width = 4;
		public uint split_tangent_width = 4;
		public uint split_color_width = 4;
		public uint split_uv_width = 4;
		public STF_Buffer splits;
		public STF_Buffer split_normals;
		public STF_Buffer split_tangents;
		public List<NamedBuffer> uvs = new();
		public List<STF_Buffer> split_colors = new();

		[Space]
		[Header("Topology")]
		public ulong tris_count;
		public ulong face_count;
		public uint face_indices_width = 4;
		public STF_Buffer tris;
		public STF_Buffer faces;
		public ulong lines_len;
		public STF_Buffer lines;
		public uint material_indices_width = 4;
		public STF_Buffer material_indices;
		public List<string> material_slots = new();

		[Space]
		[Header("Rigging")]
		public STF_Armature armature;
		public List<string> bones = new();
		public uint bone_weight_width = 4;
		public List<WeightChannel> weights = new();

		[Space]
		[Header("Blendshapes")]
		public uint blendshape_pos_width = 4;
		public uint blendshape_normal_width = 4;
		public uint blendshape_tangent_width = 4;
		public List<Blendshape> blendshapes = new();

		[Space]
		[Header("Additional Mesh Properies")]
		public ulong sharp_face_indices_len;
		public STF_Buffer sharp_face_indices;
		public ulong sharp_edges_len;
		public STF_Buffer sharp_edges;

		[Space]
		[Header("Vertex Groups")]
		public uint vertex_weight_width = 4;
		public List<VertexGroup> vertex_groups = new();
	}

	public class STF_Mesh_Module : ISTF_Module
	{
		public string STF_Type => STF_Mesh.STF_TYPE;

		public string STF_Kind => "data";

		public int Priority => 0;

		public List<string> LikeTypes => new(){"mesh"};

		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STF_Mesh)};

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public List<STF_ComponentResource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<STF_Mesh>();
			ret.SetFromJson(JsonResource, STF_Id, "STF Mesh");


			ret.vertex_count = JsonResource.Value<ulong>("vertex_count");
			ret.vertex_width = JsonResource.Value<uint>("vertex_width");
			ret.vertex_indices_width = JsonResource.Value<uint>("vertex_indices_width");
			ret.vertices = Context.ImportBuffer(JsonResource.Value<string>("vertices"));
			if(JsonResource.ContainsKey("colors")) foreach(var color in JsonResource["colors"])
				ret.colors.Add(Context.ImportBuffer(color.Value<string>()));

			ret.split_count = JsonResource.Value<ulong>("split_count");
			ret.split_indices_width = JsonResource.Value<uint>("split_indices_width");
			ret.split_normal_width = JsonResource.Value<uint>("split_normal_width");
			ret.split_tangent_width = JsonResource.Value<uint>("split_tangent_width");
			ret.split_color_width = JsonResource.Value<uint>("split_color_width");
			ret.split_uv_width = JsonResource.Value<uint>("split_uv_width");
			if(JsonResource.ContainsKey("splits")) ret.splits = Context.ImportBuffer(JsonResource.Value<string>("splits"));
			if(JsonResource.ContainsKey("split_normals")) ret.split_normals = Context.ImportBuffer(JsonResource.Value<string>("split_normals"));
			if(JsonResource.ContainsKey("split_tangents")) ret.split_tangents = Context.ImportBuffer(JsonResource.Value<string>("split_tangents"));
			if(JsonResource.ContainsKey("uvs")) foreach(JObject uv in (JArray)JsonResource["uvs"])
				ret.uvs.Add(new () {name = uv.Value<string>("name"), uv = Context.ImportBuffer(uv.Value<string>("uv"))});
			if(JsonResource.ContainsKey("split_colors")) foreach(var color in JsonResource["split_colors"])
				ret.split_colors.Add(Context.ImportBuffer(color.Value<string>()));

			ret.tris_count = JsonResource.Value<ulong>("tris_count");
			ret.face_count = JsonResource.Value<ulong>("face_count");
			ret.face_indices_width = JsonResource.Value<uint>("face_indices_width");
			if(JsonResource.ContainsKey("tris")) ret.tris = Context.ImportBuffer(JsonResource.Value<string>("tris"));
			if(JsonResource.ContainsKey("faces")) ret.faces = Context.ImportBuffer(JsonResource.Value<string>("faces"));
			ret.material_indices_width = JsonResource.Value<uint>("material_indices_width");
			if(JsonResource.ContainsKey("material_indices")) ret.material_indices = Context.ImportBuffer(JsonResource.Value<string>("material_indices"));

			ret.lines_len = JsonResource.Value<ulong>("lines_len");
			if(JsonResource.ContainsKey("lines")) ret.lines = Context.ImportBuffer(JsonResource.Value<string>("lines"));

			ret.sharp_edges_len = JsonResource.Value<ulong>("sharp_edges_len");
			if(JsonResource.ContainsKey("sharp_edges")) ret.sharp_edges = Context.ImportBuffer(JsonResource.Value<string>("sharp_edges"));

			if(JsonResource.ContainsKey("armature"))
			{
				ret.armature = (STF_Armature)Context.ImportResource(JsonResource.Value<string>("armature"));

				foreach(var bone in JsonResource["bones"])
					ret.bones.Add(bone.Value<string>());
				ret.bone_weight_width = JsonResource.Value<uint>("bone_weight_width");

				foreach(var weightChannel in JsonResource["weights"])
					ret.weights.Add(new STF_Mesh.WeightChannel {
						buffer = Context.ImportBuffer(weightChannel.Value<string>("buffer")),
						count = weightChannel.Value<ulong>("count"),
						indexed = weightChannel.Value<bool>("indexed")
					});
			}

			if(JsonResource.ContainsKey("blendshapes"))
			{
				ret.blendshape_pos_width = JsonResource.Value<uint>("blendshape_pos_width");
				ret.blendshape_normal_width = JsonResource.Value<uint>("blendshape_normal_width");
				ret.blendshape_tangent_width = JsonResource.Value<uint>("blendshape_tangent_width");

				foreach(var jsonBlendshape in JsonResource["blendshapes"])
				{
					var blendshape = new STF_Mesh.Blendshape {
						count = jsonBlendshape.Value<ulong>("count"),
						indexed = jsonBlendshape.Value<bool>("indexed"),
						default_value = jsonBlendshape.Value<float>("default_value"),
						limit_lower = jsonBlendshape.Value<float>("limit_lower"),
						limit_upper = jsonBlendshape.Value<float>("limit_upper"),
						name = jsonBlendshape.Value<string>("name"),
						position_offsets = Context.ImportBuffer(jsonBlendshape.Value<string>("position_offsets")),
						normal_offsets = Context.ImportBuffer(jsonBlendshape.Value<string>("normal_offsets")),
					};
					if(blendshape.indexed) blendshape.indices = Context.ImportBuffer(jsonBlendshape.Value<string>("indices"));
					ret.blendshapes.Add(blendshape);
				}
			}

			// TODO additional mesh properties & vertex groups


			return (ret, new(){ret});
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var MeshObject = ApplicationObject as STF_Mesh;
			var ret = new JObject {
				{"type", STF_Type},
				{"name", MeshObject.STF_Name},
			};

			return (ret, MeshObject.STF_Id);
		}
	}
}
