
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity
{
	public interface IImportContext
	{
		ImportState ImportState {get;}
		object ImportResource(string ID, object ParentApplicationObject);
		JObject GetJsonResource(string ID);
	}
}
