using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Instance_Mesh : STF_InstanceResource
	{
		public const string STF_TYPE = "stf.instance.mesh";
		public override string STF_Type => STF_TYPE;

		public STF_Mesh Mesh;
		public GameObject ArmatureInstance;
		public Renderer UnityMeshInstance;
	}

	public class STF_Instance_Mesh_Module : ISTF_Module
	{
		public string STF_Type => STF_Instance_Mesh.STF_TYPE;

		public string STF_Kind => "instance";

		public int Priority => 0;

		public List<string> LikeTypes => new(){"instance.mesh", "instance"};

		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STF_Instance_Mesh)};

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public List<STF_ComponentResource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = (STF_Node)ContextObject;
			var ret = go.gameObject.AddComponent<STF_Instance_Mesh>();
			//var ret = ScriptableObject.CreateInstance<STF_Instance_Mesh>();
			go.Instance = ret;
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STF Instance Mesh");

			Context.AddTask(new Task(() => {
				if(JsonResource.ContainsKey("armature_instance"))
				{
					ret.ArmatureInstance = ((STF_MonoBehaviour)Context.ImportResource((string)JsonResource["armature_instance"])).gameObject;
				}

				ret.Mesh = (STF_Mesh)Context.ImportResource((string)JsonResource["mesh"]);

				if(ret.Mesh.ProcessedUnityMesh)
				{
					if(ret.Mesh.weights != null && ret.Mesh.weights.Count > 0 || ret.Mesh.blendshapes != null && ret.Mesh.blendshapes.Count > 0)
					{
						var renderer = go.gameObject.AddComponent<SkinnedMeshRenderer>();
						renderer.sharedMesh = ret.Mesh.ProcessedUnityMesh;
						renderer.materials = new Material[ret.Mesh.material_slots.Count];
						if(ret.ArmatureInstance)
						{
							var instance = ret.ArmatureInstance.GetComponent<STF_Instance_Armature>();
							renderer.rootBone = ret.ArmatureInstance.transform;
							var bones = new Transform[ret.Mesh.bones.Count];
							for(int i = 0; i < ret.Mesh.bones.Count; i++)
							{
								var id = ret.Mesh.bones[i];
								var bone = ret.ArmatureInstance.GetComponentsInChildren<STF_NodeResource>().FirstOrDefault(bone => bone.STF_Id == id && bone.STF_Owner == instance.gameObject);
								bones[i] = bone ? bone.transform : instance.transform;
							}
							renderer.bones = bones;
						}

						ret.UnityMeshInstance = renderer;
					}
					else
					{
						var meshFilter = go.gameObject.AddComponent<MeshFilter>();
						meshFilter.sharedMesh = ret.Mesh.ProcessedUnityMesh;
						var renderer = go.gameObject.AddComponent<MeshRenderer>();
						renderer.materials = new Material[ret.Mesh.material_slots.Count];
					}
				}
			}));

			// TODO material slots & blendshape values

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var PrefabObject = ApplicationObject as STF_Mesh;
			var ret = new JObject {
				{"type", STF_Type},
				{"name", PrefabObject.STF_Name},
			};

			return (ret, PrefabObject.STF_Id);
		}
	}
}
