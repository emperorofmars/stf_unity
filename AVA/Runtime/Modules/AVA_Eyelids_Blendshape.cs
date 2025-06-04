using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using com.squirrelbite.stf_unity.modules;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava
{
	public class AVA_Eyelids_Blendshape : STF_DataComponentResource
	{
		public const string _STF_Type = "ava.eyelids.blendshape";
		public override string STF_Type => _STF_Type;

		public static readonly string[] _EyelidShapes = { "closed", "up", "down", "left", "right", "closed_left", "up_left", "down_left", "left_left", "right_left", "closed_right", "up_right", "down_right", "left_right", "right_right" };

		public List<string> Mapppings = new();
	}

	public class AVA_Eyelids_Blendshape_Module : ISTF_Module
	{
		public string STF_Type => AVA_Eyelids_Blendshape._STF_Type;

		public string STF_Kind => "component";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"eyelid.blendshapes"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(AVA_Eyelids_Blendshape)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<AVA_Eyelids_Blendshape>();
			ret.SetFromJson(JsonResource, STF_Id, "AVA Eyelid Blendshapes");

			foreach (var shape in AVA_Eyelids_Blendshape._EyelidShapes)
				if (JsonResource.ContainsKey(shape))
					ret.Mapppings.Add(JsonResource.Value<string>(shape));
				else
					ret.Mapppings.Add(null);

			return (ret, new() { ret });
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_AVA_Eyelids_Blendshape_Module
	{
		static Register_AVA_Eyelids_Blendshape_Module()
		{
			STF_Module_Registry.RegisterModule(new AVA_Eyelids_Blendshape_Module());
		}
	}
#endif
}