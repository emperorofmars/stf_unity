using System.Collections.Generic;
using com.squirrelbite.stf_unity.resources;
using com.squirrelbite.stf_unity.resources.stf_material;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors.stf_material
{
	public class STF_PropertyConverter_Material_Standard : ISTF_PropertyConverter
	{
		public ImportPropertyPathPart ConvertPropertyPath(ISTF_Resource Resource, List<string> STFPath)
		{
			if(STFPath.Count <= 2 || !int.TryParse(STFPath[1], out int propertyIndex))
				return null;

			if(STFPath[0] == "albedo.color" && propertyIndex == 0 && STFPath[2] == "color")
			{
				return new ImportPropertyPathPart(new List<string>() { "material._Color.r", "material._Color.g", "material._Color.b", "material._Color.a" });
			}

			return null;
		}
	}

	public class StandardConverter : IMaterialConverter
	{
		public string ShaderName => "Standard";

		public (Material ConvertedMaterial, List<UnityEngine.Object> GeneratedObjects) ConvertToUnityMaterial(STF_Material STFMaterial)
		{
			var ret = new Material(Shader.Find(ShaderName));
			ret.name = STFMaterial.STF_Name;

			STFMaterial.PropertyConverter = new STF_PropertyConverter_Material_Standard();

			var generatedObjects = new List<UnityEngine.Object>();

			MaterialConverterUtil.SetTextureProperty(STFMaterial, ret, "albedo.texture", 0, "_MainTex");
			MaterialConverterUtil.SetColorProperty(STFMaterial, ret, "albedo.color", 0, "_Color");

			MaterialConverterUtil.SetTextureProperty(STFMaterial, ret, "normal.texture", 0, "_BumpMap");

			{
				var metallicValue = MaterialConverterUtil.FindPropertyValue(STFMaterial, "metallic.texture");
				var smoothnessValue = MaterialConverterUtil.FindPropertyValue(STFMaterial, "smoothness.texture");
				var roughnessValue = MaterialConverterUtil.FindPropertyValue(STFMaterial, "roughness.texture");

				var channelMetallic = metallicValue != null ? new ImageChannelSetup.ImageChannel(metallicValue, false) : ImageChannelSetup.ImageChannel.Empty();

				var channelSmoothness = ImageChannelSetup.ImageChannel.Empty();
				if(smoothnessValue != null) channelSmoothness = new ImageChannelSetup.ImageChannel(smoothnessValue, false);
				else if(roughnessValue != null) channelSmoothness = new ImageChannelSetup.ImageChannel(roughnessValue, true);

				if(channelMetallic.Source != null || channelSmoothness.Source != null)
				{
					var imageChannels = new ImageChannelSetup(
						channelMetallic,
						ImageChannelSetup.ImageChannel.Empty(),
						ImageChannelSetup.ImageChannel.Empty(),
						channelSmoothness
					);
					MaterialConverterUtil.AssembleTextureChannels(imageChannels, ret, "_MetallicGlossMap", generatedObjects);
				}
			}

			MaterialConverterUtil.SetFloatProperty(STFMaterial, ret, "specular.value", 0, "_SpecularHighlights");
			return (ret, generatedObjects);
		}
	}
}
