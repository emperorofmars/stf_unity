using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stfexp;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors.stfexp
{
	public class STF_Light_Converter : ISTF_PropertyConverter
	{
		private Light Light;
		public STF_Light_Converter(Light Light)
		{
			this.Light = Light;
		}
		public ImportPropertyPathPart ConvertPropertyPath(ISTF_Resource STFResource, List<string> STFPath)
		{
			var convert = new System.Func<List<float>, List<float>>(Values => {
				Values[0] *= Mathf.Rad2Deg;
				return Values;
			});

			if(STFPath.Count == 1)
			{
				switch(STFPath[0])
				{
					case "temperature":
						return new ImportPropertyPathPart(Light.name, typeof(Light), new() { "m_ColorTemperature" });
					case "color":
						return new ImportPropertyPathPart(Light.name, typeof(Light), new() { "m_Color.r", "m_Color.g", "m_Color.b" });
					case "brightness":
						return new ImportPropertyPathPart(Light.name, typeof(Light), new() { "m_Intensity" });
					case "range":
						return new ImportPropertyPathPart(Light.name, typeof(Light), new() { "m_Range" });
					case "spot_angle":
						return new ImportPropertyPathPart(Light.name, typeof(Light), new() { "m_SpotAngle" });
				}
			}
			return null;
		}
	}
	public class STFEXP_Light_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Light);
		public const uint _Order = 100;
		public uint Order => _Order;
		public int Priority => 1;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{

			var stfLight = STFResource as STFEXP_Light;
			var lightGo = new GameObject(string.IsNullOrWhiteSpace(stfLight.STF_Name) ? stfLight.light_type : stfLight.STF_Name);
			lightGo.transform.SetParent(stfLight.transform, false);
			lightGo.transform.Rotate(Vector3.right, 90);
			var light = lightGo.AddComponent<Light>();

			switch(stfLight.light_type)
			{
				case "point":
					light.type = LightType.Point;
					light.range = stfLight.range;
					break;
				case "directional":
					light.type = LightType.Directional;
					break;
				case "spot":
					light.type = LightType.Spot;
					light.range = stfLight.range;
					light.spotAngle = stfLight.spot_angle * Mathf.Rad2Deg;
					break;
				default:
					light.type = LightType.Point;
					break;
			};
			light.intensity = stfLight.brightness;
			light.color = stfLight.color;
			light.useColorTemperature = stfLight.use_temperature;
			light.colorTemperature = stfLight.temperature;

			light.shadows = stfLight.shadow ? LightShadows.Soft : LightShadows.None;

			light.enabled = stfLight.enabled;

			stfLight.PropertyConverter = new STF_Light_Converter(light);
			return (new() { light }, null);
		}
	}
}
