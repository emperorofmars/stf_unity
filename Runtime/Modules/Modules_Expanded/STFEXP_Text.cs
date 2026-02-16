using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules.stfexp
{
	public class STFEXP_Text : STF_DataResource
	{
		public const string _STF_Type = "stfexp.text";
		public override string STF_Type => _STF_Type;

		public string text = "";
	}

	public class STFEXP_Text_Module : ISTF_Module
	{
		public string STF_Type => STFEXP_Text._STF_Type;
		public string STF_Kind => "data";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"text"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_Text)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<STFEXP_Text>();
			ret.SetFromJson(JsonResource, STF_Id, "STFEXP Light");

			if(JsonResource.ContainsKey("text")) ret.text = JsonResource.Value<string>("text");

			return (ret, new() {ret});
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new System.NotImplementedException();
		}
	}
}
