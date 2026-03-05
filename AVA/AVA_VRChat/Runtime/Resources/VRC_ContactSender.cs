using System.Collections.Generic;
using com.squirrelbite.stf_unity.handlers;
using Newtonsoft.Json.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava.vrchat.modules
{
	[AddComponentMenu("STF/Resources/com.vrchat/com.vrchat.contact_sender")]
	public class VRC_ContactSender : VRC_ContactBase
	{
		public const string _STF_Type = "com.vrchat.contact_sender";
		public override string STF_Type => _STF_Type;
	}

	public class VRC_ContactSender_Module : ISTF_Handler
	{
		public string STF_Type => VRC_ContactSender._STF_Type;
		public string STF_Category => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(VRC_ContactSender)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<VRC_ContactSender>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "VRC Contact Sender");
			ret.ParseBase(JsonResource);

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new System.NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_VRC_ContactSender_Module
	{
		static Register_VRC_ContactSender_Module()
		{
			STF_Handler_Registry.RegisterHandler(new VRC_ContactSender_Module());
		}
	}
#endif
}
