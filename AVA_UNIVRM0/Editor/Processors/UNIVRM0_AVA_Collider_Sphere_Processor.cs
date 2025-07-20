#if UNITY_EDITOR
#if STF_AVA_UNIVRM0_FOUND

using UnityEditor;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;
using VRM;

namespace com.squirrelbite.stf_unity.ava.univrm0.processors
{
	public class UNIVRM0_AVA_Collider_Sphere_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(AVA_Collider_Sphere);

		public const uint _Order = 100;
		public uint Order => _Order;

		public int Priority => 1;

		public (List<Object> ProcessedObjects, List<Object> ObjectsToRegister) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var stfCollider = STFResource as AVA_Collider_Sphere;

			var colliderGroup = stfCollider.gameObject.AddComponent<VRMSpringBoneColliderGroup>();
			var collider = new VRMSpringBoneColliderGroup.SphereCollider();
			colliderGroup.Colliders = new VRMSpringBoneColliderGroup.SphereCollider[1];
			colliderGroup.Colliders[0] = collider;

			collider.Radius = stfCollider.radius;
			collider.Offset = stfCollider.offset_position;

			colliderGroup.enabled = stfCollider.enabled;

			return (new() { colliderGroup }, null);
		}
	}

	[InitializeOnLoad]
	public class Register_UNIVRM0_AVA_Collider_Sphere_Processor
	{
		static Register_UNIVRM0_AVA_Collider_Sphere_Processor()
		{
			STF_Processor_Registry.RegisterProcessor(DetectorUNIVRM0.STF_UNIVRM0_AVATAR_CONTEXT, new UNIVRM0_AVA_Collider_Sphere_Processor());
		}
	}
}

#endif
#endif
