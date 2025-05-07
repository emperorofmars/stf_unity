using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
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

			if(JsonResource.ContainsKey("material_slots"))
				foreach(var slot in JsonResource["material_slots"])
					ret.material_slots.Add((STF_DataResource)Context.ImportResource(slot.Value<string>()));

			ret.lines_len = JsonResource.Value<ulong>("lines_len");
			if(JsonResource.ContainsKey("lines")) ret.lines = Context.ImportBuffer(JsonResource.Value<string>("lines"));

			if(JsonResource.ContainsKey("armature"))
				ret.armature = (STF_Armature)Context.ImportResource(JsonResource.Value<string>("armature"));

			if(JsonResource.ContainsKey("bones") && JsonResource.ContainsKey("weights"))
			{
				foreach(var bone in JsonResource["bones"])
					ret.bones.Add(bone.Value<string>());
				ret.bone_weight_width = JsonResource.Value<uint>("bone_weight_width");

				foreach(var JsonWeightChannel in JsonResource["weights"])
					ret.weights.Add(new STF_Mesh.WeightChannel {
						buffer = Context.ImportBuffer(JsonWeightChannel.Value<string>("buffer")),
						count = JsonWeightChannel.Value<ulong>("count"),
						indexed = JsonWeightChannel.Value<bool>("indexed")
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

			if(JsonResource.ContainsKey("sharp_face_indices"))
			{
				ret.sharp_face_indices_len = JsonResource.Value<ulong>("sharp_face_indices_len");
				ret.sharp_face_indices = Context.ImportBuffer(JsonResource.Value<string>("sharp_face_indices"));
			}

			if(JsonResource.ContainsKey("sharp_edges"))
			{
				ret.sharp_edges_len = JsonResource.Value<ulong>("sharp_edges_len");
				ret.sharp_edges = Context.ImportBuffer(JsonResource.Value<string>("sharp_edges"));
			}

			if(JsonResource.ContainsKey("sharp_vertices"))
			{
				ret.sharp_vertices_len = JsonResource.Value<ulong>("sharp_vertices_len");
				ret.sharp_vertices = Context.ImportBuffer(JsonResource.Value<string>("sharp_vertices"));
			}

			if(JsonResource.ContainsKey("vertex_groups"))
			{
				ret.vertex_weight_width = JsonResource.Value<uint>("vertex_weight_width");
				foreach(var JsonVertexgroup in JsonResource["vertex_groups"])
					ret.vertex_groups.Add(new STF_Mesh.VertexGroup {
						count = JsonVertexgroup.Value<ulong>("count"),
						indexed = JsonVertexgroup.Value<bool>("indexed"),
						name = JsonVertexgroup.Value<string>("name"),
						buffer = Context.ImportBuffer(JsonVertexgroup.Value<string>("buffer"))
					});
			}


			var unityMesh = ConvertToUnityMesh(ret);
			ret.ProcessedUnityMesh = unityMesh;


			return (ret, new(){ret, unityMesh});
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

		protected Mesh ConvertToUnityMesh(STF_Mesh STFMesh)
		{
			var ret = new Mesh();
			ret.name = "Processed " + STFMesh._STF_Name;

			// TODO use BinaryPrimitives whenever thats supported for floats and stuff

			var vertices = new Vector3[STFMesh.vertex_count];
			for(ulong i = 0; i < STFMesh.vertex_count; i++)
			{
				vertices[i].Set(
					-BitConverter.ToSingle(STFMesh.vertices.Data, (int)i * 4 * 3),
					BitConverter.ToSingle(STFMesh.vertices.Data, (int)i * 4 * 3 + 4),
					BitConverter.ToSingle(STFMesh.vertices.Data, (int)i * 4 * 3 + 8)
				);
			}

			var splits = new uint[STFMesh.split_count];
			for(int i = 0; i < (int)STFMesh.split_count; i++)
			{
				splits[i] = BitConverter.ToUInt32(STFMesh.splits.Data, i * 4);
			}

			var normals = new Vector3[STFMesh.split_count];
			for(ulong i = 0; i < STFMesh.split_count; i++)
			{
				normals[i].Set(
					-BitConverter.ToSingle(STFMesh.split_normals.Data, (int)i * 4 * 3),
					BitConverter.ToSingle(STFMesh.split_normals.Data, (int)i * 4 * 3 + 4),
					BitConverter.ToSingle(STFMesh.split_normals.Data, (int)i * 4 * 3 + 8)
				);
			}

			var tangents = new Vector3[STFMesh.split_count];
			for(int i = 0; i < (int)STFMesh.split_count; i++)
			{
				tangents[i].Set(
					-BitConverter.ToSingle(STFMesh.split_tangents.Data, i * 4 * 3),
					BitConverter.ToSingle(STFMesh.split_tangents.Data, i * 4 * 3 + 4),
					BitConverter.ToSingle(STFMesh.split_tangents.Data, i * 4 * 3 + 8)
				);
			}

			var uvs = new List<Vector2[]>();
			foreach(var uvBuffer in STFMesh.uvs)
			{
				var uv = new Vector2[STFMesh.split_count];
				for(int i = 0; i < (int)STFMesh.split_count; i++)
				{
					uv[i].Set(
						BitConverter.ToSingle(uvBuffer.uv.Data, i * 4),
						BitConverter.ToSingle(uvBuffer.uv.Data, i * 4 + 4)
					);
				}
				uvs.Add(uv);
			}

			bool compareUVs(int a, int b)
			{
				foreach(var uv in uvs)
				{
					if((uv[a] - uv[b]).magnitude > 0.0001) return false;
				}
				return true;
			}

			// TODO colors


			var woo = 0;

			var verts_to_split = new Dictionary<int, List<int>>();
			var deduped_splits = new List<int>();
			var split_to_deduped = new Dictionary<int, int>();
			var split_to_deduped_split = new Dictionary<int, int>();
			for(int splitIndex = 0; splitIndex < (int)STFMesh.split_count; splitIndex++)
			{
				var vertexIndex = (int)splits[splitIndex];
				if(!verts_to_split.ContainsKey(vertexIndex))
				{
					verts_to_split.Add(vertexIndex, new List<int> {splitIndex});
					deduped_splits.Add(vertexIndex);
					split_to_deduped.Add(splitIndex, deduped_splits.Count - 1);
					split_to_deduped_split.Add(splitIndex, splitIndex);
				}
				else
				{
					var success = false;
					for(int candidateIndex = 0; candidateIndex < verts_to_split[vertexIndex].Count; candidateIndex++)
					{
						var splitCandidate = verts_to_split[vertexIndex][candidateIndex];
						if(
							(normals[splitIndex] - normals[splitCandidate]).magnitude < 0.0001
							&& (tangents[splitIndex] - tangents[splitCandidate]).magnitude < 0.0001
							&& compareUVs(splitIndex, splitCandidate)
							// TODO colors
						)
						{
							split_to_deduped_split.Add(splitIndex, splitCandidate);
							success = true;

							woo++;
							break;
						}
					}
					if(!success)
					{
						verts_to_split[vertexIndex].Add(splitIndex);
						deduped_splits.Add(vertexIndex);
						split_to_deduped.Add(splitIndex, deduped_splits.Count - 1);
						split_to_deduped_split.Add(splitIndex, splitIndex);
					}
				}
			}

			Debug.Log($"Woo: {woo}");


			var unity_vertices = new List<Vector3>();
			var unity_normals = new List<Vector3>();
			foreach(var split in deduped_splits)
			{
				unity_vertices.Add(vertices[split]);
				unity_normals.Add(normals[split]);
			}

			var tris = new int[STFMesh.tris_count * 3];
			for(int i = 0; i < (int)STFMesh.tris_count * 3; i++)
			{
				tris[i] = (int)BitConverter.ToUInt32(STFMesh.tris.Data, i * 4);
			}

			var faceLengths = new uint[STFMesh.face_count];
			var faceMaterialIndices = new uint[STFMesh.face_count];
			for(int i = 0; i < (int)STFMesh.face_count; i++)
			{
				faceLengths[i] = BitConverter.ToUInt32(STFMesh.faces.Data, i * 4);
				faceMaterialIndices[i] = BitConverter.ToUInt32(STFMesh.material_indices.Data, i * 4);
			}

			var subMeshIndices = new List<List<int>>();
			var trisIndex = 0;
			for(int faceIndex = 0; faceIndex < (int)STFMesh.face_count; faceIndex++)
			{
				var matIndex = (int)faceMaterialIndices[faceIndex];
				for(uint faceLen = 0; faceLen < faceLengths[faceIndex]; faceLen++)
				{
					while(subMeshIndices.Count <= matIndex)
						subMeshIndices.Add(new List<int>());
					/*subMeshIndices[matIndex].Add(tris[trisIndex * 3]);
					subMeshIndices[matIndex].Add(tris[trisIndex * 3 + 1]);
					subMeshIndices[matIndex].Add(tris[trisIndex * 3 + 2]);*/

					subMeshIndices[matIndex].Add(split_to_deduped[split_to_deduped_split[tris[trisIndex * 3]]]);
					subMeshIndices[matIndex].Add(split_to_deduped[split_to_deduped_split[tris[trisIndex * 3 + 1]]]);
					subMeshIndices[matIndex].Add(split_to_deduped[split_to_deduped_split[tris[trisIndex * 3 + 2]]]);

					trisIndex++;
				}
			}


			ret.SetVertices(unity_vertices);
			ret.SetNormals(unity_normals);

			ret.subMeshCount = subMeshIndices.Count;
			for(int subMeshIdx = 0; subMeshIdx < subMeshIndices.Count; subMeshIdx++)
			{
				ret.SetIndices(subMeshIndices[subMeshIdx], MeshTopology.Triangles, subMeshIdx);
			}

			ret.UploadMeshData(false);
			ret.RecalculateBounds();

			return ret;
		}
	}
}
