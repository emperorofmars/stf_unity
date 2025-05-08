using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Unity.Collections;
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
			if(JsonResource.ContainsKey("split_tangents")) ret.split_tangents = Context.ImportBuffer(JsonResource.Value<string>("split_tangents"));
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
					ret.material_slots.Add((STF_DataResource)Context.ImportResource(slot.Value<string>()));

			if(JsonResource.ContainsKey("lines")) ret.lines = Context.ImportBuffer(JsonResource.Value<string>("lines"));

			if(JsonResource.ContainsKey("armature"))
				ret.armature = (STF_Armature)Context.ImportResource(JsonResource.Value<string>("armature"));

			if(JsonResource.ContainsKey("bones") && JsonResource.ContainsKey("weights"))
			{
				foreach(var bone in JsonResource["bones"])
					ret.bones.Add(bone.Value<string>());

				foreach(var JsonWeightChannel in JsonResource["weights"])
					ret.weights.Add(new STF_Mesh.WeightChannel {
						buffer = Context.ImportBuffer(JsonWeightChannel.Value<string>("buffer")),
						count = JsonWeightChannel.Value<ulong>("count"),
						indexed = JsonWeightChannel.Value<bool>("indexed")
					});
			}

			if(JsonResource.ContainsKey("blendshapes"))
			{
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
				ret.sharp_face_indices = Context.ImportBuffer(JsonResource.Value<string>("sharp_face_indices"));

			if(JsonResource.ContainsKey("sharp_edges"))
				ret.sharp_edges = Context.ImportBuffer(JsonResource.Value<string>("sharp_edges"));

			if(JsonResource.ContainsKey("sharp_vertices"))
				ret.sharp_vertices = Context.ImportBuffer(JsonResource.Value<string>("sharp_vertices"));

			if(JsonResource.ContainsKey("vertex_groups"))
			{
				foreach(var JsonVertexgroup in JsonResource["vertex_groups"])
					ret.vertex_groups.Add(new STF_Mesh.VertexGroup {
						count = JsonVertexgroup.Value<ulong>("count"),
						indexed = JsonVertexgroup.Value<bool>("indexed"),
						name = JsonVertexgroup.Value<string>("name"),
						buffer = Context.ImportBuffer(JsonVertexgroup.Value<string>("buffer"))
					});
			}


			var unityMesh = ConvertToUnityMesh(Context, ret);
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


		protected Mesh ConvertToUnityMesh(ImportContext Context, STF_Mesh STFMesh)
		{
			var ret = new Mesh { name = "Processed " + STFMesh._STF_Name };

			// TODO use BinaryPrimitives whenever thats supported for floats and stuff

			var vertex_count = STFMesh.vertices.BufferLength / (STFMesh.float_width * 3);
			var split_count = STFMesh.splits.BufferLength / STFMesh.indices_width;
			var tris_count = STFMesh.tris.BufferLength / (STFMesh.indices_width * 3);
			var face_count = STFMesh.faces.BufferLength / STFMesh.indices_width;


			float parseFloat(byte[] Buffer, int IndexBytes, int Width, int OffsetBytes = 0)
			{
				return Width switch
				{
					4 => BitConverter.ToSingle(Buffer, IndexBytes + OffsetBytes),
					8 => (float)BitConverter.ToDouble(Buffer, IndexBytes + OffsetBytes),
					_ => throw new NotImplementedException()
				};
			}

			int parseInt(byte[] Buffer, int IndexBytes, int Width, int OffsetBytes = 0)
			{
				return Width switch
				{
					1 => Buffer[IndexBytes + OffsetBytes],
					2 => BitConverter.ToUInt16(Buffer, IndexBytes + OffsetBytes),
					4 => (int)BitConverter.ToUInt32(Buffer, IndexBytes + OffsetBytes),
					8 => (int)BitConverter.ToUInt64(Buffer, IndexBytes + OffsetBytes),
					_ => throw new NotImplementedException()
				};
			}

			var vertices = new Vector3[vertex_count];
			for(int i = 0; i < vertex_count; i++)
			{
				vertices[i].Set(
					-parseFloat(STFMesh.vertices.Data, i * STFMesh.float_width * 3, STFMesh.float_width),
					parseFloat(STFMesh.vertices.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width),
					parseFloat(STFMesh.vertices.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width * 2)
				);
			}

			var splits = new int[split_count];
			for(int i = 0; i < split_count; i++)
				splits[i] = parseInt(STFMesh.splits.Data, i * STFMesh.indices_width, STFMesh.indices_width);

			var normals = new Vector3[split_count];
			for(int i = 0; i < split_count; i++)
			{
				normals[i].Set(
					-parseFloat(STFMesh.split_normals.Data, i * STFMesh.float_width * 3, STFMesh.float_width),
					parseFloat(STFMesh.split_normals.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width),
					parseFloat(STFMesh.split_normals.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width * 2)
				);
				normals[i].Normalize();
			}

			var tangents = new Vector4[split_count];
			for(int i = 0; i < split_count; i++)
			{
				tangents[i].Set(
					-parseFloat(STFMesh.split_tangents.Data, i * STFMesh.float_width * 3, STFMesh.float_width),
					parseFloat(STFMesh.split_tangents.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width),
					parseFloat(STFMesh.split_tangents.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width * 2),
					1
				);
				tangents[i].Normalize();
			}

			var uvs = new List<Vector2[]>();
			foreach(var uvBuffer in STFMesh.uvs)
			{
				var uv = new Vector2[split_count];
				for(int i = 0; i < split_count; i++)
				{
					uv[i].Set(
						parseFloat(uvBuffer.uv.Data, i * STFMesh.float_width * 2, STFMesh.float_width),
						1 - parseFloat(uvBuffer.uv.Data, i * STFMesh.float_width * 2, STFMesh.float_width, STFMesh.float_width)
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


			var verts_to_split = new Dictionary<int, List<int>>();
			var deduped_split_indices = new List<int>();
			var split_to_deduped_split_index = new List<int>();
			for(int splitIndex = 0; splitIndex < split_count; splitIndex++)
			{
				var vertexIndex = (int)splits[splitIndex];
				if(!verts_to_split.ContainsKey(vertexIndex))
				{
					verts_to_split.Add(vertexIndex, new List<int> {splitIndex});
					deduped_split_indices.Add(splitIndex);
					split_to_deduped_split_index.Add(deduped_split_indices.Count - 1);
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
							split_to_deduped_split_index.Add(split_to_deduped_split_index[splitCandidate]);
							success = true;
							break;
						}
					}
					if(!success)
					{
						verts_to_split[vertexIndex].Add(splitIndex);
						deduped_split_indices.Add(splitIndex);
						split_to_deduped_split_index.Add(deduped_split_indices.Count - 1);
					}
				}
			}


			var unity_vertices = new List<Vector3>();
			var unity_normals = new List<Vector3>();
			var unity_tangents = new List<Vector4>();
			var unity_uvs = new List<List<Vector2>>();
			for(int uvIndex = 0; uvIndex < uvs.Count; uvIndex++)
			{
				unity_uvs.Add(new());
			}
			for(int i = 0; i < deduped_split_indices.Count; i++)
			{
				unity_vertices.Add(vertices[splits[deduped_split_indices[i]]]);
				unity_normals.Add(normals[deduped_split_indices[i]]);
				unity_tangents.Add(tangents[deduped_split_indices[i]]);
				for(int uvIndex = 0; uvIndex < uvs.Count; uvIndex++)
				{
					unity_uvs[uvIndex].Add(uvs[uvIndex][deduped_split_indices[i]]);
				}
			}


			var tris = new Vector3Int[tris_count];
			for(int i = 0; i < tris_count; i++)
			{
				tris[i].Set(
					parseInt(STFMesh.tris.Data, i * STFMesh.indices_width * 3, STFMesh.indices_width, STFMesh.indices_width * 2),
					parseInt(STFMesh.tris.Data, i * STFMesh.indices_width * 3, STFMesh.indices_width, STFMesh.indices_width),
					parseInt(STFMesh.tris.Data, i * STFMesh.indices_width * 3, STFMesh.indices_width)
				);
			}

			var faceLengths = new int[face_count];
			var faceMaterialIndices = new int[face_count];
			for(int i = 0; i < face_count; i++)
			{
				faceLengths[i] = parseInt(STFMesh.faces.Data, i * STFMesh.indices_width, STFMesh.indices_width);
				faceMaterialIndices[i] = parseInt(STFMesh.material_indices.Data, i * STFMesh.material_indices_width, STFMesh.material_indices_width);
			}

			var subMeshIndices = new List<List<int>>();
			var trisIndex = 0;
			for(int faceIndex = 0; faceIndex < face_count; faceIndex++)
			{
				var matIndex = (int)faceMaterialIndices[faceIndex];
				for(uint faceLen = 0; faceLen < faceLengths[faceIndex]; faceLen++)
				{
					while(subMeshIndices.Count <= matIndex) subMeshIndices.Add(new List<int>());

					subMeshIndices[matIndex].Add(split_to_deduped_split_index[tris[trisIndex].x]);
					subMeshIndices[matIndex].Add(split_to_deduped_split_index[tris[trisIndex].y]);
					subMeshIndices[matIndex].Add(split_to_deduped_split_index[tris[trisIndex].z]);

					trisIndex++;
				}
			}


			ret.SetVertices(unity_vertices);
			ret.SetNormals(unity_normals);
			ret.SetTangents(unity_tangents);
			for(int uvIndex = 0; uvIndex < unity_uvs.Count; uvIndex++)
			{
				ret.SetUVs(uvIndex, unity_uvs[uvIndex]);
			}
			ret.SetVertices(unity_vertices);
			ret.SetNormals(unity_normals);
			ret.SetTangents(unity_tangents);
			for(int uvIndex = 0; uvIndex < unity_uvs.Count; uvIndex++)
			{
				ret.SetUVs(uvIndex, unity_uvs[uvIndex]);
			}


			ret.indexFormat = unity_vertices.Count > ushort.MaxValue ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
			ret.subMeshCount = subMeshIndices.Count;
			for(int subMeshIdx = 0; subMeshIdx < subMeshIndices.Count; subMeshIdx++)
			{
				ret.SetIndices(subMeshIndices[subMeshIdx], MeshTopology.Triangles, subMeshIdx);
			}


			if(STFMesh.armature != null && STFMesh.bones != null && STFMesh.weights != null)
			{
				const int MAX_BONES_PER_VERTEX = 8;

				var weights = new List<BoneWeight1>[vertex_count];
				for(int i = 0; i < vertex_count; i++) weights[i] = new List<BoneWeight1>();

				for(int weightChannel = 0; weightChannel < STFMesh.weights.Count; weightChannel++)
				{
					if(!STFMesh.weights[weightChannel].indexed)
					{
						for(int weightIndex = 0; weightIndex < vertex_count; weightIndex++)
						{
							var width = STFMesh.float_width + STFMesh.bone_indices_width;
							var offset = weightIndex * width;

							var boneIndex = parseInt(STFMesh.weights[weightChannel].buffer.Data, offset, STFMesh.bone_indices_width, 0);
							var weight = parseFloat(STFMesh.weights[weightChannel].buffer.Data, offset, STFMesh.float_width, STFMesh.bone_indices_width);
							if(boneIndex >= 0 && weight != 0)
							{
								weights[weightIndex].Add(new BoneWeight1 {boneIndex = boneIndex, weight = weight});
							}
						}
					}
					else
					{
						for(int weightIndex = 0; weightIndex < (int)STFMesh.weights[weightChannel].count; weightIndex++)
						{
							var width = STFMesh.indices_width + STFMesh.float_width + STFMesh.bone_indices_width;
							var offset = weightIndex * width;

							var vertIndex = parseInt(STFMesh.weights[weightChannel].buffer.Data, offset, STFMesh.indices_width, 0);
							var boneIndex = parseInt(STFMesh.weights[weightChannel].buffer.Data, offset, STFMesh.bone_indices_width, STFMesh.indices_width);
							var weight = parseFloat(STFMesh.weights[weightChannel].buffer.Data, offset, STFMesh.float_width, STFMesh.indices_width + STFMesh.bone_indices_width);

							weights[vertIndex].Add(new BoneWeight1 {boneIndex = boneIndex, weight = weight});
						}
					}
				}

				var unity_weights = new List<BoneWeight1>();
				var bonesPerVertex = new byte[deduped_split_indices.Count];
				for(int i = 0; i < deduped_split_indices.Count; i++)
				{
					var boneWeights = weights[splits[deduped_split_indices[i]]].OrderByDescending(b => b.weight).ToList();

					if(boneWeights.Count > 0)
					{
						bonesPerVertex[i] = (byte)Math.Min(boneWeights.Count, MAX_BONES_PER_VERTEX);

						var sum_weights = .0;
						/*foreach(var weight in boneWeights)
							sum_weights += weight.weight;*/
						for(int weightIndex = 0; weightIndex < boneWeights.Count && weightIndex < MAX_BONES_PER_VERTEX; weightIndex++)
							sum_weights += boneWeights[weightIndex].weight;
						sum_weights /= boneWeights.Count;

						/*foreach(var weight in boneWeights.OrderByDescending(b => b.weight))
							unity_weights.Add(new BoneWeight1 {boneIndex = weight.boneIndex, weight = (float)(weight.weight / sum_weights)});*/
						for(int weightIndex = 0; weightIndex < boneWeights.Count && weightIndex < MAX_BONES_PER_VERTEX; weightIndex++)
							unity_weights.Add(new BoneWeight1 {boneIndex = boneWeights[weightIndex].boneIndex, weight = (float)(boneWeights[weightIndex].weight / sum_weights)});


						if(i < 10) {
							var s = "";
							foreach(var weight in boneWeights.OrderByDescending(b => b.weight))
								s += weight.weight + ", ";
							Debug.Log(s);
						}
					}
					else
					{
						bonesPerVertex[i] = 1;
						unity_weights.Add(new BoneWeight1 {boneIndex = 0, weight = 1});
					}
				}

				Debug.Log(STFMesh.STF_Name);

				ret.SetBoneWeights(new NativeArray<byte>(bonesPerVertex, Allocator.Temp), new NativeArray<BoneWeight1>(unity_weights.ToArray(), Allocator.Temp));
				ret.bindposes = STFMesh.armature.Bindposes.ToArray();
			}


			ret.RecalculateBounds();
			ret.UploadMeshData(false);

			return ret;
		}
	}
}
