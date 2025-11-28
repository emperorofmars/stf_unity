using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stfexp;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors.stfexp
{
	public class STF_Camera_Converter : ISTF_PropertyConverter
	{
		private bool Orthographic;
		private Camera Camera;
		public STF_Camera_Converter(Camera Camera, bool Orthographic)
		{
			this.Orthographic = Orthographic;
			this.Camera = Camera;
		}
		public ImportPropertyPathPart ConvertPropertyPath(ISTF_Resource STFResource, List<string> STFPath)
		{
			var convert = new System.Func<List<float>, List<float>>(Values => {
				Values[0] *= Mathf.Rad2Deg;
				return Values;
			});

			if (STFPath.Count == 1 && STFPath[0] == "fov")
			{
				return new ImportPropertyPathPart(Camera.name, typeof(Camera), new() { Orthographic ? "orthographic size" : "field of view" }, !Orthographic ? convert : null);
			}
			else return null;
		}
	}

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
				camera.orthographicSize = stfCamera.fov / 2;
			}
			else
			{
				camera.fieldOfView = stfCamera.fov * Mathf.Rad2Deg;
			}

			camera.enabled = stfCamera.enabled;

			stfCamera.PropertyConverter = new STF_Camera_Converter(camera, camera.orthographic);
			return (new() { camera }, null);
		}
	}
}
