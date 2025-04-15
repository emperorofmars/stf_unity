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

			ret.vertex_count = (ulong)JsonResource.GetValue("vertex_count");
			ret.vertex_width = (uint)JsonResource.GetValue("vertex_width");
			ret.vertex_indices_width = (uint)JsonResource.GetValue("vertex_indices_width");
			ret.vertices = Context.ImportBuffer((string)JsonResource["vertices"]);
			if(JsonResource.ContainsKey("colors")) foreach(var color in JsonResource["colors"])
				ret.colors.Add(Context.ImportBuffer((string)color));

			ret.split_count = (ulong)JsonResource["split_count"];
			ret.split_indices_width = (uint)JsonResource["split_indices_width"];
			ret.split_normal_width = (uint)JsonResource["split_normal_width"];
			ret.split_tangent_width = (uint)JsonResource["split_tangent_width"];
			ret.split_color_width = (uint)JsonResource["split_color_width"];
			ret.split_uv_width = (uint)JsonResource["split_uv_width"];
			ret.splits = Context.ImportBuffer((string)JsonResource["splits"]);
			ret.split_normals = Context.ImportBuffer((string)JsonResource["split_normals"]);
			ret.split_tangents = Context.ImportBuffer((string)JsonResource["split_tangents"]);
			if(JsonResource.ContainsKey("uvs")) foreach(JObject uv in (JArray)JsonResource["uvs"])
				ret.uvs.Add(new () {name = (string)uv["name"], uv = Context.ImportBuffer((string)uv["uv"])});
			if(JsonResource.ContainsKey("split_colors")) foreach(var color in JsonResource["split_colors"])
				ret.split_colors.Add(Context.ImportBuffer((string)color));

			ret.tris_count = (ulong)JsonResource["tris_count"];
			ret.face_count = (ulong)JsonResource["face_count"];
			ret.face_indices_width = (uint)JsonResource["face_indices_width"];
			ret.tris = Context.ImportBuffer((string)JsonResource["tris"]);
			ret.faces = Context.ImportBuffer((string)JsonResource["faces"]);
			ret.material_indices_width = (uint)JsonResource["material_indices_width"];
			ret.material_indices = Context.ImportBuffer((string)JsonResource["material_indices"]);

			ret.lines_len = (ulong)JsonResource["lines_len"];
			ret.lines = Context.ImportBuffer((string)JsonResource["lines"]);

			ret.sharp_edges_len = (ulong)JsonResource["sharp_edges_len"];
			ret.sharp_edges = Context.ImportBuffer((string)JsonResource["sharp_edges"]);

			if(JsonResource.ContainsKey("armature"))
				ret.armature = (STF_Armature)Context.ImportResource((string)JsonResource["armature"]);

			foreach(var bone in JsonResource["bones"])
				ret.bones.Add((string)bone);
			ret.bone_weight_width = (uint)JsonResource["bone_weight_width"];

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
