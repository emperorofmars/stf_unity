using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stf_material;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class STF_Instance_Mesh_PropertyConverter : ISTF_PropertyConverter
	{
		public (string RelativePath, System.Type Type, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(ISTF_Resource Resource, List<string> STFPath)
		{
			var convert = new System.Func<List<float>, List<float>>(Values =>
			{
				Values[0] *= 100;
				return Values;
			});

			if (STFPath.Count == 3 && STFPath[0] == "blendshape" && STFPath[2] == "value")
			{
				return ("", typeof(SkinnedMeshRenderer), new() { "blendShape." + STFPath[1] }, convert);
			}
			else return ("", null, null, null);
		}
	}

	public class STF_Material_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STF_Material);
		public uint Order => 10;
		public int Priority => 1;

		public (List<Object>, List<Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var STFMaterial = STFResource as STF_Material;
			var materialMapping = "Standard";

			if (Context.ImportConfig.MaterialMappings.Find(m => m.ID == STFMaterial.STF_Id) is var mapping && mapping != null && !string.IsNullOrWhiteSpace(mapping.TargetShader) && STF_Material_Converter_Registry.Converters.ContainsKey(mapping.TargetShader))
			{
				materialMapping = mapping.TargetShader;
			}
			else
			{
				Context.ImportConfig.MaterialMappings.Add(new()
				{
					ID = STFMaterial.STF_Id,
					MaterialName = STFMaterial.STF_Name,
					TargetShader = "Standard",
				});
			}
			STFResource.PropertyConverter = new STF_Instance_Mesh_PropertyConverter();

			var (ConvertedMaterial, GeneratedObjects) = STF_Material_Converter_Registry.Converters[materialMapping].ConvertToUnityMaterial(STFMaterial);

			var ret = new List<Object>() { ConvertedMaterial };
			if (GeneratedObjects != null) ret.AddRange(GeneratedObjects);
			return (ret, ret);
		}
	}
}
