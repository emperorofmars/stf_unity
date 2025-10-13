#if UNITY_EDITOR
#if STF_AVA_VRCSDK3_FOUND

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using VRC.SDK3.Dynamics.PhysBone.Components;
using UnityEngine;
using com.squirrelbite.stf_unity.modules.stfexp;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public class VRC_STFEXP_Collider_Sphere_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STFEXP_Collider_Sphere);

		public const uint _Order = 100;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfCollider = STFResource as STFEXP_Collider_Sphere;
			var collider = stfCollider.gameObject.AddComponent<VRCPhysBoneCollider>();
			collider.shapeType = VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Sphere;
			collider.radius = stfCollider.radius;
			collider.position = stfCollider.offset_position;

			collider.enabled = stfCollider.enabled;

			return (new() { collider }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_VRC_STFEXP_Collider_Sphere_Processor
	{
		static Register_VRC_STFEXP_Collider_Sphere_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorVRC.STF_VRC_AVATAR_CONTEXT, new VRC_STFEXP_Collider_Sphere_Processor());
		}
	}
}

#endif
#endif
