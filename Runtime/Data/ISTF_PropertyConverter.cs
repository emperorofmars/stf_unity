
using System.Collections.Generic;

namespace com.squirrelbite.stf_unity
{
	public interface ISTF_PropertyConverter
	{
		(string RelativePath, System.Type Type, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(ISTF_Resource Resource, List<string> STFPath);
	}
}
