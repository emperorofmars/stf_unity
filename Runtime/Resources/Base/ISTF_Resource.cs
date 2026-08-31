using System.Collections.Generic;

namespace com.squirrelbite.stf_unity.resources
{
	/// <summary>
	/// Resources get imported & exported by ISTF_Handler.
	///
	/// This resource directly represents the STF resources data, and doesn't have to be understandable by Unity.
	///
	/// Each ISTF_Handler implementation has one corresponding ISTF_Resource implementation.
	/// </summary>
	public interface ISTF_Resource
	{
		abstract string STF_Type {get;}
		abstract string STF_Category {get;}

		string STF_Id {get; set;}
		string STF_Name {get; set;}

		bool Degraded {get; set;}

		abstract List<object> ProcessedObjects {get;}

		abstract ISTF_PropertyConverter PropertyConverter {get; set;}
	}
}
