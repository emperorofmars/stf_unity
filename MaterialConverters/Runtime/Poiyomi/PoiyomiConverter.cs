using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules.stf_material.util;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace com.squirrelbite.stf_unity.modules.stf_material
{
	public class PoiyomiConverter : IMaterialConverter
	{
		public const string _ShaderName = ".poiyomi/Poiyomi Toon";
		public string ShaderName => _ShaderName;

		public (string RelativePath, System.Type Type, List<string> PropertyNames, System.Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(List<string> STFPath)
		{
			throw new System.NotImplementedException();
		}

		public (Material ConvertedMaterial, List<Object> GeneratedObjects) ConvertToUnityMaterial(STF_Material STFMaterial)
		{
			var ret = new Material(Shader.Find(ShaderName));
			ret.name = STFMaterial.STF_Name;

			var generatedObjects = new List<Object>();

			MaterialConverterUtil.SetTextureProperty(STFMaterial, ret, "albedo.texture", 0, "_MainTex");
			MaterialConverterUtil.SetColorProperty(STFMaterial, ret, "albedo.color", 0, "_Color");

			MaterialConverterUtil.SetTextureProperty(STFMaterial, ret, "normal.texture", 0, "_BumpMap");

			{
				var metallicValue = MaterialConverterUtil.FindPropertyValue(STFMaterial, "metallic.texture");
				var smoothnessValue = MaterialConverterUtil.FindPropertyValue(STFMaterial, "smoothness.texture");
				var roughnessValue = MaterialConverterUtil.FindPropertyValue(STFMaterial, "roughness.texture");
				var reflectionValue = MaterialConverterUtil.FindPropertyValue(STFMaterial, "reflection.texture");
				var specularValue = MaterialConverterUtil.FindPropertyValue(STFMaterial, "specular.texture");

				if (metallicValue != null || smoothnessValue != null || roughnessValue != null || reflectionValue != null || specularValue != null)
				{
					var channelMetallic = metallicValue != null ? new ImageChannelSetup.ImageChannel(metallicValue, false) : ImageChannelSetup.ImageChannel.Empty();

					var channelSmoothness = ImageChannelSetup.ImageChannel.Empty();
					if (smoothnessValue != null) channelSmoothness = new ImageChannelSetup.ImageChannel(smoothnessValue, false);
					else if (roughnessValue != null) channelSmoothness = new ImageChannelSetup.ImageChannel(roughnessValue, true);

					var channelReflection = ImageChannelSetup.ImageChannel.Empty();
					if (reflectionValue != null) channelReflection = new ImageChannelSetup.ImageChannel(reflectionValue, false);
					else if (metallicValue != null) channelReflection = new ImageChannelSetup.ImageChannel(metallicValue, false);

					var channelSpecular = ImageChannelSetup.ImageChannel.Empty();
					if (specularValue != null) channelSpecular = new ImageChannelSetup.ImageChannel(specularValue, false);
					else if (smoothnessValue != null) channelSpecular = new ImageChannelSetup.ImageChannel(smoothnessValue, false);
					else if (roughnessValue != null) channelSpecular = new ImageChannelSetup.ImageChannel(roughnessValue, true);

					var imageChannels = new ImageChannelSetup(
						channelMetallic,
						channelSmoothness,
						channelReflection,
						channelSpecular
					);
					if (MaterialConverterUtil.AssembleTextureChannels(imageChannels, ret, "_MochieMetallicMaps", generatedObjects))
					{
						ret.SetFloat("_MochieBRDF", 1);
						ret.SetFloat("_MochieMetallicMultiplier", 1);
					}
				}
			}
			return (ret, generatedObjects);
		}
	}


#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_PoiyomiMaterial
	{
		static Register_PoiyomiMaterial()
		{
			if (Shader.Find(PoiyomiConverter._ShaderName))
			{
				STF_Material_Converter_Registry.RegisterConverter(new PoiyomiConverter());
			}
		}
	}
#endif
}
