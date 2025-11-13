using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules.stfexp
{
	public class STFEXP_Constraint_Rotation : STF_NodeComponentResource
	{
		public const string _STF_Type = "stfexp.constraint.rotation";
		public override string STF_Type => _STF_Type;

		[System.Serializable]
		public class Source
		{
			public float Weight = 0.5f;
			public List<string> SourcePath = new();
			public GameObject SourceGo;
		}

		public List<Source> Sources = new();
	}

	public class STFEXP_Constraint_Rotation_InstanceModHandler : STF_InstanceModHandler
	{
		public void HandleInstanceMod(ImportContext Context, ISTF_ComponentResource Resource, JObject JsonResource)
		{
			var ret = Resource as STFEXP_Constraint_Rotation;
			ret.Sources = new();
			if(JsonResource.ContainsKey("sources")) foreach(JObject jsonSource in JsonResource["sources"] as JArray)
			{
				var source = new STFEXP_Constraint_Rotation.Source();
				source.Weight = jsonSource.Value<float>("weight");
				if (jsonSource.ContainsKey("source"))
					source.SourcePath = STFUtil.ConvertResourcePath(JsonResource, jsonSource["source"]);
				ret.Sources.Add(source);

				Context.AddTask(new Task(() => {
					if (source.SourcePath.Count > 0)
						source.SourceGo = STFUtil.ResolveBinding(Context, ret, source.SourcePath);
					else if(ret.transform.parent && ret.transform.parent.parent)
						source.SourceGo = ret.transform.parent.parent.gameObject;
				}));
			}
		}
	}

	public class STFEXP_Constraint_Rotation_Module : ISTF_Module
	{
		public string STF_Type => STFEXP_Constraint_Rotation._STF_Type;
		public string STF_Kind => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"constraint", "constraint.rotation"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_Constraint_Rotation)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<STFEXP_Constraint_Rotation>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STFEXP Constraint Rotation");

			if(JsonResource.ContainsKey("sources")) foreach(JObject jsonSource in (JsonResource["sources"] as JArray).Cast<JObject>())
			{
				var source = new STFEXP_Constraint_Rotation.Source();
				source.Weight = jsonSource.Value<float>("weight");
				if (jsonSource.ContainsKey("source"))
					source.SourcePath = STFUtil.ConvertResourcePath(JsonResource, jsonSource["source"]);
				ret.Sources.Add(source);

				Context.AddTask(new Task(() => {
					if (source.SourcePath.Count > 0)
						source.SourceGo = STFUtil.ResolveBinding(Context, ret, source.SourcePath);
					else if(ret.transform.parent && ret.transform.parent.parent)
						source.SourceGo = ret.transform.parent.parent.gameObject;
				}));
			}

			if (JsonResource.ContainsKey("enabled") && JsonResource.Value<bool>("enabled") == false)
				ret.enabled = false;

			ret.InstanceModHandler = new STFEXP_Constraint_Rotation_InstanceModHandler();
			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new System.NotImplementedException();
		}
	}
}
