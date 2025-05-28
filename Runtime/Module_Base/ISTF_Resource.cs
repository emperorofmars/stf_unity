
using System.Collections.Generic;

namespace com.squirrelbite.stf_unity
{
	public interface ISTF_Resource
	{
		abstract string STF_Type {get;}
		abstract string STF_Kind {get;}

		string STF_Id {get; set;}
		string STF_Name {get; set;}

		bool Degraded {get; set;}

		abstract List<object> ProcessedObjects {get;}


		(string RelativePath, System.Type Type, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(List<string> STFPath);
		List<string> ConvertPropertyPath(string UnityPath); // TODO improve this
	}
}
