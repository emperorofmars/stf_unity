#if UNITY_EDITOR

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using com.squirrelbite.stf_unity.modules.stfexp;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors.stfexp
{
	public class STFEXP_Collider_Sphere_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Collider_Sphere);

		public const uint _Order = 100;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{

			var stfCollider = STFResource as STFEXP_Collider_Sphere;
			var collider = stfCollider.gameObject.AddComponent<SphereCollider>();

			collider.radius = stfCollider.radius;
			collider.center = stfCollider.offset_position;

			collider.enabled = stfCollider.enabled;

			return (new() { collider }, null);
		}
	}
#if UNITY_EDITOR
	[InitializeOnLoad]
	public class Register_STFEXP_Collider_Sphere_Processor
	{
		static Register_STFEXP_Collider_Sphere_Processor()
		{
			STF_Processor_Registry.RegisterProcessor("default", new STFEXP_Collider_Sphere_Processor());
		}
	}
#endif
}

#endif
