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
	public class AVA_VoicePosition : STF_NodeComponentResource
	{
		public const string _STF_Type = "ava.voice_position";
		public override string STF_Type => _STF_Type;

		public GameObject VoicePosition;
	}

	public class AVA_VoicePosition_Module : ISTF_Module
	{
		public string STF_Type => AVA_VoicePosition._STF_Type;
		public string STF_Kind => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"voice_position"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(AVA_VoicePosition)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<AVA_VoicePosition>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "AVA VoicePosition");

			if (JsonResource.ContainsKey("voice_position") && STFUtil.ImportResource(Context, JsonResource, JsonResource["voice_position"], "node") is STF_MonoBehaviour voice_position && voice_position != null)
				ret.VoicePosition = voice_position.gameObject;

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
	class Register_AVA_VoicePosition_Module
	{
		static Register_AVA_VoicePosition_Module()
		{
			STF_Module_Registry.RegisterModule(new AVA_VoicePosition_Module());
		}
	}
#endif
}
