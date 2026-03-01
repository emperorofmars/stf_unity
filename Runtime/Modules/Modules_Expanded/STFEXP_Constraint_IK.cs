using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules.stfexp
{
	public class STFEXP_Constraint_IK : STF_NodeComponentResource
	{
		public const string _STF_Type = "stfexp.constraint.ik";
		public override string STF_Type => _STF_Type;
		public int ChainLength = 1;
		public List<string> TargetPath = new();
		public GameObject TargetGo;
		public List<string> PolePath = new();
		public GameObject PoleGo;
	}

	public class STFEXP_Constraint_IK_Module : ISTF_Module
	{
		public string STF_Type => STFEXP_Constraint_IK._STF_Type;
		public string STF_Kind => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"constraint", "constraint.ik"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_Constraint_IK)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<STFEXP_Constraint_IK>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STFEXP Constraint IK");

			if(JsonResource.ContainsKey("chain_length")) ret.ChainLength = JsonResource.Value<int>("chain_length");

			if(JsonResource.ContainsKey("target"))
				ret.TargetPath = STFUtil.ConvertResourcePath(JsonResource, JsonResource["target"]);
			if(JsonResource.ContainsKey("pole"))
				ret.PolePath = STFUtil.ConvertResourcePath(JsonResource, JsonResource["pole"]);

			Context.AddTask(new Task(() => {
				if(ret.TargetPath.Count > 0)
					ret.TargetGo = STFUtil.ResolveBinding(Context, ret, ret.TargetPath);
				if(ret.PolePath.Count > 0)
					ret.PoleGo = STFUtil.ResolveBinding(Context, ret, ret.PolePath);
			}));

			if (JsonResource.ContainsKey("enabled") && JsonResource.Value<bool>("enabled") == false)
				ret.enabled = false;

			return (ret, null);
		}

		public void ImportInstanceMod(ImportContext Context, ISTF_Resource Resource, JObject JsonResource)
		{
			var ret = Resource as STFEXP_Constraint_IK;

			if(JsonResource.ContainsKey("chain_length")) ret.ChainLength = JsonResource.Value<int>("chain_length");

			if(JsonResource.ContainsKey("target"))
				ret.TargetPath = STFUtil.ConvertResourcePath(JsonResource, JsonResource["target"]);
			if(JsonResource.ContainsKey("pole"))
				ret.PolePath = STFUtil.ConvertResourcePath(JsonResource, JsonResource["pole"]);

			Context.AddTask(new Task(() => {
				if(ret.TargetPath.Count > 0)
					ret.TargetGo = STFUtil.ResolveBinding(Context, ret, ret.TargetPath);
				if(ret.PolePath.Count > 0)
					ret.PoleGo = STFUtil.ResolveBinding(Context, ret, ret.PolePath);
			}));
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new System.NotImplementedException();
		}
	}
}
