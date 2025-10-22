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

		public (List<Object>, List<Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var STFMaterial = STFResource as STF_Material;
			var materialMapping = STF_Material_Converter_Registry.DefaultShader;

			// todo handle shader targets & style hints

			var importOptions = Context.ImportConfig.GetResourceImportOptions(STF_Material.STF_TYPE, STFMaterial.STF_Id);
			if(importOptions.ContainsKey("target_shader") && importOptions.Value<string>("target_shader") is string targetShader && STF_Material_Converter_Registry.Converters.ContainsKey(targetShader))
			{
				materialMapping = targetShader;
			}
			else
			{
				importOptions["target_shader"] = STF_Material_Converter_Registry.DefaultShader;
			}
			Context.ImportConfig.ConfirmResourceImportOptions(STF_Material.STF_TYPE, STFMaterial.STF_Id, importOptions);


			/*if (Context.ImportConfig.MaterialMappings.Find(m => m.ID == STFMaterial.STF_Id) is var mapping && mapping != null && !string.IsNullOrWhiteSpace(mapping.TargetShader) && STF_Material_Converter_Registry.Converters.ContainsKey(mapping.TargetShader))
			{
				materialMapping = mapping.TargetShader;
			}
			else
			{
				Context.ImportConfig.MaterialMappings.Add(new()
				{
					ID = STFMaterial.STF_Id,
					MaterialName = STFMaterial.STF_Name,
					TargetShader = materialMapping,
				});
			}*/

			var (ConvertedMaterial, GeneratedObjects) = STF_Material_Converter_Registry.Converters[materialMapping].ConvertToUnityMaterial(STFMaterial);

			var ret = new List<Object>() { ConvertedMaterial };
			if (GeneratedObjects != null) ret.AddRange(GeneratedObjects);
			return (ret, ret);
		}
	}
}
