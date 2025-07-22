
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


			// Facecorners & Splits
			var splits = new int[split_count];
			for (int i = 0; i < split_count; i++)
				splits[i] = parseInt(STFMesh.splits.Data, i * STFMesh.indices_width, STFMesh.indices_width);

			var face_corners = STFMesh.face_corners != null ? new int[STFMesh.face_corners.BufferLength / STFMesh.indices_width] : new int[split_count];
			if (STFMesh.face_corners != null)
			{
				for (int i = 0; i < face_corners.Length; i++)
					face_corners[i] = parseInt(STFMesh.face_corners.Data, i * STFMesh.indices_width, STFMesh.indices_width);
			}
			else
			{
				for (int i = 0; i < split_count; i++)
					face_corners[i] = i;
			}


			// Vertices
			var vertices = new Vector3[vertex_count];
			for (int i = 0; i < vertex_count; i++)
			{
				vertices[i].Set(
					-parseFloat(STFMesh.vertices.Data, i * STFMesh.float_width * 3, STFMesh.float_width),
					parseFloat(STFMesh.vertices.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width),
					parseFloat(STFMesh.vertices.Data, i * STFMesh.float_width * 3, STFMesh.float_width, STFMesh.float_width * 2)
				);
			}

			var verts_to_split = new Dictionary<int, List<int>>();
			for (int splitIndex = 0; splitIndex < split_count; splitIndex++)
			{
				var vertexIndex = splits[splitIndex];
				if (!verts_to_split.ContainsKey(vertexIndex))
					verts_to_split.Add(vertexIndex, new List<int> { splitIndex });
				else
					verts_to_split[vertexIndex].Add(splitIndex);
			}

			var unity_vertices = new List<Vector3>();
			for (int i = 0; i < splits.Count(); i++)
				unity_vertices.Add(vertices[splits[i]]);
			ret.SetVertices(unity_vertices);


			// Normals
			if (STFMesh.split_normals != null)
			{
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
				ret.SetNormals(normals);
				ret.RecalculateTangents();
			}
			else
			{
				ret.RecalculateNormals();
				ret.RecalculateTangents();
			}

			// UVs
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
			for (int uvIndex = 0; uvIndex < uvs.Count; uvIndex++)
				ret.SetUVs(uvIndex, uvs[uvIndex]);

			// Colors
			if (STFMesh.split_colors != null)
			{
				var split_colors = new Color[split_count];
				for (int i = 0; i < split_count; i++)
				{
					split_colors[i].r = parseFloat(STFMesh.split_colors.Data, i * STFMesh.float_width * 4, STFMesh.float_width);
					split_colors[i].g = parseFloat(STFMesh.split_colors.Data, i * STFMesh.float_width * 4, STFMesh.float_width, STFMesh.float_width);
					split_colors[i].b = parseFloat(STFMesh.split_colors.Data, i * STFMesh.float_width * 4, STFMesh.float_width, STFMesh.float_width * 2);
					split_colors[i].a = parseFloat(STFMesh.split_colors.Data, i * STFMesh.float_width * 4, STFMesh.float_width, STFMesh.float_width * 3);
				}
				ret.SetColors(split_colors);
			}


			// Topology
			var tris = new Vector3Int[tris_count];
			for (int i = 0; i < tris_count; i++)
			{
				tris[i].Set(
					face_corners[parseInt(STFMesh.tris.Data, i * STFMesh.indices_width * 3, STFMesh.indices_width, STFMesh.indices_width * 2)],
					face_corners[parseInt(STFMesh.tris.Data, i * STFMesh.indices_width * 3, STFMesh.indices_width, STFMesh.indices_width)],
					face_corners[parseInt(STFMesh.tris.Data, i * STFMesh.indices_width * 3, STFMesh.indices_width)]
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

					subMeshIndices[matIndex].Add(tris[trisIndex].x);
					subMeshIndices[matIndex].Add(tris[trisIndex].y);
					subMeshIndices[matIndex].Add(tris[trisIndex].z);

					trisIndex++;
				}
			}

			ret.indexFormat = unity_vertices.Count() > ushort.MaxValue ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
			ret.subMeshCount = subMeshIndices.Count;
			for (int subMeshIdx = 0; subMeshIdx < subMeshIndices.Count; subMeshIdx++)
			{
				ret.SetIndices(subMeshIndices[subMeshIdx], MeshTopology.Triangles, subMeshIdx);
			}


			// Weightpaint
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
				var bonesPerVertex = new byte[splits.Count()];
				for (int i = 0; i < splits.Count(); i++)
				{
					var boneWeights = weights[splits[i]].OrderByDescending(b => b.weight).ToList();

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


			// Blendshapes
			if (STFMesh.blendshapes.Count > 0)
			{
				foreach (var stfBlendshape in STFMesh.blendshapes)
				{
					var blendshapePositions = new Vector3[splits.Count()];
					var blendshapeNormals = new Vector3[splits.Count()];
					var blendshapeTangents = new Vector3[splits.Count()];

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
							blendshapePositions[split_index] = blendshapePosition;
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
							blendshapeNormals[splitIndex] = blendshapeSplitNormal;
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
