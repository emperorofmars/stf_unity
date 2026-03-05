using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.handlers.stfexp
{
	[AddComponentMenu("STF/Resources/stfexp/stfexp.lightprobe_anchor")]
	[HelpURL("https://docs.stfform.at/modules/stfexp/stfexp_lightprobe_anchor.html")]
	public class STFEXP_LightprobeAnchor : STF_NodeComponentResource
	{
		public const string _STF_Type = "stfexp.lightprobe_anchor";
		public override string STF_Type => _STF_Type;

		public List<string> Anchor = new();
		public GameObject TargetGo;
	}

	public class STFEXP_LightprobeAnchor_Handler : ISTF_Handler
	{
		public string STF_Type => STFEXP_LightprobeAnchor._STF_Type;
		public string STF_Category => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"constraint"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_LightprobeAnchor)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<STFEXP_LightprobeAnchor>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STFEXP Constraint Twist");

			if (JsonResource.ContainsKey("anchor"))
				ret.Anchor = STFUtil.ConvertResourcePath(JsonResource, JsonResource["anchor"]);

			Context.AddTask(new Task(() => {
				if (ret.Anchor.Count > 0)
					ret.TargetGo = STFUtil.ResolveBinding(Context, ret, ret.Anchor);
				else if(ret.transform.parent && ret.transform.parent.parent)
					ret.TargetGo = ret.transform.parent.parent.gameObject;
			}));

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new System.NotImplementedException();
		}
	}
}
