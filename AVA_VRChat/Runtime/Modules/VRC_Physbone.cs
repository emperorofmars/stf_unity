using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using Newtonsoft.Json.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava.vrchat.modules
{
	public class VRC_Physbone : STF_NodeComponentResource
	{
		public const string _STF_Type = "com.vrchat.physbone";
		public override string STF_Type => _STF_Type;

		public List<string> Colliders = new();
		public List<string> Ignores = new();
		public string Json;
	}

	public class VRC_Physbone_Module : ISTF_Module
	{
		public string STF_Type => VRC_Physbone._STF_Type;

		public string STF_Kind => "component";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"colliders"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(VRC_Physbone)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<VRC_Physbone>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "VRC Physbone");
			ret.Json = JsonResource.GetValue("values").ToString();
			if (JsonResource.ContainsKey("colliders")) ret.Colliders = JsonResource["colliders"].ToObject<List<string>>();
			if (JsonResource.ContainsKey("ignores")) ret.Ignores = JsonResource["ignores"].ToObject<List<string>>();
			
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
	class Register_VRC_Physbone_Module
	{
		static Register_VRC_Physbone_Module()
		{
			STF_Module_Registry.RegisterModule(new VRC_Physbone_Module());
		}
	}
#endif
}