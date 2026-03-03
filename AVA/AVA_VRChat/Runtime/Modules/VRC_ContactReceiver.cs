using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using Newtonsoft.Json.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava.vrchat.modules
{
	[AddComponentMenu("STF/Modules/com.vrchat/com.vrchat.contact_receiver")]
	public class VRC_ContactReceiver : VRC_ContactBase
	{
		public const string _STF_Type = "com.vrchat.contact_receiver";
		public override string STF_Type => _STF_Type;

		public string receiver_type = "constant";
		public string parameter;
	}

	public class VRC_ContactReceiver_Module : ISTF_Module
	{
		public string STF_Type => VRC_ContactReceiver._STF_Type;
		public string STF_Kind => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(VRC_ContactReceiver)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<VRC_ContactReceiver>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "VRC Contact Receiver");
			ret.ParseBase(JsonResource);

			if(JsonResource.ContainsKey("receiver_type")) ret.receiver_type = JsonResource.Value<string>("receiver_type");
			if(JsonResource.ContainsKey("shape")) ret.parameter = JsonResource.Value<string>("parameter");

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new System.NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_VRC_ContactReceiver_Module
	{
		static Register_VRC_ContactReceiver_Module()
		{
			STF_Module_Registry.RegisterModule(new VRC_ContactReceiver_Module());
		}
	}
#endif
}
