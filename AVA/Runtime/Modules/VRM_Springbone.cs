using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava
{
	public class VRM_Springbone : STF_NodeComponentResource
	{
		[System.Serializable]
		public class SpringboneTarget
		{
			public List<string> Target = new();
		}
		public const string _STF_Type = "dev.vrm.springbone";
		public override string STF_Type => _STF_Type;

		public float stiffness = 1f;
		public float gravityPower = 0f;
		public Vector3 gravityDir = Vector3.down;
		public float dragForce = 0.4f;
		public SpringboneTarget center = new();
		public float hitRadius = 0.02f;
		public List<SpringboneTarget> Colliders = new();
	}

	public class VRM_Springbone_Module : ISTF_Module
	{
		public string STF_Type => VRM_Springbone._STF_Type;
		public string STF_Kind => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"secondary_motion"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(VRM_Springbone)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<VRM_Springbone>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "VRM Springbone");

			if (JsonResource.ContainsKey("stiffness")) ret.stiffness = JsonResource.Value<float>("stiffness");
			if (JsonResource.ContainsKey("gravityPower")) ret.gravityPower = JsonResource.Value<float>("gravityPower");
			if (JsonResource.ContainsKey("gravityDir")) ret.gravityDir = TRSUtil.ParseVector3(JsonResource["gravityDir"] as JArray);
			if (JsonResource.ContainsKey("dragForce")) ret.dragForce = JsonResource.Value<float>("dragForce");
			if (JsonResource.ContainsKey("center")) ret.center.Target = STFUtil.ConvertResourcePath(JsonResource, JsonResource["center"]);
			if (JsonResource.ContainsKey("hitRadius")) ret.stiffness = JsonResource.Value<float>("hitRadius");
			if (JsonResource.ContainsKey("colliders")) foreach(var colliderPath in JsonResource["colliders"])
				ret.Colliders.Add(new() {Target=STFUtil.ConvertResourcePath(JsonResource, colliderPath)});

			if (JsonResource.ContainsKey("enabled") && JsonResource.Value<bool>("enabled") == false)
				ret.enabled = false;

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new System.NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_VRM_Springbone_Module
	{
		static Register_VRM_Springbone_Module()
		{
			STF_Module_Registry.RegisterModule(new VRM_Springbone_Module());
		}
	}
#endif
}