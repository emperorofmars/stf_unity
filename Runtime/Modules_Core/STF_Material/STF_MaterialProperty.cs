

using System.Collections.Generic;

namespace com.squirrelbite.stf_unity.modules.stf_material
{
	[System.Serializable]
	public class STF_MaterialProperty
	{
		public string PropertyType;
		public string ValueType;

		// If Unity wasn't stupid this would just work
		// public List<IMaterialValueType> Values = new();

		public List<FloatValue> FloatValues = new();
		public List<IntValue> IntValues = new();
		public List<ColorValue> ColorValues = new();
		public List<ImageValue> ImageValues = new();
		public List<JsonFallbackValue> JsonFallbackValues = new();
	}

}
