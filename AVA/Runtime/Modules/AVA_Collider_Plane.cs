using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava
{
	public class AVA_Collider_Plane : STF_NodeComponentResource
	{
		public const string _STF_Type = "ava.collider.plane";
		public override string STF_Type => _STF_Type;

		public Vector3 offset_position;
		public Quaternion offset_rotation;
	}

	public class AVA_Collider_Plane_Module : ISTF_Module
	{
		public string STF_Type => AVA_Collider_Plane._STF_Type;

		public string STF_Kind => "component";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"collider", "collider.plane"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(AVA_Collider_Plane)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<AVA_Collider_Plane>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "AVA Plane Collider");

			if (JsonResource.ContainsKey("offset_position")) ret.offset_position = TRSUtil.ParseVector3(JsonResource["offset_position"] as JArray);
			if (JsonResource.ContainsKey("offset_rotation")) ret.offset_rotation = TRSUtil.ParseQuat(JsonResource["offset_rotation"] as JArray);

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
	class Register_AVA_Collider_Plane_Module
	{
		static Register_AVA_Collider_Plane_Module()
		{
			STF_Module_Registry.RegisterModule(new AVA_Collider_Plane_Module());
		}
	}
#endif
}