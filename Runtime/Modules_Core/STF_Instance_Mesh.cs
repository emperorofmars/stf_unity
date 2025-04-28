using System.Collections.Generic;
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
