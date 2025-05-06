using System;
using System.Buffers;
using System.Buffers.Binary;
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

			// Whenever VRChat bothers to update to a Unity version that supports new enough C# for this to work -.-
			/*var vertices = new Vector3[STFMesh.vertex_count];
			{
				var bufferReader = new SequenceReader<byte>(new ReadOnlySequence<byte>(STFMesh.vertices.Data));
				for(long i = 0; i < (long)STFMesh.vertex_count; i++)
				{
					BinaryPrimitives.ReadSingleLittleEndian(bufferReader.UnreadSpan);
					---
				}
			}*/

			static float readFloat(byte[] Bytes, ulong Position)
			{
				var @byte = new byte[4];
				for(uint i = 0; i < 4; i++)
				{
					@byte[i] = Bytes[Position * 4 + i];
				}
				return BitConverter.ToSingle(@byte, 0);
			}

			static uint readUInt(byte[] Bytes, ulong Position)
			{
				var @byte = new byte[4];
				for(uint i = 0; i < 4; i++)
				{
					@byte[i] = Bytes[Position * 4 + i];
				}
				return BitConverter.ToUInt32(@byte, 0);
			}

			static ulong readULong(byte[] Bytes, ulong Position)
			{
				var @byte = new byte[8];
				for(uint i = 0; i < 8; i++)
				{
					@byte[i] = Bytes[Position * 8 + i];
				}
				return BitConverter.ToUInt64(@byte, 0);
			}


			// TODO make the binary shuffeling its own util
			var vertices = new Vector3[STFMesh.vertex_count];
			for(ulong i = 0; i < STFMesh.vertex_count; i++)
			{
				vertices[i].Set(
					-readFloat(STFMesh.vertices.Data, i * 3),
					readFloat(STFMesh.vertices.Data, i * 3 + 1),
					readFloat(STFMesh.vertices.Data, i * 3 + 2)
				);
			}

			var splits = new ulong[STFMesh.split_count];
			for(ulong i = 0; i < STFMesh.split_count; i++)
			{
				splits[i] = STFMesh.split_indices_width switch
				{
					4 => readUInt(STFMesh.splits.Data, i),
					8 => readULong(STFMesh.splits.Data, i),
					_ => throw new STFException("Invalid Split width", ErrorSeverity.FATAL_ERROR, STF_Type, null, null),
				};
			}

			// normals, tangents, uvs, ...

			// minimize splits

			// tris

			// faces

			// face material indices


			ret.SetVertices(vertices);


			// convert finally to unity submeshes
			ret.subMeshCount = STFMesh.material_slots.Count;
			for(int subMeshIdx = 0; subMeshIdx < STFMesh.material_slots.Count; subMeshIdx++)
			{


				/*var primitive = STFMesh.material_slots[subMeshIdx];
				var indicesPos = (int)primitive["indices_pos"];
				var indicesLen = (int)primitive["indices_len"];

				var indexBuffer = new int[indicesLen];
				Buffer.BlockCopy(arrayBuffer, indicesPosCounted * sizeof(int) + vertexCount * bufferWidth * sizeof(float), indexBuffer, 0, indicesLen * sizeof(int));

				ret.SetIndices(indexBuffer, MeshTopology.Triangles, subMeshIdx);

				indicesPosCounted += indicesLen;*/
			}


			return ret;
		}
	}
}
