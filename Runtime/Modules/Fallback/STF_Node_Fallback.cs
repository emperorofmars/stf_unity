using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Node_Fallback : STF_NodeResource, IJsonFallback
	{
		public string _FallbackType;
		public string FallbackType => _FallbackType;
		public override string STF_Type => FallbackType;

		public string _FallbackJson;
		public string FallbackJson => _FallbackJson;

		public List<Object> _ReferencedResources = new();
		public List<Object> ReferencedResources => _ReferencedResources;

		public List<STF_Buffer> _ReferencedBuffers = new();
		public List<STF_Buffer> ReferencedBuffers => _ReferencedBuffers;
	}

	public static class STF_Node_Fallback_Module// : IJsonFallback_Module
	{

		public static ISTF_Resource Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = new GameObject((string)JsonResource.GetValue("name") ?? "STF Node");
			var ret = go.AddComponent<STF_Node_Fallback>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject);
			ret._FallbackType = (string)JsonResource.GetValue("type");
			ret._FallbackJson = JsonResource.ToString();

			TRSUtil.ParseTRS(ret.transform, JsonResource);

			if(JsonResource.ContainsKey("children")) foreach(var childID in (JArray)JsonResource["children"])
			{
				if(Context.ImportResource((string)childID, "node") is STF_NodeResource childObject)
				{
					childObject.transform.SetParent(ret.transform);
				}
			}

			// TODO referenced resources and buffers

			return ret;
		}

		public static (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var FallbackObject = ApplicationObject as STF_Node_Fallback;
			// export buffers and resources
			return (JObject.Parse(FallbackObject.FallbackJson), FallbackObject.STF_Type);
		}
	}
}
