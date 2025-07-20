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
	public class AVA_SecondaryMotion : STF_NodeComponentResource
	{
		public const string _STF_Type = "ava.secondary_motion";
		public override string STF_Type => _STF_Type;

		public float intensity = 0.3f;
	}

	public class AVA_SecondaryMotion_Module : ISTF_Module
	{
		public string STF_Type => AVA_SecondaryMotion._STF_Type;

		public string STF_Kind => "component";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"visemes"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(AVA_SecondaryMotion)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<AVA_SecondaryMotion>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "AVA Secondary Motion");

			if (JsonResource.ContainsKey("intensity")) ret.intensity = JsonResource.Value<float>("intensity");

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
	class Register_AVA_SecondaryMotion_Module
	{
		static Register_AVA_SecondaryMotion_Module()
		{
			STF_Module_Registry.RegisterModule(new AVA_SecondaryMotion_Module());
		}
	}
#endif
}