using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using com.squirrelbite.stf_unity.resources;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava
{
	[AddComponentMenu("STF/Resources/ava/ava.secondary_motion")]
	[HelpURL("https://docs.stfform.at/resources/ava/ava_secondary_motion.html")]
	public class AVA_SecondaryMotion : STF_NodeComponentResource
	{
		public const string _STF_Type = "ava.secondary_motion";
		public override string STF_Type => _STF_Type;

		public float intensity = 0.3f;
	}

	public class AVA_SecondaryMotion_Handler : ISTF_Handler
	{
		public string STF_Type => AVA_SecondaryMotion._STF_Type;
		public string STF_Category => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"secondary_motion"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(AVA_SecondaryMotion)};
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
			throw new System.NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_AVA_SecondaryMotion_Handler
	{
		static Register_AVA_SecondaryMotion_Handler()
		{
			STF_Handler_Registry.RegisterHandler(new AVA_SecondaryMotion_Handler());
		}
	}
#endif
}
