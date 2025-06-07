
using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules.stf_material.util;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules.stf_material
{
	public class StandardConverter : IMaterialConverter
	{
		public string ShaderName => "Standard";

		public (string RelativePath, Type Type, List<string> PropertyNames, Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(List<string> STFPath)
		{
			throw new NotImplementedException();
		}

		public (Material ConvertedMaterial, List<UnityEngine.Object> GeneratedObjects) ConvertToUnityMaterial(STF_Material STFMaterial)
		{
			var ret = new Material(Shader.Find(ShaderName));
			ret.name = STFMaterial.STF_Name;

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
					Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
					var imageChannels = new ImageChannelSetup(
						channelMetallic,
						ImageChannelSetup.ImageChannel.Empty(),
						ImageChannelSetup.ImageChannel.Empty(),
						channelSmoothnes
					);
					MaterialConverterUtil.AssembleTextureChannels(imageChannels, ret, "_MetallicGlossMap", generatedObjects);
				}
					Debug.Log("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
			}

			MaterialConverterUtil.SetFloatProperty(STFMaterial, ret, "specular.value", 0, "_SpecularHighlights");
			return (ret, generatedObjects);
		}
	}
}
