using System.Collections.Generic;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules.stf_material
{
	public interface IMaterialConverter
	{
		string ShaderName {get;}

		(string RelativePath, System.Type Type, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(List<string> STFPath);
		(Material ConvertedMaterial, List<Object> GeneratedObjects) ConvertToUnityMaterial(STF_Material STFMaterial);
	}
}
