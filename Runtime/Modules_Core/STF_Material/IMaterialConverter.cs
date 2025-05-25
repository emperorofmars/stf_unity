using System.Collections.Generic;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules.stf_material
{
	public interface IMaterialConvertState
	{
		void SaveResource(Object Resource, string Name);
		Texture2D SaveImageResource(byte[] Bytes, string Name, string Extension);
	}

	public interface IMaterialConverter
	{
		string ShaderName {get;}

		(string RelativePath, System.Type Type, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(List<string> STFPath);
		Material ConvertToUnityMaterial(IMaterialConvertState State, STF_Material MTFMaterial, Material ExistingUnityMaterial = null);
	}
}
