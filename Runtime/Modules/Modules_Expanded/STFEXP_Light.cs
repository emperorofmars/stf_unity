using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules.stfexp
{
	public class STFEXP_Light : STF_InstanceResource
	{
		public const string _STF_Type = "stfexp.light";
		public override string STF_Type => _STF_Type;

		public string light_type = "point";
		public float brightness = 1;
		public bool use_temperature = false;
		public float temperature = 6570;
		public Color color = Color.white;
		public float range = 10;
		public float spot_angle = 30 * Mathf.Deg2Rad;
		public bool shadow = false;
	}

	public class STFEXP_Light_Module : ISTF_Module
	{
		public string STF_Type => STFEXP_Light._STF_Type;
		public string STF_Kind => "instance";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"light"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_Light)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<STFEXP_Light>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STFEXP Light");

			if(JsonResource.ContainsKey("light_type")) ret.light_type = JsonResource.Value<string>("light_type");
			if(JsonResource.ContainsKey("brightness")) ret.brightness = JsonResource.Value<float>("brightness");
			if(JsonResource.ContainsKey("temperature"))
			{
				ret.use_temperature = true;
				ret.temperature = JsonResource.Value<float>("temperature");
			}
			if(JsonResource.ContainsKey("color")) ret.color = new Color((float)JsonResource["color"][0], (float)JsonResource["color"][1], (float)JsonResource["color"][2]);
			if(JsonResource.ContainsKey("range")) ret.range = JsonResource.Value<float>("range");
			if(JsonResource.ContainsKey("spot_angle")) ret.spot_angle = JsonResource.Value<float>("spot_angle");

			if(JsonResource.ContainsKey("shadow")) ret.shadow = JsonResource.Value<bool>("shadow");

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
