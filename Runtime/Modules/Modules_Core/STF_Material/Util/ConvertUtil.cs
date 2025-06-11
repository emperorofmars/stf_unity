using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules.stf_material.util;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules.stf_material
{
	public static class MaterialConverterUtil
	{
		public static bool HasProperty(STF_Material STFMaterial, string PropertyType)
		{
			return STFMaterial.Properties.Find(p => p.PropertyType == PropertyType) != null;
		}

		public static List<IMaterialValueType> FindPropertyValues(STF_Material STFMaterial, string PropertyType, string ValueType = null)
		{
			return STFMaterial.Properties.Find(p => p.PropertyType == PropertyType && (ValueType == null || p.ValueType == ValueType))?.Values;
		}

		public static IMaterialValueType FindPropertyValue(STF_Material STFMaterial, string PropertyType, int Index = 0, string ValueType = null)
		{
			var values = FindPropertyValues(STFMaterial, PropertyType, ValueType);
			if(values != null && values.Count > Index)
				return values[Index];
			else
				return null;
		}

		public static bool SetTextureProperty(STF_Material STFMaterial, Material UnityMaterial, string PropertyType, int Index, string UnityPropertyName)
		{
			var value = FindPropertyValue(STFMaterial, PropertyType, Index, "image");
			if(value != null && ((ImageValue)value).Image != null && STFUtil.GetProcessed<Texture2D>(((ImageValue)value).Image) != null)
			{
				UnityMaterial.SetTexture(UnityPropertyName, STFUtil.GetProcessed<Texture2D>(((ImageValue)value).Image));
				return true;
			}
			return false;
		}
		public static bool SetColorProperty(STF_Material STFMaterial, Material UnityMaterial, string PropertyType, int Index, string UnityPropertyName)
		{
			var value = FindPropertyValue(STFMaterial, PropertyType, Index, "color");
			if(value != null)
			{
				UnityMaterial.SetColor(UnityPropertyName, ((ColorValue)value).Value);
				return true;
			}
			return false;
		}
		public static bool SetFloatProperty(STF_Material STFMaterial, Material UnityMaterial, string PropertyType, int Index, string UnityPropertyName)
		{
			var value = FindPropertyValue(STFMaterial, PropertyType, Index, "float");
			if(value != null)
			{
				UnityMaterial.SetFloat(UnityPropertyName, ((FloatValue)value).Value);
				return true;
			}
			return false;
		}
		public static bool SetIntProperty(STF_Material STFMaterial, Material UnityMaterial, string PropertyType, int Index, string UnityPropertyName)
		{
			var value = FindPropertyValue(STFMaterial, PropertyType, Index, "int");
			if(value != null)
			{
				UnityMaterial.SetInteger(UnityPropertyName, ((IntValue)value).Value);
				return true;
			}
			return false;
		}

		public static bool AssembleTextureChannels(ImageChannelSetup Channels, Material UnityMaterial, string UnityPropertyName, List<UnityEngine.Object> GeneratedObjects)
		{
			// check if these are texture channel property values pointing to the same texture
			/*var isSameTexture = true;
			Texture2D originalTexture = null;
			for(int i = 0; i < 4; i++)
			{
				if(Channels[i].Source == null) { isSameTexture = false; break; }
				else if(Channels[i].Source != TextureChannelPropertyValue._TYPE) { isSameTexture = false; break; }
				else if(originalTexture == null) originalTexture = ((TextureChannelPropertyValue)Channels[i].Source).Texture;
				else if(originalTexture != ((TextureChannelPropertyValue)Channels[i].Source).Texture) { isSameTexture = false; break; }
			}
			if(isSameTexture)
			{
				UnityMaterial.SetTexture(UnityPropertyName, originalTexture);
				return true;
			}
			else
			{*/
				var finalTexture = ImageUtil.AssembleTextureChannels(Channels);
				finalTexture.name = UnityMaterial.name + "_" + UnityPropertyName;
				GeneratedObjects.Add(finalTexture);
				UnityMaterial.SetTexture(UnityPropertyName, finalTexture);
				return true;
			//}
		}
	}
}
