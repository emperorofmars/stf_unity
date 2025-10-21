using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using com.squirrelbite.stf_unity.modules;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava
{
	public class AVA_FacialTracking_Blendshape : STF_DataComponentResource
	{
		public const string _STF_Type = "ava.facial_tracking.blendshape";
		public override string STF_Type => _STF_Type;
		public string ft_type;
	}

	public class AVA_FacialTracking_Blendshape_Module : ISTF_Module
	{
		public string STF_Type => AVA_FacialTracking_Blendshape._STF_Type;
		public string STF_Kind => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"facial_tracking.blendshapes"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(AVA_FacialTracking_Blendshape)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<AVA_FacialTracking_Blendshape>();
			ret.SetFromJson(JsonResource, STF_Id, "AVA Facial Tracking Blendshapes");

			if(JsonResource.ContainsKey("ft_type")) ret.ft_type = JsonResource.Value<string>("ft_type");

			return (ret, new() { ret });
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new System.NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_AVA_FacialTracking_Blendshape_Module
	{
		static Register_AVA_FacialTracking_Blendshape_Module()
		{
			STF_Module_Registry.RegisterModule(new AVA_FacialTracking_Blendshape_Module());
		}
	}
#endif
}
