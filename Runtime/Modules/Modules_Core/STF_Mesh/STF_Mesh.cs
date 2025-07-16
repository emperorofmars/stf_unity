using System.Collections.Generic;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Mesh : STF_DataResource
	{
		public const string STF_TYPE = "stf.mesh";
		public override string STF_Type => STF_TYPE;

		//public Mesh ProcessedUnityMesh;

		[System.Serializable]
		public class NamedBuffer
		{
			public string name;
			public STF_Buffer uv;
		}

		[System.Serializable]
		public class VertexGroup
		{
			public string name;
			public STF_Buffer indices;
			public STF_Buffer weights;
		}

		[System.Serializable]
		public class Blendshape
		{
			public string name;
			public float default_value;
			public float limit_upper;
			public float limit_lower;
			public STF_Buffer indices;
			public STF_Buffer position_offsets;
			public STF_Buffer split_indices;
			public STF_Buffer split_normals;
		}

		// TODO material slots

		[Header("Meta")]
		public int float_width = 4;
		public int indices_width = 4;


		[Space]
		[Header("Vertices")]
		public STF_Buffer vertices;
		public List<STF_Buffer> colors = new();

		[Space]
		[Header("Splits")]
		public STF_Buffer splits;
		public STF_Buffer split_normals;
		public List<NamedBuffer> uvs = new();
		public List<STF_Buffer> split_colors = new();

		[Space]
		[Header("Topology")]
		public STF_Buffer tris;
		public STF_Buffer faces;
		public STF_Buffer lines;
		public int material_indices_width = 1;
		public STF_Buffer material_indices;
		public List<STF_DataResource> material_slots = new();

		[Space]
		[Header("Rigging")]
		public STF_Armature armature;
		public List<string> bones = new();
		public int bone_indices_width = 1;
		public STF_Buffer weight_lens;
		public STF_Buffer bone_indices;
		public STF_Buffer weights;

		[Space]
		[Header("Blendshapes")]
		public List<Blendshape> blendshapes = new();

		[Space]
		[Header("Additional Mesh Properies")]
		public STF_Buffer sharp_face_indices;
		public STF_Buffer sharp_edges;
		public STF_Buffer sharp_vertices;

		[Space]
		[Header("Vertex Groups")]
		public List<VertexGroup> vertex_groups = new();
	}
}
