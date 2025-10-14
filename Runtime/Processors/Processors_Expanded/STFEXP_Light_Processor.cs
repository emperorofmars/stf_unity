#if UNITY_EDITOR

using UnityEditor;
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
			var light = lightGo.gameObject.AddComponent<Light>();

			switch(stfLight.light_type)
			{
				case "point":
					light.type = LightType.Point;
					light.range = stfLight.radius;
					break;
				case "directional":
					light.type = LightType.Directional;
					break;
				case "spot":
					light.type = LightType.Spot;
					light.range = stfLight.radius;
					break;
				case "area":
					switch(stfLight.area_shape)
					{
						case "disc":
							light.type = LightType.Disc;
							light.areaSize = new Vector2(stfLight.size, stfLight.size);
							break;
						case "square":
							light.type = LightType.Rectangle;
							light.areaSize = new Vector2(stfLight.size, stfLight.size);
							break;
						case "rectangle":
							light.type = LightType.Rectangle;
							light.areaSize = new Vector2(stfLight.width, stfLight.height);
							break;
						case "ellipsis":
							light.type = LightType.Disc;
							light.areaSize = new Vector2(stfLight.width, stfLight.height);
							break;
						default:
							light.type = LightType.Disc;
							light.areaSize = new Vector2(stfLight.size, stfLight.size);
							break;
					};
					break;
				default:
					light.type = LightType.Point;
					break;
			};
			light.color = stfLight.color;
			light.useColorTemperature = stfLight.use_temperature;
			light.colorTemperature = stfLight.temperature;

			light.enabled = stfLight.enabled;

			return (new() { light }, null);
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	public class Register_STFEXP_Light_Processor
	{
		static Register_STFEXP_Light_Processor()
		{
			STF_Processor_Registry.RegisterProcessor("default", new STFEXP_Light_Processor());
		}
	}
#endif
}

#endif
