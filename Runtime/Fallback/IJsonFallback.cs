using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity.modules
{
	public interface IJsonFallback
	{
		abstract string FallbackType {get;}
		abstract string FallbackJson {get;}

		abstract List<ISTF_Resource> ReferencedResources {get;}
		abstract List<STF_Buffer> ReferencedBuffers {get;}
	}

	public interface IJsonFallback_Module
	{
		public ISTF_Resource Import(ImportContext Context, JObject Json, string STF_Id, ISTF_Resource ContextObject);

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject);
	}
}
