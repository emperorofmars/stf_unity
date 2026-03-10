using System.Collections.Generic;
using com.squirrelbite.stf_unity.resources;
using Newtonsoft.Json.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava.vrchat.modules
{
	[AddComponentMenu("STF/Resources/com.vrchat/com.vrchat.avatar_colliders")]
	public class VRC_AvatarColliders : STF_NodeComponentResource
	{
		public const string _STF_Type = "com.vrchat.avatar_colliders";
		public override string STF_Type => _STF_Type;

		[TextArea(1, 5)]
		public string Json;
	}

	public class VRC_AvatarColliders_Module : ISTF_Handler
	{
		public string STF_Type => VRC_AvatarColliders._STF_Type;
		public string STF_Category => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"collider"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(VRC_AvatarColliders)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<VRC_AvatarColliders>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "VRC AvatarColliders");
			if (JsonResource.ContainsKey("values"))
				ret.Json = JsonResource["values"].ToString();

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
	class Register_VRC_AvatarColliders_Module
	{
		static Register_VRC_AvatarColliders_Module()
		{
			STF_Handler_Registry.RegisterHandler(new VRC_AvatarColliders_Module());
		}
	}
#endif
}
