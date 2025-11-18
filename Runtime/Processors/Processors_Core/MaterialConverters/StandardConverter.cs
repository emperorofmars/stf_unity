using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules.stf_material.util;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules.stf_material
{
	public class STF_PropertyConverter_Material_Standard : ISTF_PropertyConverter
	{
		public (string RelativePath, System.Type Type, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(ISTF_Resource Resource, List<string> STFPath)
		{
			if(STFPath.Count <= 2 || !int.TryParse(STFPath[1], out int propertyIndex))
				return ("", null, null, null);

			if(STFPath[0] == "albedo.color" && propertyIndex == 0 && STFPath[2] == "color")
			{
				return ("", null, new() { "material._Color.r", "material._Color.g", "material._Color.b", "material._Color.a" }, null);
			}

			return ("", null, null, null);
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

				var channelSmoothnes = ImageChannelSetup.ImageChannel.Empty();
				if(smoothnessValue != null) channelSmoothnes = new ImageChannelSetup.ImageChannel(smoothnessValue, false);
				else if(roughnessValue != null) channelSmoothnes = new ImageChannelSetup.ImageChannel(roughnessValue, true);

				if(channelMetallic.Source != null || channelSmoothnes.Source != null)
				{
					var imageChannels = new ImageChannelSetup(
						channelMetallic,
						ImageChannelSetup.ImageChannel.Empty(),
						ImageChannelSetup.ImageChannel.Empty(),
						channelSmoothnes
					);
					MaterialConverterUtil.AssembleTextureChannels(imageChannels, ret, "_MetallicGlossMap", generatedObjects);
				}
			}

			MaterialConverterUtil.SetFloatProperty(STFMaterial, ret, "specular.value", 0, "_SpecularHighlights");
			return (ret, generatedObjects);
		}
	}
}
