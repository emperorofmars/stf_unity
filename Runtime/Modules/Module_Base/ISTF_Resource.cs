
using System.Collections.Generic;

namespace com.squirrelbite.stf_unity.modules
{
	public interface ISTF_Resource
	{
		abstract string STF_Type {get;}
		abstract string STF_Kind {get;}

		string STF_Id {get; set;}
		string STF_Name {get; set;}

		bool Degraded {get; set;}

		abstract List<object> ProcessedObjects {get;}

		abstract ISTF_PropertyConverter PropertyConverter {get; set;}
	}
}
