
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity.modules
{
	public interface IJsonFallback_Module
	{
		public string STF_Type {get;}

		public string STF_Kind {get;}

		public object Import(ImportContext Context, JObject Json, string ID, object ParentApplicationObject);

		public (JObject Json, string ID) Export(ExportContext Context, object ApplicationObject, object ParentApplicationObject);
	}
}
