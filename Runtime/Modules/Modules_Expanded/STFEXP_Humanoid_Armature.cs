using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace com.squirrelbite.stf_unity.modules.stfexp
{

	public class STFEXP_Humanoid_Armature : STF_NodeComponentResource
	{
		public const string _STF_Type = "stfexp.armature.humanoid";
		public override string STF_Type => _STF_Type;
		
		public static class HumanoidLocomotionType { public const string Plantigrade = "planti"; public const string Digitigrade = "digi"; }
		public string locomotion_type = HumanoidLocomotionType.Plantigrade;
		public bool no_jaw = false;

		[Serializable]
		public class BoneMapping
		{
			public string Mapping;
			public string BoneID;
			public bool set_rotation_limits = false;
			public float p_min;
			public float p_center;
			public float p_max;
			public float s_min;
			public float s_center;
			public float s_max;
			public float t_min;
			public float t_center;
			public float t_max;
		}
		public List<BoneMapping> bone_mappings = new();

		[Serializable]
		public class HumanoidSettings
		{
			public float arm_stretch = 0.053f;
			public float upper_arm_twist = 0.5f;
			public float lower_arm_twist = 0.5f;
			public float leg_stretch = 0.05f;
			public float upper_leg_twist = 0.5f;
			public float lower_leg_twist = 0.5f;
			public float feet_spacing = 0;
			public bool use_translation = false;
		}
		public HumanoidSettings settings = new();
	}

	public class STFEXP_Humanoid_Armature_Module : ISTF_Module
	{
		public string STF_Type => STFEXP_Humanoid_Armature._STF_Type;

		public string STF_Kind => "component";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"humanoid"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STFEXP_Humanoid_Armature)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<STFEXP_Humanoid_Armature>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "STFEXP Humanoid Armature");

			if(JsonResource.ContainsKey("locomotion_type")) ret.locomotion_type = JsonResource.Value<string>("locomotion_type");
			if(JsonResource.ContainsKey("no_jaw")) ret.no_jaw = JsonResource.Value<bool>("no_jaw");

			if(JsonResource.ContainsKey("settings") && JsonResource["settings"] is JObject JsonSettings && JsonSettings != null)
			{
				if(JsonSettings.ContainsKey("arm_stretch")) ret.settings.arm_stretch = JsonSettings.Value<float>("arm_stretch");
				if(JsonSettings.ContainsKey("upper_arm_twist")) ret.settings.upper_arm_twist = JsonSettings.Value<float>("upper_arm_twist");
				if(JsonSettings.ContainsKey("lower_arm_twist")) ret.settings.lower_arm_twist = JsonSettings.Value<float>("lower_arm_twist");
				if(JsonSettings.ContainsKey("leg_stretch")) ret.settings.leg_stretch = JsonSettings.Value<float>("leg_stretch");
				if(JsonSettings.ContainsKey("upper_leg_twist")) ret.settings.upper_leg_twist = JsonSettings.Value<float>("upper_leg_twist");
				if(JsonSettings.ContainsKey("lower_leg_twist")) ret.settings.lower_leg_twist = JsonSettings.Value<float>("lower_leg_twist");
				if(JsonSettings.ContainsKey("feet_spacing")) ret.settings.feet_spacing = JsonSettings.Value<float>("feet_spacing");
				if(JsonSettings.ContainsKey("use_translation")) ret.settings.use_translation = JsonSettings.Value<bool>("use_translation");
			}

			if(JsonResource.ContainsKey("mappings") && JsonResource["mappings"] is JObject JsonMappings && JsonMappings != null)
			{
				foreach((var mapping, var targetToken) in JsonMappings)
				{
					if(targetToken is JObject JsonTarget && JsonTarget != null)
					{
						var boneMapping = new STFEXP_Humanoid_Armature.BoneMapping();
						boneMapping.Mapping = mapping;
						if(JsonTarget.ContainsKey("target")) boneMapping.BoneID = JsonTarget.Value<string>("target");
						if(JsonTarget.ContainsKey("rotation_limits") && JsonTarget["rotation_limits"] is JObject JsonLimits && JsonLimits != null)
						{
							boneMapping.set_rotation_limits = true;

							if(JsonLimits.ContainsKey("primary") && JsonLimits["primary"] is JArray pArr && pArr != null)
							{
								if(pArr[0].Type == JTokenType.Float) boneMapping.p_min = pArr[0].Value<float>();
								if(pArr[1].Type == JTokenType.Float) boneMapping.p_center = pArr[1].Value<float>();
								if(pArr[2].Type == JTokenType.Float) boneMapping.p_max = pArr[2].Value<float>();
							}
							if(JsonLimits.ContainsKey("secondary") && JsonLimits["secondary"] is JArray sArr && sArr != null)
							{
								if(sArr[0].Type == JTokenType.Float) boneMapping.s_min = sArr[0].Value<float>();
								if(sArr[1].Type == JTokenType.Float) boneMapping.s_center = sArr[1].Value<float>();
								if(sArr[2].Type == JTokenType.Float) boneMapping.s_max = sArr[2].Value<float>();
							}
							if(JsonLimits.ContainsKey("twist") && JsonLimits["twist"] is JArray tArr && tArr != null)
							{
								if(tArr[0].Type == JTokenType.Float) boneMapping.t_min = tArr[0].Value<float>();
								if(tArr[1].Type == JTokenType.Float) boneMapping.t_center = tArr[1].Value<float>();
								if(tArr[2].Type == JTokenType.Float) boneMapping.t_max = tArr[2].Value<float>();
							}
						}
						ret.bone_mappings.Add(boneMapping);
					}
				}
			}

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_STFEXP_Humanoid_Armature
	{
		static Register_STFEXP_Humanoid_Armature()
		{
			STF_Module_Registry.RegisterModule(new STFEXP_Humanoid_Armature_Module());
		}
	}
#endif
}