using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava
{
	public class AVA_FaceMesh : STF_NodeComponentResource
	{
		public const string _STF_Type = "ava.face_mesh";
		public override string STF_Type => _STF_Type;
	}

	public class AVA_FaceMesh_Module : ISTF_Module
	{
		public string STF_Type => AVA_FaceMesh._STF_Type;

		public string STF_Kind => "component";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"visemes"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(AVA_FaceMesh)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<AVA_FaceMesh>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "AVA FaceMesh");

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_AVA_FaceMesh_Module
	{
		static Register_AVA_FaceMesh_Module()
		{
			STF_Module_Registry.RegisterModule(new AVA_FaceMesh_Module());
		}
	}
#endif
}