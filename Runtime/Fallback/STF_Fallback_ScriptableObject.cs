using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Fallback_ScriptableObject : STF_ScriptableObject, IJsonFallback
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

		public override string STF_Kind => "fallback";

		public override (string RelativePath, System.Type Type, string PropertyName) ConvertPropertyPath(List<string> STFPath)
		{
			throw new System.NotImplementedException();
		}

		public override List<string> ConvertPropertyPath(string UnityPath)
		{
			throw new System.NotImplementedException();
		}
	}

	public static class STF_Fallback_ScriptableObject_Module// : IJsonFallback_Module
	{

		public static ISTF_Resource Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<STF_Fallback_ScriptableObject>();
			ret.SetFromJson(JsonResource, STF_Id);
			ret._FallbackType = (string)JsonResource.GetValue("type");
			ret._FallbackJson = JsonResource.ToString();

			// TODO referenced resources and buffers

			return ret;
		}

		public static (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var FallbackObject = ApplicationObject as STF_Fallback_ScriptableObject;
			// TODO export buffers and resources
			return (JObject.Parse(FallbackObject.FallbackJson), FallbackObject.STF_Type);
		}
	}
}
