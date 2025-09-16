using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using com.squirrelbite.stf_unity.modules;
using System.Linq;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.ava
{
	public class AVA_Emotes : STF_NodeComponentResource
	{
		public const string _STF_Type = "ava.emotes";
		public override string STF_Type => _STF_Type;

		[System.Serializable]
		public class Emote
		{
			public string meaning;
			public STF_Animation animation;
			public VRM_BlendshapePose fallback;
		}

		public List<Emote> emotes = new();
	}

	public class AVA_Emotes_Module : ISTF_Module
	{
		public string STF_Type => AVA_Emotes._STF_Type;

		public string STF_Kind => "component";

		public int Priority => 1;

		public List<string> LikeTypes => new(){"emotes"};

		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(AVA_Emotes)};

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<AVA_Emotes>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "AVA Emotes");

			if (JsonResource.ContainsKey("emotes"))
			{
				foreach ((var meaning, var jsonEmote) in JsonResource["emotes"] as JObject)
				{
					if (jsonEmote is JObject jsonObjectEmote && jsonObjectEmote.ContainsKey("animation"))
					{
						var emote = new AVA_Emotes.Emote() { meaning = meaning, animation = Context.ImportResource((string)jsonObjectEmote["animation"], "data") as STF_Animation };
						if (jsonObjectEmote.ContainsKey("fallback") && jsonObjectEmote["fallback"].Type == JTokenType.String)
							emote.fallback = Context.ImportResource(jsonObjectEmote.Value<string>("fallback"), "data") as VRM_BlendshapePose;
						ret.emotes.Add(emote);
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
	class Register_AVA_Emotes_Module
	{
		static Register_AVA_Emotes_Module()
		{
			STF_Module_Registry.RegisterModule(new AVA_Emotes_Module());
		}
	}
#endif
}