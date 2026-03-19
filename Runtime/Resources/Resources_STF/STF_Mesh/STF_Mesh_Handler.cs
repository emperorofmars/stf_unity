using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.resources
{
	public class STF_Mesh_Handler : ISTF_Handler
	{
		public string STF_Type => STF_Mesh.STF_TYPE;
		public string STF_Category => "data";
		public int Priority => 0;
		public List<string> LikeTypes => new(){"mesh"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STF_Mesh)};
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<STF_Mesh>();
			ret.SetFromJson(JsonResource, STF_Id, "STF Mesh");

			ret.float_width = JsonResource.Value<int>("float_width");
			ret.indices_width = JsonResource.Value<int>("indices_width");
			ret.material_indices_width = JsonResource.Value<int>("material_indices_width");
			ret.bone_indices_width = JsonResource.Value<int>("bone_indices_width");
			ret.weight_lens_width = JsonResource.Value<int>("weight_lens_width");

			ret.vertices = Context.ImportBuffer(JsonResource, JsonResource["vertices"]);

			if(JsonResource.ContainsKey("splits")) ret.splits = Context.ImportBuffer(JsonResource, JsonResource["splits"]);
			if(JsonResource.ContainsKey("face_corners")) ret.face_corners = Context.ImportBuffer(JsonResource, JsonResource["face_corners"]);
			if(JsonResource.ContainsKey("split_normals")) ret.split_normals = Context.ImportBuffer(JsonResource, JsonResource["split_normals"]);
			if(JsonResource.ContainsKey("uvs"))
				foreach(JObject uv in (JArray)JsonResource["uvs"])
					ret.uvs.Add(new () {name = uv.Value<string>("name"), uv = Context.ImportBuffer(JsonResource, uv["uv"])});
			if(JsonResource.ContainsKey("split_colors") && JsonResource["split_colors"].Type == JTokenType.String) ret.split_colors = Context.ImportBuffer(JsonResource, JsonResource["split_colors"]);

			if (JsonResource.ContainsKey("tris")) ret.tris = Context.ImportBuffer(JsonResource, JsonResource["tris"]);
			if(JsonResource.ContainsKey("faces")) ret.faces = Context.ImportBuffer(JsonResource, JsonResource["faces"]);
			if(JsonResource.ContainsKey("material_indices")) ret.material_indices = Context.ImportBuffer(JsonResource, JsonResource["material_indices"]);

			if(JsonResource.ContainsKey("material_slots"))
				foreach(var slot in JsonResource["material_slots"])
					ret.material_slots.Add((STF_DataResource)Context.ImportResource(JsonResource, slot, "data"));

			if(JsonResource.ContainsKey("lines")) ret.lines = Context.ImportBuffer(JsonResource, JsonResource["lines"]);

			if(JsonResource.ContainsKey("armature"))
				ret.armature = (STF_Armature)Context.ImportResource(JsonResource, JsonResource["armature"], "data");

			if(JsonResource.ContainsKey("bones") && JsonResource.ContainsKey("weights"))
			{
				foreach(var bone in JsonResource["bones"])
					ret.bones.Add(bone.Value<string>());

				ret.weight_lens = Context.ImportBuffer(JsonResource, JsonResource["weight_lens"]);
				ret.bone_indices = Context.ImportBuffer(JsonResource, JsonResource["bone_indices"]);
				ret.weights = Context.ImportBuffer(JsonResource, JsonResource["weights"]);
			}

			if(JsonResource.ContainsKey("blendshapes"))
			{
				foreach(JObject jsonBlendshape in JsonResource["blendshapes"].Cast<JObject>())
				{
					var blendshape = new STF_Mesh.Blendshape {
						default_value = jsonBlendshape.Value<float>("default_value"),
						limit_lower = jsonBlendshape.Value<float>("limit_lower"),
						limit_upper = jsonBlendshape.Value<float>("limit_upper"),
						name = jsonBlendshape.Value<string>("name"),
						position_offsets = Context.ImportBuffer(JsonResource, jsonBlendshape["position_offsets"]),
					};
					if (jsonBlendshape.ContainsKey("indices"))
						blendshape.indices = Context.ImportBuffer(JsonResource, jsonBlendshape["indices"]);
					if (jsonBlendshape.ContainsKey("split_indices"))
						blendshape.split_indices = Context.ImportBuffer(JsonResource, jsonBlendshape["split_indices"]);
					if (jsonBlendshape.ContainsKey("split_normals"))
						blendshape.split_normals = Context.ImportBuffer(JsonResource, jsonBlendshape["split_normals"]);
					ret.blendshapes.Add(blendshape);
				}
			}

			if(JsonResource.ContainsKey("sharp_face_indices"))
				ret.sharp_face_indices = Context.ImportBuffer(JsonResource, JsonResource["sharp_face_indices"]);

			if(JsonResource.ContainsKey("sharp_edges"))
				ret.sharp_edges = Context.ImportBuffer(JsonResource, JsonResource["sharp_edges"]);

			if(JsonResource.ContainsKey("sharp_vertices"))
				ret.sharp_vertices = Context.ImportBuffer(JsonResource, JsonResource["sharp_vertices"]);

			if (JsonResource.ContainsKey("vertex_groups"))
			{
				foreach (JObject JsonVertexgroup in JsonResource["vertex_groups"].Cast<JObject>())
				{
					var vertexGroup = new STF_Mesh.VertexGroup {
						name = JsonVertexgroup.Value<string>("name"),
						weights = Context.ImportBuffer(JsonResource, JsonVertexgroup["weights"]),
					};
					if (JsonVertexgroup.ContainsKey("indices"))
						vertexGroup.indices = Context.ImportBuffer(JsonResource, JsonVertexgroup["indices"]);
					ret.vertex_groups.Add(vertexGroup);
				}
			}

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
