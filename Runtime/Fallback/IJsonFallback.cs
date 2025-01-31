
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity.modules
{
	public interface IJsonFallback_Module
	{
		public string STF_Type {get;}

		public string STF_Kind {get;}

		public (object ApplicationObject, IImportContext Context) Import(IImportContext Context, JObject Json, string ID, object ParentApplicationObject);

		public (JObject Json, string ID, IExportContext Context) Export(IExportContext Context, object ApplicationObject, object ParentApplicationObject);
	}
}
