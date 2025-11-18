using System.Collections.Generic;

namespace com.squirrelbite.stf_unity.modules
{
	/// <summary>
	/// Resources get imported & exported by ISTF_Module.
	/// Each ISTF_Module implementation has one corresponding ISTF_Resource implementation.
	/// </summary>
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
