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
	public class STFEXP_LightprobeAnchor : STF_NodeComponentResource
	{
		public const string _STF_Type = "stfexp.lightprobe_anchor";
		public override string STF_Type => _STF_Type;

		public List<string> Anchor = new();
		public GameObject TargetGo;
	}

	public class STFEXP_LightprobeAnchor_Module : ISTF_Module
	{
		public string STF_Type => STFEXP_LightprobeAnchor._STF_Type;

		public string STF_Kind => "component";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"constraint"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_LightprobeAnchor)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<STFEXP_LightprobeAnchor>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STFEXP Constraint Twist");

			if (JsonResource.ContainsKey("anchor"))
				ret.Anchor = JsonResource["anchor"].ToObject<List<string>>();

			Context.AddTask(new Task(() => {
				if (ret.Anchor.Count > 0)
					ret.TargetGo = STFUtil.ResolveBinding(Context, ret, ret.Anchor);
				else
					ret.TargetGo = ret.transform?.parent?.parent?.gameObject;
			}));

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_STFEXP_LightprobeAnchor
	{
		static Register_STFEXP_LightprobeAnchor()
		{
			STF_Module_Registry.RegisterModule(new STFEXP_LightprobeAnchor_Module());
		}
	}
#endif
}