using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public interface IJsonFallback
	{
		string FallbackType {get;}
		string FallbackJson {get;}

		List<STF_ScriptableObject> ReferencedScriptableObjectResources  {get;}
		List<STF_MonoBehaviour> ReferencedMonoBehaviourResources  {get;}
		List<STF_Buffer> ReferencedBuffers {get;}

		static void ImportReferences(ImportContext Context, JObject JsonResource, ISTF_Resource ContextObject, IJsonFallback Fallback)
		{
			if(JsonResource.ContainsKey("referenced_resources")) foreach(string resourceId in JsonResource["referenced_resources"])
			{
				var resource = Context.ImportResource(resourceId, null, ContextObject);
				if(resource is STF_ScriptableObject resourceScriptableObject)
					Fallback.ReferencedScriptableObjectResources.Add(resourceScriptableObject);
				else if(resource is STF_MonoBehaviour resourceMonoBehaviour)
					Fallback.ReferencedMonoBehaviourResources.Add(resourceMonoBehaviour);
			}
			if(JsonResource.ContainsKey("referenced_buffers")) foreach(string bufferId in JsonResource["referenced_buffers"])
			{
				Fallback.ReferencedBuffers.Add(Context.ImportBuffer(bufferId));
			}
		}
	}

	public interface IJsonFallback_Module
	{
		public ISTF_Resource Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject);

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject);
	}
}
