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

			// todo handle shader targets & style hints

			var materialMapping = Context.ImportConfig.GetAndConfirmImportOption(STF_Material.STF_TYPE, STFMaterial.STF_Id, STFMaterial.STF_Name, "target_shader", STF_Material_Converter_Registry.DefaultShader);

			var (ConvertedMaterial, GeneratedObjects) = STF_Material_Converter_Registry.Converters[materialMapping].ConvertToUnityMaterial(STFMaterial);

			var ret = new List<Object>() { ConvertedMaterial };
			if (GeneratedObjects != null) ret.AddRange(GeneratedObjects);
			return (ret, ret);
		}
	}
}
