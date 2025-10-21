using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stfexp;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors.stfexp
{
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

			return (new() { light }, null);
		}
	}
}
