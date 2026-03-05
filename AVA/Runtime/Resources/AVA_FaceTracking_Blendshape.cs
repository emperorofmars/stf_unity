using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using com.squirrelbite.stf_unity.handlers;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava
{
	[AddComponentMenu("STF/Resources/ava/ava.face_tracking.blendshape")]
	[HelpURL("https://docs.stfform.at/resources/ava/ava_face_tracking_blendshape.html")]
	public class AVA_FaceTracking_Blendshape : STF_DataComponentResource
	{
		public const string _STF_Type = "ava.face_tracking.blendshape";
		public override string STF_Type => _STF_Type;
		public string ft_type;
	}

	public class AVA_FacialTracking_Blendshape_Module : ISTF_Handler
	{
		public string STF_Type => AVA_FaceTracking_Blendshape._STF_Type;
		public string STF_Category => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"face_tracking.blendshapes", "face_tracking"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(AVA_FaceTracking_Blendshape)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<AVA_FaceTracking_Blendshape>();
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
			STF_Handler_Registry.RegisterHandler(new AVA_FacialTracking_Blendshape_Module());
		}
	}
#endif
}
