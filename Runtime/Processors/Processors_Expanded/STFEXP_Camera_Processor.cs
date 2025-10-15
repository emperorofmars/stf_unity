#if UNITY_EDITOR

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stfexp;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors.stfexp
{
	public class STFEXP_Camera_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Camera);

		public const uint _Order = 100;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{

			var stfCamera = STFResource as STFEXP_Camera;
			var cameraGo = new GameObject(string.IsNullOrWhiteSpace(stfCamera.STF_Name) ? "STF Camera" : stfCamera.STF_Name);
			cameraGo.transform.SetParent(stfCamera.transform, false);
			cameraGo.transform.Rotate(Vector3.right, 90);
			cameraGo.transform.Rotate(Vector3.forward, 180);
			var camera = cameraGo.AddComponent<Camera>();

			camera.aspect = stfCamera.aspect_ratio;

			camera.orthographic = stfCamera.projection == "orthographic";
			if(camera.orthographic)
			{
				Debug.Log(stfCamera.aspect_ratio);
				camera.orthographicSize = stfCamera.fov / 2;
			}
			else
			{
				camera.fieldOfView = stfCamera.fov * Mathf.Rad2Deg;
			}

			camera.enabled = stfCamera.enabled;

			return (new() { camera }, null);
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoad]
	public class Register_STFEXP_Camera_Processor
	{
		static Register_STFEXP_Camera_Processor()
		{
			STF_Processor_Registry.RegisterProcessor("default", new STFEXP_Camera_Processor());
		}
	}
#endif
}

#endif
