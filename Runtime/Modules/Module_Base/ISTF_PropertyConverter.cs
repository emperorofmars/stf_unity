using System.Collections.Generic;

namespace com.squirrelbite.stf_unity.modules
{
	/// <summary>
	/// Converts animation paths from STF to Unity's animation system.
	/// </summary>
	public interface ISTF_PropertyConverter
	{
		/// <returns>
		/// <param name="RelativePath">path from the parent resource.</param>
		/// <param name="Type">the animated properties type.</param>
		/// <param name="PropertyNames">i.e. an stf 'translation' property becomes { "localPosition.x", "localPosition.y", "localPosition.z" }</param>
		/// <param name="ConvertValueFunc">a callback to convert values, i.e. to invert the X axis of the translation.</param>
		/// <returns>
		(string RelativePath, System.Type Type, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(ISTF_Resource Resource, List<string> STFPath);
	}
}
