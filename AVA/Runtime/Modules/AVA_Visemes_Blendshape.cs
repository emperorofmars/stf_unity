using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava
{
	public class AVA_Visemes_Blendshape : STF_DataComponentResource
	{
		public const string _STF_Type = "ava.visemes.blendshape";
		public override string STF_Type => _STF_Type;

		public static readonly string[] _Visemes15 = { "sil", "aa", "ch", "dd", "e", "ff", "ih", "kk", "nn", "oh", "ou", "pp", "rr", "ss", "th" };

		public List<string> Mapppings = new();
	}

	public class AVA_Visemes_Blendshape_Module : ISTF_Module
	{
		public string STF_Type => AVA_Visemes_Blendshape._STF_Type;

		public string STF_Kind => "component";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"visemes"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(AVA_Visemes_Blendshape)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<AVA_Visemes_Blendshape>();
			ret.SetFromJson(JsonResource, STF_Id, "AVA Visemes Blendshape");

			foreach (var viseme in AVA_Visemes_Blendshape._Visemes15)
				if (JsonResource.ContainsKey(viseme))
					ret.Mapppings.Add(JsonResource.Value<string>(viseme));
				else
					ret.Mapppings.Add(JsonResource.Value<string>(null));

			return (ret, new() { ret });
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_AVA_Visemes_Blendshape_Module
	{
		static Register_AVA_Visemes_Blendshape_Module()
		{
			STF_Module_Registry.RegisterModule(new AVA_Visemes_Blendshape_Module());
		}
	}
#endif
}