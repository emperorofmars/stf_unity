using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.stfexp
{
	public class STFEXP_Humanoid_Armature : STF_NodeComponentResource
	{
		public const string _STF_Type = "stfexp.armature.humanoid";
		public override string STF_Type => _STF_Type;

		public string locomotion_type = "planti";
		public bool no_jaw = false;

		public override (string RelativePath, Type Type, List<string> PropertyNames, Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(List<string> STFPath)
		{
			throw new NotImplementedException();
		}

		public override List<string> ConvertPropertyPath(string UnityPath)
		{
			throw new NotImplementedException();
		}
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

			ret.locomotion_type = JsonResource.Value<string>("locomotion_type");
			ret.no_jaw = JsonResource.Value<bool>("no_jaw");

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