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
		public List<STF_DataResource> material_slots = new();

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
		public ulong sharp_vertices_len;
		public STF_Buffer sharp_vertices;

		[Space]
		[Header("Vertex Groups")]
		public uint vertex_weight_width = 4;
		public List<VertexGroup> vertex_groups = new();
	}
}
