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

		public List<ISTF_Resource> _ReferencedResources = new();
		public List<ISTF_Resource> ReferencedResources => _ReferencedResources;

		public List<STF_Buffer> _ReferencedBuffers = new();
		public List<STF_Buffer> ReferencedBuffers => _ReferencedBuffers;
	}

	public class STF_Node_Fallback_Module : IJsonFallback_Module
	{

		public ISTF_Resource Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = new GameObject((string)JsonResource.GetValue("name") ?? "STF Node");
			var ret = go.AddComponent<STF_Node_Fallback>();
			ret.SetFromJson(JsonResource, STF_Id);
			ret._FallbackType = (string)JsonResource.GetValue("type");
			ret._FallbackJson = JsonResource.ToString();


			TRSUtil.ParseTRS(ret.transform, JsonResource);

			if(JsonResource.ContainsKey("children")) foreach(var childID in (JArray)JsonResource["children"])
			{
				if(Context.ImportResource((string)childID) is STF_NodeResource childObject)
				{
					childObject.transform.SetParent(ret.transform);
				}
			}

			return ret;
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var FallbackObject = ApplicationObject as STF_Node_Fallback;
			// export buffers and resources
			return (JObject.Parse(FallbackObject.FallbackJson), FallbackObject.STF_Type);
		}
	}
}
