using System.Collections.Generic;

namespace com.squirrelbite.stf_unity.modules
{
	// Converts animation paths from STF to Unity's animation system
	public interface ISTF_PropertyConverter
	{
		// RelativePath: path from the parent resource, 
		// Type: the animated properties type
		// PropertyNames: i.e. a translation property becomes { "localPosition.x", "localPosition.y", "localPosition.z" }
		// ConvertValueFunc: a callback to convert values, i.e. to invert the X axis of the translation.
		(string RelativePath, System.Type Type, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(ISTF_Resource Resource, List<string> STFPath);
	}
}
