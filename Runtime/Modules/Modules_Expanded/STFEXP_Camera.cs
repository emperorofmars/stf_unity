using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules.stfexp
{
	public class STFEXP_Camera : STF_InstanceResource
	{
		public const string _STF_Type = "stfexp.camera";
		public override string STF_Type => _STF_Type;

		public string projection = "perspective";
		public float aspect_ratio = 16/9;
		public float fov = Mathf.PI / 2;
	}

	public class STFEXP_Camera_Module : ISTF_Module
	{
		public string STF_Type => STFEXP_Camera._STF_Type;
		public string STF_Kind => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"light"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_Camera)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<STFEXP_Camera>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STFEXP Camera");

			if(JsonResource.ContainsKey("projection")) ret.projection = JsonResource.Value<string>("projection");
			if(JsonResource.ContainsKey("aspect_ratio")) ret.aspect_ratio = JsonResource.Value<float>("aspect_ratio");
			if(JsonResource.ContainsKey("fov")) ret.fov = JsonResource.Value<float>("fov");

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
