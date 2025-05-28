using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stf_material;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class STF_Material_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STF_Material);
		public uint Order => 10;
		public int Priority => 1;

		public List<Object> Process(ProcessorContext Context, ISTF_Resource STFResource)
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

			var (ConvertedMaterial, GeneratedObjects) = STF_Material_Converter_Registry.Converters[materialMapping].ConvertToUnityMaterial(STFMaterial);

			var ret = new List<Object>() { ConvertedMaterial };
			if (GeneratedObjects != null) ret.AddRange(GeneratedObjects);
			return ret;
		}
	}
}