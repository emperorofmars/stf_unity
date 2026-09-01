using System.Collections.Generic;
using UnityEngine;
using com.squirrelbite.stf_unity.processors.stf_material;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.squirrelbite.stf_unity.resources.stf_material
{
	public class STF_PropertyConverter_Material_VRCToonStandard : ISTF_PropertyConverter
	{
		private readonly Material Mat;
		public STF_PropertyConverter_Material_VRCToonStandard(Material Mat)
		{
			this.Mat = Mat;
		}

		public ImportPropertyPathPart ConvertPropertyPath(ISTF_Resource Resource, List<string> STFPath)
		{
			if(STFPath.Count <= 2 || !int.TryParse(STFPath[1], out int propertyIndex))
				return null;

			if(STFPath[0] == "albedo.color" && propertyIndex == 0 && STFPath[2] == "color")
			{
				return new ImportPropertyPathPart(new List<string>() { "material._Color.r", "material._Color.g", "material._Color.b", "material._Color.a" });
			}

			// TODO decal alpha blend, etc...

			return null;
		}
	}

	public class VRCToonStandardConverter : IMaterialConverter
	{
		public const string _ShaderName = "VRChat/Mobile/Toon Standard";
		public string ShaderName => _ShaderName;

		public (Material ConvertedMaterial, List<Object> GeneratedObjects) ConvertToUnityMaterial(STF_Material STFMaterial)
		{
			var shader = Shader.Find(ShaderName);
			var ret = new Material(Shader.Find(ShaderName));
			ret.name = STFMaterial.STF_Name;

			STFMaterial.PropertyConverter = new STF_PropertyConverter_Material_VRCToonStandard(ret);

			var generatedObjects = new List<Object>();

			MaterialConverterUtil.SetTextureProperty(STFMaterial, ret, "albedo.texture", 0, "_MainTex");
			MaterialConverterUtil.SetColorProperty(STFMaterial, ret, "albedo.color", 0, "_Color");

			if(MaterialConverterUtil.SetTextureProperty(STFMaterial, ret, "normal.texture", 0, "_BumpMap"))
				ret.SetKeyword(new UnityEngine.Rendering.LocalKeyword(shader, "USE_NORMAL_MAPS"), true);

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
					var tex = MaterialConverterUtil.CreateTextureFromChannels(imageChannels, ret.name + "_MetallicGlossMap", generatedObjects);
					if(tex)
					{
						ret.SetTexture("_MetallicMap", tex);
						ret.SetTexture("_GlossMap", tex);
						ret.SetFloat("_GlossStrength", 1);
						ret.SetFloat("_MetallicStrength", 1);
						ret.SetKeyword(new UnityEngine.Rendering.LocalKeyword(shader, "USE_SPECULAR"), true);
					}
				}
			}

			if(MaterialConverterUtil.SetFloatProperty(STFMaterial, ret, "specular.value", 0, "_GlossStrength"))
				ret.SetKeyword(new UnityEngine.Rendering.LocalKeyword(shader, "USE_SPECULAR"), true);
			if(MaterialConverterUtil.SetFloatProperty(STFMaterial, ret, "metallic.value", 0, "_MetallicStrength"))
				ret.SetKeyword(new UnityEngine.Rendering.LocalKeyword(shader, "USE_SPECULAR"), true);

			return (ret, generatedObjects);
		}
	}


#if UNITY_EDITOR
	[InitializeOnLoad]
	class Register_VRCToonStandard
	{
		static Register_VRCToonStandard()
		{
			if (Shader.Find(VRCToonStandardConverter._ShaderName))
			{
				STF_Material_Converter_Registry.RegisterConverter(new VRCToonStandardConverter());
			}
		}
	}
#endif
}
