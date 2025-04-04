
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity
{
	public interface ISTF_Resource
	{
		abstract string STF_Type {get;}
		abstract string STF_Kind {get;}

		string STF_Id {get; set;}
		string STF_Name {get; set;}

		bool Degraded {get; set;}
	}
}
