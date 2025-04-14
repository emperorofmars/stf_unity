using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Instance_Armature : STF_InstanceResource
	{
		public const string STF_TYPE = "stf.instance.armature";
		public override string STF_Type => STF_TYPE;

		public STF_Armature Armature;

		// TODO pose & mods
	}

	public class STF_Instance_Armature_Module : ISTF_Module
	{
		public string STF_Type => STF_Instance_Armature.STF_TYPE;

		public string STF_Kind => "instance";

		public int Priority => 0;

		public List<string> LikeTypes => new(){"instance.armature", "instance"};

		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STF_Instance_Armature)};

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public List<STF_ComponentResource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public (ISTF_Resource STFResource, object ApplicationObject) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = (STF_Node)ContextObject;
			var ret = ScriptableObject.CreateInstance<STF_Instance_Armature>();
			go.Instance = ret;
			ret.SetFromJson(JsonResource, STF_Id, "STF Armature Instance");

			ret.Armature = (STF_Armature)Context.ImportResource((string)JsonResource["armature"]);

			var instance = Object.Instantiate(ret.Armature.gameObject, go.transform);
			for(var child_count = instance.transform.childCount; child_count > 0; child_count--)
			{
				instance.transform.GetChild(child_count - 1).SetParent(go.transform);
			}
			#if UNITY_EDITOR
			Object.DestroyImmediate(instance);
			#else
			Object.Destroy(instance);
			#endif

			return (ret, ret);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var PrefabObject = ApplicationObject as STF_Instance_Armature;
			var ret = new JObject {
				{"type", STF_Type},
				{"name", PrefabObject.STF_Name},
			};

			return (ret, PrefabObject.STF_Id);
		}
	}
}
