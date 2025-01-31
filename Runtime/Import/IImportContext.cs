
using com.squirrelbite.stf_unity.modules;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity
{
	public interface IImportContext
	{
		ImportState ImportState {get;}
		IJsonFallback_Module FallbackModule {get;}
		object ImportResource(string ID, object ParentApplicationObject);
		object HandleFallback(IImportContext Context, JObject JsonResource, string ID, object ParentApplicationObject = null);
		JObject GetJsonResource(string ID);
		void Report(STFReport Report);
	}
}
