using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	[AddComponentMenu("STF/Modules/Fallback/STF_Fallback_MonoBehaviour")]
	public class STF_Fallback_MonoBehaviour : STF_MonoBehaviour, IJsonFallback
	{
		public string _FallbackType;
		public string FallbackType => _FallbackType;
		public override string STF_Type => FallbackType;

		public string _FallbackJson;
		public string FallbackJson => _FallbackJson;

		public List<STF_ScriptableObject> _ReferencedScriptableObjectResources = new();
		public List<STF_ScriptableObject> ReferencedScriptableObjectResources => _ReferencedScriptableObjectResources;

		public List<STF_MonoBehaviour> _ReferencedMonoBehaviourResources = new();
		public List<STF_MonoBehaviour> ReferencedMonoBehaviourResources => _ReferencedMonoBehaviourResources;

		public List<STF_Buffer> _ReferencedBuffers = new();
		public List<STF_Buffer> ReferencedBuffers => _ReferencedBuffers;

		public override string STF_Kind => "fallback";
	}

	public static class STF_Fallback_MonoBehaviour_Module// : IJsonFallback_Module
	{

		public static ISTF_Resource Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = (STF_MonoBehaviour)ContextObject;
			var ret = go.gameObject.AddComponent<STF_Fallback_MonoBehaviour>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject);
			ret._FallbackType = (string)JsonResource.GetValue("type");
			ret._FallbackJson = JsonResource.ToString();

			Context.AddTask(new Task(() => {
				IJsonFallback.ImportReferences(Context, JsonResource, ContextObject, ret);
			}));

			/*if(JsonResource.ContainsKey("referenced_resources")) foreach(string resourceId in JsonResource["referenced_resources"])
			{
				var resource = Context.ImportResource(resourceId, null, ContextObject);
				if(resource is STF_ScriptableObject resourceScriptableObject)
					ret.ReferencedScriptableObjectResources.Add(resourceScriptableObject);
				else if(resource is STF_MonoBehaviour resourceMonoBehaviour)
					ret.ReferencedMonoBehaviourResources.Add(resourceMonoBehaviour);
			}

			if(JsonResource.ContainsKey("referenced_buffers")) foreach(string bufferId in JsonResource["referenced_buffers"])
			{
				ret.ReferencedBuffers.Add(Context.ImportBuffer(bufferId));
			}*/

			return ret;
		}

		public static (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var FallbackObject = ApplicationObject as STF_Fallback_MonoBehaviour;
			// export buffers and resources
			return (JObject.Parse(FallbackObject.FallbackJson), FallbackObject.STF_Type);
		}
	}
}
