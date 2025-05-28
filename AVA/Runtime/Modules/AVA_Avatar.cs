using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace com.squirrelbite.stf_unity.ava
{
	public class AVA_Avatar : STF_NodeComponentResource
	{
		public const string _STF_Type = "ava.avatar";
		public override string STF_Type => _STF_Type;

		public GameObject Viewport;

		public override (string RelativePath, Type Type, List<string> PropertyNames, Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(List<string> STFPath)
		{
			throw new NotImplementedException();
		}

		public override List<string> ConvertPropertyPath(string UnityPath)
		{
			throw new NotImplementedException();
		}
	}

	public class AVA_Avatar_Module : ISTF_Module
	{
		public string STF_Type => AVA_Avatar._STF_Type;

		public string STF_Kind => "component";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"avatar"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(AVA_Avatar)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<AVA_Avatar>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "AVA Avatar");

			if (JsonResource.ContainsKey("viewport") && Context.ImportResource((string)JsonResource["viewport"], "node") is STF_MonoBehaviour viewport && viewport != null)
				ret.Viewport = viewport.gameObject;

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_PoiyomiMaterial
	{
		static Register_PoiyomiMaterial()
		{
			STF_Module_Registry.RegisterModule(new AVA_Avatar_Module());
		}
	}
#endif
}