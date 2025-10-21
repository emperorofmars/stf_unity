using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using com.squirrelbite.stf_unity.modules;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava
{
	public class AVA_EyeRotation_Bone : STF_NodeComponentResource
	{
		public const string _STF_Type = "ava.eye_rotation.bone";
		public override string STF_Type => _STF_Type;

		public float limits_up = 15;
		public float limits_down = 12;
		public float limits_in = 15;
		public float limits_out = 16;
	}

	public class AVA_EyeRotation_Bone_Module : ISTF_Module
	{
		public string STF_Type => AVA_EyeRotation_Bone._STF_Type;
		public string STF_Kind => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"visemes"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(AVA_EyeRotation_Bone)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_PrefabResource;
			var ret = go.gameObject.AddComponent<AVA_EyeRotation_Bone>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "AVA EyeRotation Bone");

			if (JsonResource.ContainsKey("up")) ret.limits_up = JsonResource.Value<float>("up");
			if (JsonResource.ContainsKey("down")) ret.limits_down = JsonResource.Value<float>("down");
			if (JsonResource.ContainsKey("in")) ret.limits_in = JsonResource.Value<float>("in");
			if (JsonResource.ContainsKey("out")) ret.limits_out = JsonResource.Value<float>("out");

			if (JsonResource.ContainsKey("enabled") && JsonResource.Value<bool>("enabled") == false)
				ret.enabled = false;

			return (ret, new () { ret });
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new System.NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_AVA_EyeRotation_Bone_Module
	{
		static Register_AVA_EyeRotation_Bone_Module()
		{
			STF_Module_Registry.RegisterModule(new AVA_EyeRotation_Bone_Module());
		}
	}
#endif
}