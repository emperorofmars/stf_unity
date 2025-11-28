using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules.stf_material.util;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules.stf_material
{
	public class STF_PropertyConverter_Material_UniversalRenderPipeline_Lit : ISTF_PropertyConverter
	{
		public ImportPropertyPathPart ConvertPropertyPath(ISTF_Resource Resource, List<string> STFPath)
		{
			return null;
		}
	}

	public class UniversalRenderPipeline_Lit : IMaterialConverter
	{
		public string ShaderName => "Universal Render Pipeline/Lit";

		public (Material ConvertedMaterial, List<UnityEngine.Object> GeneratedObjects) ConvertToUnityMaterial(STF_Material STFMaterial)
		{
			var ret = new Material(Shader.Find(ShaderName));
			ret.name = STFMaterial.STF_Name;

			STFMaterial.PropertyConverter = new STF_PropertyConverter_Material_UniversalRenderPipeline_Lit();

			var generatedObjects = new List<UnityEngine.Object>();


			MaterialConverterUtil.SetTextureProperty(STFMaterial, ret, "albedo.texture", 0, "_BaseMap");
			MaterialConverterUtil.SetColorProperty(STFMaterial, ret, "albedo.color", 0, "_BaseColor");

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
					if(!MaterialConverterUtil.SetFloatProperty(STFMaterial, ret, "metallic.value", 0, "_Metallic"))
						ret.SetFloat("_Metallic", 1);
					if(!MaterialConverterUtil.SetFloatProperty(STFMaterial, ret, "smoothness.value", 0, "_Smoothness"))
						if(MaterialConverterUtil.FindPropertyValue(STFMaterial, "roughness.value", 0, "float") is FloatValue roughness_value && roughness_value != null)
							ret.SetFloat("_Smoothness", 1 - roughness_value.Value);
						else
							ret.SetFloat("_Smoothness", 1);
				}
			}

			MaterialConverterUtil.SetFloatProperty(STFMaterial, ret, "specular.value", 0, "_SpecularHighlights");
			return (ret, generatedObjects);
		}
	}
}
