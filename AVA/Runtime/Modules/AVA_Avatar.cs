using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using com.squirrelbite.stf_unity.modules;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava
{
	public class AVA_Avatar : STF_NodeComponentResource
	{
		public const string _STF_Type = "ava.avatar";
		public override string STF_Type => _STF_Type;

		public GameObject Viewport;
		public STF_Instance_Armature PrimaryArmatureInstance;
		public STF_Instance_Mesh PrimaryMeshInstance;
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
			if (JsonResource.ContainsKey("primary_armature_instance"))
				Context.AddTask(new Task(() => {
					if (Context.ImportResource((string)JsonResource["primary_armature_instance"], "node") is STF_Node primary_armature_instance && primary_armature_instance != null)
						ret.PrimaryArmatureInstance = primary_armature_instance.Instance as STF_Instance_Armature;
				}));
			if (JsonResource.ContainsKey("primary_mesh_instance"))
				Context.AddTask(new Task(() => {
					if (Context.ImportResource((string)JsonResource["primary_mesh_instance"], "node") is STF_Node primary_mesh_instance && primary_mesh_instance != null)
						ret.PrimaryMeshInstance = primary_mesh_instance.Instance as STF_Instance_Mesh;
				}));

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
	class Register_AVA_Avatar_Module
	{
		static Register_AVA_Avatar_Module()
		{
			STF_Module_Registry.RegisterModule(new AVA_Avatar_Module());
		}
	}
#endif
}