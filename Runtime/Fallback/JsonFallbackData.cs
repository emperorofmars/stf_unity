
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class JsonFallbackRoot : ScriptableObject
	{
		public string Json;
		public List<Object> ReferencedResources = new();
		public List<STF_Buffer> ReferencedBuffers = new();
	}

	public class JsonFallbackRoot_Module : IJsonFallback_Module
	{
		public string _STF_Type = "";
		public string STF_Type => _STF_Type;

		public string STF_Kind => "data";

		public (object ApplicationObject, IImportContext Context) Import(IImportContext Context, JObject Json, string ID, object ParentApplicationObject)
		{
			var ret = ScriptableObject.CreateInstance<JsonFallbackRoot>();
			ret.name = (string)Json.GetValue("name") ?? "STF Fallback";
			return (ret, Context);
		}

		public (JObject Json, string ID, IExportContext Context) Export(IExportContext Context, object ApplicationObject, object ParentApplicationObject)
		{
			var FallbackObject = ApplicationObject as JsonFallbackRoot;
			return (JObject.Parse(FallbackObject.Json), "", Context);
		}
	}
}
