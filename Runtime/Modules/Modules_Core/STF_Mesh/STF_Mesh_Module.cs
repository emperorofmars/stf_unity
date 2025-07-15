using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Mesh_Module : ISTF_Module
	{
		public string STF_Type => STF_Mesh.STF_TYPE;

		public string STF_Kind => "data";

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

			ret.vertices = Context.ImportBuffer(JsonResource.Value<string>("vertices"));
			if(JsonResource.ContainsKey("colors"))
				foreach(var color in JsonResource["colors"])
					ret.colors.Add(Context.ImportBuffer(color.Value<string>()));

			if(JsonResource.ContainsKey("splits")) ret.splits = Context.ImportBuffer(JsonResource.Value<string>("splits"));
			if(JsonResource.ContainsKey("split_normals")) ret.split_normals = Context.ImportBuffer(JsonResource.Value<string>("split_normals"));
			if(JsonResource.ContainsKey("uvs"))
				foreach(JObject uv in (JArray)JsonResource["uvs"])
					ret.uvs.Add(new () {name = uv.Value<string>("name"), uv = Context.ImportBuffer(uv.Value<string>("uv"))});
			if(JsonResource.ContainsKey("split_colors"))
				foreach(var color in JsonResource["split_colors"])
					ret.split_colors.Add(Context.ImportBuffer(color.Value<string>()));

			if(JsonResource.ContainsKey("tris")) ret.tris = Context.ImportBuffer(JsonResource.Value<string>("tris"));
			if(JsonResource.ContainsKey("faces")) ret.faces = Context.ImportBuffer(JsonResource.Value<string>("faces"));
			if(JsonResource.ContainsKey("material_indices")) ret.material_indices = Context.ImportBuffer(JsonResource.Value<string>("material_indices"));

			if(JsonResource.ContainsKey("material_slots"))
				foreach(var slot in JsonResource["material_slots"])
					ret.material_slots.Add((STF_DataResource)Context.ImportResource(slot.Value<string>(), "data"));

			if(JsonResource.ContainsKey("lines")) ret.lines = Context.ImportBuffer(JsonResource.Value<string>("lines"));

			if(JsonResource.ContainsKey("armature"))
				ret.armature = (STF_Armature)Context.ImportResource(JsonResource.Value<string>("armature"), "data");

			if(JsonResource.ContainsKey("bones") && JsonResource.ContainsKey("weights"))
			{
				foreach(var bone in JsonResource["bones"])
					ret.bones.Add(bone.Value<string>());

				ret.weight_lens = Context.ImportBuffer(JsonResource.Value<string>("weight_lens"));
				ret.weights = Context.ImportBuffer(JsonResource.Value<string>("weights"));
			}

			if(JsonResource.ContainsKey("blendshapes"))
			{
				foreach(var jsonBlendshape in JsonResource["blendshapes"])
				{
					var blendshape = new STF_Mesh.Blendshape {
						default_value = jsonBlendshape.Value<float>("default_value"),
						limit_lower = jsonBlendshape.Value<float>("limit_lower"),
						limit_upper = jsonBlendshape.Value<float>("limit_upper"),
						name = jsonBlendshape.Value<string>("name"),
						position_offsets = Context.ImportBuffer(jsonBlendshape.Value<string>("position_offsets")),
						normals = Context.ImportBuffer(jsonBlendshape.Value<string>("normals")),
					};
					if(jsonBlendshape.Value<string>("indices") is var indicesBufferId && !string.IsNullOrEmpty(indicesBufferId))
						blendshape.indices = Context.ImportBuffer(indicesBufferId);
					ret.blendshapes.Add(blendshape);
				}
			}

			if(JsonResource.ContainsKey("sharp_face_indices"))
				ret.sharp_face_indices = Context.ImportBuffer(JsonResource.Value<string>("sharp_face_indices"));

			if(JsonResource.ContainsKey("sharp_edges"))
				ret.sharp_edges = Context.ImportBuffer(JsonResource.Value<string>("sharp_edges"));

			if(JsonResource.ContainsKey("sharp_vertices"))
				ret.sharp_vertices = Context.ImportBuffer(JsonResource.Value<string>("sharp_vertices"));

			if (JsonResource.ContainsKey("vertex_groups"))
			{
				foreach (var JsonVertexgroup in JsonResource["vertex_groups"])
				{
					var vertexGroup = new STF_Mesh.VertexGroup {
						name = JsonVertexgroup.Value<string>("name"),
						weights = Context.ImportBuffer(JsonVertexgroup.Value<string>("weights")),
					};
					if (JsonVertexgroup.Value<string>("indices") is var indicesBufferId && !string.IsNullOrWhiteSpace(indicesBufferId))
						vertexGroup.indices = Context.ImportBuffer(indicesBufferId);
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
