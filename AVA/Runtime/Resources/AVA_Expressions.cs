using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using com.squirrelbite.stf_unity.resources;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava
{
	[AddComponentMenu("STF/Resources/ava/ava.expressions")]
	[HelpURL("https://docs.stfform.at/resources/ava/ava_expressions.html")]
	public class AVA_Expressions : STF_NodeComponentResource
	{
		public const string _STF_Type = "ava.expressions";
		public override string STF_Type => _STF_Type;

		[System.Serializable]
		public class Expression
		{
			public string meaning;
			public STF_Animation animation;
			public VRM_BlendshapePose fallback;
		}

		public List<Expression> expressions = new();
	}

	public class AVA_Expressions_Module : ISTF_Handler
	{
		public string STF_Type => AVA_Expressions._STF_Type;
		public string STF_Category => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"expressions"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(AVA_Expressions)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<AVA_Expressions>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "AVA Expressions");

			if (JsonResource.ContainsKey("expressions"))
			{
				foreach ((var meaning, var jsonExpression) in JsonResource["expressions"] as JObject)
				{
					if (jsonExpression is JObject jsonObjectEmote && jsonObjectEmote.ContainsKey("animation"))
					{
						var emote = new AVA_Expressions.Expression() { meaning = meaning, animation = Context.ImportResource(JsonResource, jsonObjectEmote["animation"], "data") as STF_Animation };
						if (jsonObjectEmote.ContainsKey("fallback") && STFUtil.GetResourceID(JsonResource, jsonObjectEmote["fallback"]) is string fallbackID && fallbackID != null)
							emote.fallback = Context.ImportResource(fallbackID, "data") as VRM_BlendshapePose;
						ret.expressions.Add(emote);
					}
				}
			}

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
	class Register_AVA_Expressions_Module
	{
		static Register_AVA_Expressions_Module()
		{
			STF_Handler_Registry.RegisterHandler(new AVA_Expressions_Module());
		}
	}
#endif
}
