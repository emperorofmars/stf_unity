using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules.stfexp
{
	public class STFEXP_Instance_Text : STF_InstanceResource
	{
		public const string _STF_Type = "stfexp.instance.text";
		public override string STF_Type => _STF_Type;

		public STFEXP_Text Text;
	}

	public class STFEXP_Instance_Text_Module : ISTF_Module
	{
		public string STF_Type => STFEXP_Instance_Text._STF_Type;
		public string STF_Kind => "instance";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"instance.text"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_Instance_Text)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<STFEXP_Instance_Text>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STFEXP Light");

			if(JsonResource.ContainsKey("text"))
			{
				if(STFUtil.ImportResource(Context, JsonResource, JsonResource.Value<int>("text")) is STFEXP_Text text && text != null)
					ret.Text = text;
			}

			if (JsonResource.ContainsKey("enabled") && JsonResource.Value<bool>("enabled") == false)
				ret.enabled = false;

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new System.NotImplementedException();
		}
	}
}
