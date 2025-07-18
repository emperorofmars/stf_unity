
using System;
using System.Collections.Generic;
using System.Linq;
using com.squirrelbite.stf_unity.modules;
using Unity.Collections;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class STF_Mesh_Processor : ISTF_Processor
	{
		public Type TargetType => typeof(STF_Mesh);
		public uint Order => 20;
		public int Priority => 1;

		public (List<UnityEngine.Object>, List<UnityEngine.Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var STFMesh = STFResource as STF_Mesh;
			var ret = new Mesh { name = STFMesh._STF_Name };

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
			for (int i = 0; i < vertex_count; i++)
			{
				vertices[i].Set(
					-parseFloat(STFMesh.vertices.Data, i * STFMesh.float_width * 3, STFMesh.float_width),
					parseFloat(STFMesh.vertices.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width),
					parseFloat(STFMesh.vertices.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width * 2)
				);
			}

			var splits = new int[split_count];
			for (int i = 0; i < split_count; i++)
				splits[i] = parseInt(STFMesh.splits.Data, i * STFMesh.indices_width, STFMesh.indices_width);

			var normals = new Vector3[split_count];
			for (int i = 0; i < split_count; i++)
			{
				normals[i].Set(
					-parseFloat(STFMesh.split_normals.Data, i * STFMesh.float_width * 3, STFMesh.float_width),
					parseFloat(STFMesh.split_normals.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width),
					parseFloat(STFMesh.split_normals.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width * 2)
				);
				normals[i].Normalize();
			}

			var uvs = new List<Vector2[]>();
			foreach (var uvBuffer in STFMesh.uvs)
			{
				var uv = new Vector2[split_count];
				for (int i = 0; i < split_count; i++)
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
				foreach (var uv in uvs)
				{
					if ((uv[a] - uv[b]).magnitude > 0.0001) return false;
				}
				return true;
			}

			// TODO colors


			var verts_to_split = new Dictionary<int, List<int>>();
			var deduped_split_indices = new List<int>();
			var split_to_deduped_split_index = new List<int>();
			for (int splitIndex = 0; splitIndex < split_count; splitIndex++)
			{
				var vertexIndex = (int)splits[splitIndex];
				if (!verts_to_split.ContainsKey(vertexIndex))
				{
					verts_to_split.Add(vertexIndex, new List<int> { splitIndex });
					deduped_split_indices.Add(splitIndex);
					split_to_deduped_split_index.Add(deduped_split_indices.Count - 1);
				}
				else
				{
					var success = false;
					for (int candidateIndex = 0; candidateIndex < verts_to_split[vertexIndex].Count; candidateIndex++)
					{
						var splitCandidate = verts_to_split[vertexIndex][candidateIndex];
						if (
							(normals[splitIndex] - normals[splitCandidate]).magnitude < 0.0001
							&& compareUVs(splitIndex, splitCandidate)
						// TODO colors
						)
						{
							split_to_deduped_split_index.Add(split_to_deduped_split_index[splitCandidate]);
							success = true;
							break;
						}
					}
					if (!success)
					{
						verts_to_split[vertexIndex].Add(splitIndex);
						deduped_split_indices.Add(splitIndex);
						split_to_deduped_split_index.Add(deduped_split_indices.Count - 1);
					}
				}
			}


			var unity_vertices = new List<Vector3>();
			var unity_normals = new List<Vector3>();
			var unity_uvs = new List<List<Vector2>>();
			for (int uvIndex = 0; uvIndex < uvs.Count; uvIndex++)
			{
				unity_uvs.Add(new());
			}
			for (int i = 0; i < deduped_split_indices.Count; i++)
			{
				unity_vertices.Add(vertices[splits[deduped_split_indices[i]]]);
				unity_normals.Add(normals[deduped_split_indices[i]]);
				for (int uvIndex = 0; uvIndex < uvs.Count; uvIndex++)
				{
					unity_uvs[uvIndex].Add(uvs[uvIndex][deduped_split_indices[i]]);
				}
			}


			var tris = new Vector3Int[tris_count];
			for (int i = 0; i < tris_count; i++)
			{
				tris[i].Set(
					parseInt(STFMesh.tris.Data, i * STFMesh.indices_width * 3, STFMesh.indices_width, STFMesh.indices_width * 2),
					parseInt(STFMesh.tris.Data, i * STFMesh.indices_width * 3, STFMesh.indices_width, STFMesh.indices_width),
					parseInt(STFMesh.tris.Data, i * STFMesh.indices_width * 3, STFMesh.indices_width)
				);
			}

			var faceLengths = new int[face_count];
			var faceMaterialIndices = new int[face_count];
			for (int i = 0; i < face_count; i++)
			{
				faceLengths[i] = parseInt(STFMesh.faces.Data, i * STFMesh.indices_width, STFMesh.indices_width);
				faceMaterialIndices[i] = parseInt(STFMesh.material_indices.Data, i * STFMesh.material_indices_width, STFMesh.material_indices_width);
			}

			var subMeshIndices = new List<List<int>>();
			var trisIndex = 0;
			for (int faceIndex = 0; faceIndex < face_count; faceIndex++)
			{
				var matIndex = (int)faceMaterialIndices[faceIndex];
				for (uint faceLen = 0; faceLen < faceLengths[faceIndex]; faceLen++)
				{
					while (subMeshIndices.Count <= matIndex) subMeshIndices.Add(new List<int>());

					subMeshIndices[matIndex].Add(split_to_deduped_split_index[tris[trisIndex].x]);
					subMeshIndices[matIndex].Add(split_to_deduped_split_index[tris[trisIndex].y]);
					subMeshIndices[matIndex].Add(split_to_deduped_split_index[tris[trisIndex].z]);

					trisIndex++;
				}
			}


			ret.SetVertices(unity_vertices);
			ret.SetNormals(unity_normals);
			ret.RecalculateTangents();
			for (int uvIndex = 0; uvIndex < unity_uvs.Count; uvIndex++)
			{
				ret.SetUVs(uvIndex, unity_uvs[uvIndex]);
			}

			ret.indexFormat = unity_vertices.Count > ushort.MaxValue ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
			ret.subMeshCount = subMeshIndices.Count;
			for (int subMeshIdx = 0; subMeshIdx < subMeshIndices.Count; subMeshIdx++)
			{
				ret.SetIndices(subMeshIndices[subMeshIdx], MeshTopology.Triangles, subMeshIdx);
			}


			if (STFMesh.armature != null && STFMesh.bones != null && STFMesh.weights != null)
			{
				const int MAX_BONES_PER_VERTEX = 4;
				var weights = new List<BoneWeight1>[vertex_count];
				for (int i = 0; i < vertex_count; i++) weights[i] = new List<BoneWeight1>();

				var position = 0;
				for (int vertexIndex = 0; vertexIndex < vertex_count; vertexIndex++)
				{
					var numWeights = parseInt(STFMesh.weight_lens.Data, vertexIndex * STFMesh.weight_lens_width, STFMesh.weight_lens_width, 0);
					for (int weightIndex = 0; weightIndex < numWeights; weightIndex++)
					{
						var offset = position;

						var boneIndex = parseInt(STFMesh.bone_indices.Data, offset * STFMesh.bone_indices_width, STFMesh.bone_indices_width, 0);
						var weight = parseFloat(STFMesh.weights.Data, offset * STFMesh.float_width, STFMesh.float_width, 0);
						if (boneIndex >= 0 && weight != 0)
						{
							weights[vertexIndex].Add(new BoneWeight1 { boneIndex = boneIndex, weight = weight });
						}
						position++;
					}
				}

				var unity_weights = new List<BoneWeight1>();
				var bonesPerVertex = new byte[deduped_split_indices.Count];
				for (int i = 0; i < deduped_split_indices.Count; i++)
				{
					var boneWeights = weights[splits[deduped_split_indices[i]]].OrderByDescending(b => b.weight).ToList();

					if (boneWeights.Count > 0)
					{
						bonesPerVertex[i] = (byte)Math.Min(boneWeights.Count, MAX_BONES_PER_VERTEX);

						var sum_weights = .0;
						for (int weightIndex = 0; weightIndex < boneWeights.Count && weightIndex < MAX_BONES_PER_VERTEX; weightIndex++)
							sum_weights += boneWeights[weightIndex].weight;

						for (int weightIndex = 0; weightIndex < boneWeights.Count && weightIndex < MAX_BONES_PER_VERTEX; weightIndex++)
							unity_weights.Add(new BoneWeight1 { boneIndex = boneWeights[weightIndex].boneIndex, weight = (float)(boneWeights[weightIndex].weight / sum_weights) });
					}
					else
					{
						bonesPerVertex[i] = 1;
						unity_weights.Add(new BoneWeight1 { boneIndex = 0, weight = 1 });
					}
				}

				ret.SetBoneWeights(new NativeArray<byte>(bonesPerVertex, Allocator.Temp), new NativeArray<BoneWeight1>(unity_weights.ToArray(), Allocator.Temp));
				var bindposes = new List<Matrix4x4>();
				foreach (var id in STFMesh.bones)
					bindposes.Add(STFMesh.armature.Bindposes[STFMesh.armature.BindOrder.FindIndex(b => b == id)]);
				ret.bindposes = bindposes.ToArray();
			}

			if (STFMesh.blendshapes.Count > 0)
			{
				foreach (var stfBlendshape in STFMesh.blendshapes)
				{
					var blendshapePositions = new Vector3[deduped_split_indices.Count];
					var blendshapeNormals = new Vector3[deduped_split_indices.Count];
					var blendshapeTangents = new Vector3[deduped_split_indices.Count];

					Array.Clear(blendshapePositions, 0, blendshapePositions.Length);
					Array.Clear(blendshapeNormals, 0, blendshapeNormals.Length);
					Array.Clear(blendshapeTangents, 0, blendshapeTangents.Length);

					for (int i = 0; i < (int)stfBlendshape.position_offsets.BufferLength / (STFMesh.float_width * 3); i++)
					{
						var vertexIndex = stfBlendshape.indices != null ? parseInt(stfBlendshape.indices.Data, i * STFMesh.indices_width, STFMesh.indices_width, 0) : i;

						var blendshapePosition = new Vector3(
							-parseFloat(stfBlendshape.position_offsets.Data, i * STFMesh.float_width * 3, STFMesh.float_width, 0),
							parseFloat(stfBlendshape.position_offsets.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width),
							parseFloat(stfBlendshape.position_offsets.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width * 2)
						);
						foreach (var split_index in verts_to_split[vertexIndex])
							blendshapePositions[split_to_deduped_split_index[split_index]] = blendshapePosition;
					}

					if (stfBlendshape.split_normals != null && stfBlendshape.split_normals.BufferLength == stfBlendshape.position_offsets.BufferLength)
					{
						for (int i = 0; i < (int)stfBlendshape.split_normals.BufferLength / (STFMesh.float_width * 3); i++)
						{
							var splitIndex = stfBlendshape.split_indices != null ? parseInt(stfBlendshape.split_indices.Data, i * STFMesh.indices_width, STFMesh.indices_width, 0) : i;
							var blendshapeSplitNormal = new Vector3(
								-parseFloat(stfBlendshape.split_normals.Data, i * STFMesh.float_width * 3, STFMesh.float_width, 0),
								parseFloat(stfBlendshape.split_normals.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width),
								parseFloat(stfBlendshape.split_normals.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width * 2)
							);
							blendshapeTangents[split_to_deduped_split_index[splitIndex]] = blendshapeSplitNormal;
						}
					}

					ret.AddBlendShapeFrame(stfBlendshape.name, 100, blendshapePositions, blendshapeNormals, blendshapeTangents);
				}
			}


			ret.RecalculateBounds();
			ret.UploadMeshData(false);

			return (new List<UnityEngine.Object>() { ret }, new List<UnityEngine.Object>() { ret });
		}
	}
}
