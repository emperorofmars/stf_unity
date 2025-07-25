using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.modules.stfexp
{
	public class STFEXP_Constraint_Twist : STF_NodeComponentResource
	{
		public const string _STF_Type = "stfexp.constraint.twist";
		public override string STF_Type => _STF_Type;

		public float Weight = 0.5f;
		public List<string> Target = new();
		public GameObject TargetGo;
	}

	public class STFEXP_Constraint_Twist_Module : ISTF_Module
	{
		public string STF_Type => STFEXP_Constraint_Twist._STF_Type;

		public string STF_Kind => "component";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"constraint"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_Constraint_Twist)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<STFEXP_Constraint_Twist>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STFEXP Constraint Twist");

			ret.Weight = JsonResource.Value<float>("weight");

			if (JsonResource.ContainsKey("target"))
				ret.Target = JsonResource["target"].ToObject<List<string>>();

			Context.AddTask(new Task(() => {
				if (ret.Target.Count > 0)
					ret.TargetGo = STFUtil.ResolveBinding(Context, ret, ret.Target);
				else
					ret.TargetGo = ret.transform?.parent?.parent?.gameObject;
			}));

			if (JsonResource.ContainsKey("enabled") && JsonResource.Value<bool>("enabled") == false)
				ret.enabled = false;

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_STFEXP_Constraint_Twist
	{
		static Register_STFEXP_Constraint_Twist()
		{
			STF_Module_Registry.RegisterModule(new STFEXP_Constraint_Twist_Module());
		}
	}
#endif
}