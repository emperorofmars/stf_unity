using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.modules.stfexp
{
	public class STFEXP_Light : STF_InstanceResource
	{
		public const string _STF_Type = "stfexp.light";
		public override string STF_Type => _STF_Type;

		public string light_type = "point";
		public string area_shape = "disc";
		public float brightness;
		public bool use_temperature = false;
		public float temperature;
		public Color color;
		public float radius;
		public float angle;
		public float size;
		public float width;
		public float height;
	}

	public class STFEXP_Light_Module : ISTF_Module
	{
		public string STF_Type => STFEXP_Light._STF_Type;

		public string STF_Kind => "component";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"light"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_Light)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<STFEXP_Light>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STFEXP Light");

			if(JsonResource.ContainsKey("light_type")) ret.light_type = JsonResource.Value<string>("light_type");
			if(JsonResource.ContainsKey("area_shape")) ret.area_shape = JsonResource.Value<string>("area_shape");
			if(JsonResource.ContainsKey("brightness")) ret.brightness = JsonResource.Value<float>("brightness");
			if(JsonResource.ContainsKey("temperature"))
			{
				ret.use_temperature = true;
				ret.temperature = JsonResource.Value<float>("temperature");
			}
			if(JsonResource.ContainsKey("color")) ret.color = new Color((float)JsonResource["color"][0], (float)JsonResource["color"][1], (float)JsonResource["color"][2]);
			if(JsonResource.ContainsKey("radius")) ret.radius = JsonResource.Value<float>("radius");
			if(JsonResource.ContainsKey("angle")) ret.angle = JsonResource.Value<float>("angle");
			if(JsonResource.ContainsKey("size")) ret.size = JsonResource.Value<float>("size");
			if(JsonResource.ContainsKey("width")) ret.width = JsonResource.Value<float>("width");
			if(JsonResource.ContainsKey("height")) ret.height = JsonResource.Value<float>("height");

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
	class Register_STFEXP_Light_Module
	{
		static Register_STFEXP_Light_Module()
		{
			STF_Module_Registry.RegisterModule(new STFEXP_Light_Module());
		}
	}
#endif
}
