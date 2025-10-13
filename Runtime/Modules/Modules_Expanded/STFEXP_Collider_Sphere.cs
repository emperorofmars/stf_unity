using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.modules.stfexp
{
	public class STFEXP_Collider_Sphere : STF_NodeComponentResource
	{
		public const string _STF_Type = "stfexp.collider.sphere";
		public override string STF_Type => _STF_Type;

		public float radius = 1;
		public Vector3 offset_position;


		public override bool CanHandleInstanceMod => true;

		public override void HandleInstanceMod(ImportContext Context, JObject JsonResource)
		{
			if (JsonResource.ContainsKey("radius")) radius = JsonResource.Value<float>("radius");
			if (JsonResource.ContainsKey("offset_position")) offset_position = TRSUtil.ParseVector3(JsonResource["offset_position"] as JArray);
		}
	}

	public class STFEXP_Collider_Sphere_Module : ISTF_Module
	{
		public string STF_Type => STFEXP_Collider_Sphere._STF_Type;

		public string STF_Kind => "component";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"collider", "collider.sphere"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_Collider_Sphere)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<STFEXP_Collider_Sphere>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STFEXP Sphere Collider");

			if (JsonResource.ContainsKey("radius")) ret.radius = JsonResource.Value<float>("radius");
			if (JsonResource.ContainsKey("offset_position")) ret.offset_position = TRSUtil.ParseVector3(JsonResource["offset_position"] as JArray);

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
	class Register_STFEXP_Collider_Sphere_Module
	{
		static Register_STFEXP_Collider_Sphere_Module()
		{
			STF_Module_Registry.RegisterModule(new STFEXP_Collider_Sphere_Module());
		}
	}
#endif
}
